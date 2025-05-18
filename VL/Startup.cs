using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Video_Library_Api.Contexts;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services;
using Video_Library_Api.Services.Semaphores;
using Video_Library_Api.Services.Strategies;
using Video_Library_Api.StreamsData;


namespace Video_Library_Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        private readonly IHostingEnvironment _hostingEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = string.Empty;
            if(Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == "true")
            {
                if(_hostingEnvironment.IsDevelopment())
                {
                    connectionString = Configuration.GetConnectionString("DockerConnection");
                }
                else
                {
                    connectionString = Configuration.GetConnectionString("DefaultConnection");
                }
            }
            else
            {
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            }

            services.AddDbContext<AppDbContext>(options =>
                    options
                        // .UseLazyLoadingProxies()
                        .UseNpgsql(connectionString, o => o.UseNetTopologySuite()));

            services.AddMvc(options => 
            {
                // Prevent the following exception: 'This method does not support GeometryCollection arguments'
                // See: https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/issues/585
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Point)));
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(LineString)));
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(MultiLineString)));
            })
            .AddJsonOptions(options =>
            {
                foreach (var converter in GeoJsonSerializer.Create(new GeometryFactory(new PrecisionModel(), 4326)).Converters)
                {
                    options.SerializerSettings.Converters.Add(converter);
                }
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpContextAccessor();
            services.AddResponseCompression();

            services.AddAutoMapper(typeof(Startup));
            
            services.AddSingleton<SeedDatabase>();
            services.AddSingleton<StreamsDictionary>();


            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IFeedsService, FeedsService>();
            services.AddScoped<IScanDirectoryService, ScanDirectoryService>();

            services.AddSingleton<BackgroundCPUServiceSemaphore>();
            services.AddSingleton<BackgroundGPUServiceSemaphore>();
            services.AddSingleton<FPServiceSemaphore>();

            Environment.SetEnvironmentVariable("NumberOfBackgroundServices", "0");

            RegisterBackgroundService<MLService>(services);
            if(_hostingEnvironment.IsDevelopment())
            {
                services.AddScoped<IMLStrategy, MLDevelopmentStrategy>();
            }
            else
            {
                services.AddScoped<IMLStrategy, MLProductionStrategy>();

                string mlAddress = Configuration.GetValue<string>("MLAddress");
                
                services.AddHttpClient("ml", client =>
                {
                    client.BaseAddress = new Uri(mlAddress);
                    client.DefaultRequestHeaders.Add(
                        "Accept", 
                        "application/json");
                    client.Timeout = TimeSpan.FromDays(2);
                });
            }

            RegisterBackgroundService<FPService>(services);
            RegisterBackgroundService<ThumbnailService>(services);
            RegisterBackgroundService<PreviewService>(services);
            RegisterBackgroundService<TranscodingService>(services);
            RegisterBackgroundService<GeoInfoService>(services);

            services.AddScoped<IVideoRepository, VideoRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IRelatedVideosRepository, RelatedVideosRepository>();
            services.AddScoped<IVideoGeolocationRepository, VideoGeolocationRepository>();
            services.AddScoped<IVideoTagsRepository, VideoTagsRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Video Library API", Version = "v1" });

                //Locate the XML file being generated by ASP.NET...
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Video Library API V1");
                c.RoutePrefix = "docs";
            });

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseMvc();
            app.UseStaticFiles();
            app.UseDirectoryBrowser();
            app.UseResponseCompression();
        }

        private IServiceCollection RegisterBackgroundService<T>(IServiceCollection services) where T : class
        {
            string envVariableName = "NumberOfBackgroundServices";
            string envNumberOfServices = Environment.GetEnvironmentVariable(envVariableName);
            int numberOfServices = int.Parse(envNumberOfServices) + 1;
            Environment.SetEnvironmentVariable(envVariableName, numberOfServices.ToString());
            return services.AddScoped<T>();
        }
    }
}
