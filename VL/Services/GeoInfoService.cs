using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services.Semaphores;
using System.Threading;

namespace Video_Library_Api.Services
{
    public class GeoInfoService : BackgroundService
    {
        public GeoInfoService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            BackgroundCPUServiceSemaphore semaphore)
            : base(scopeFactory, logger, semaphore)
        { }
        protected async override Task StartAsync(Video video, IServiceScope scope)
        {
            string hash = video.Id;
            string klvPath = video.GetKlvPath();
            
            Console.WriteLine($"[{hash}]: Get GeoInfo started");
            _logger.LogInformation($"[{hash}]: Get GeoInfo started");

            try
            {
                var videoService = scope.ServiceProvider.GetService<IVideoService>();
                var videoGeolocationRepository = scope.ServiceProvider.GetService<IVideoGeolocationRepository>();
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();


                VideoGeolocation videoGeolocation=null;
                try
                {
                    videoGeolocation = await GetGeoCordinatesKlv2Json(klvPath, hash);
                }
                catch(Exception exc)
                {
                    Console.WriteLine(exc);
                }
                
                if(videoGeolocation != null)
                {
                    video.VideoGeolocation = videoGeolocation;

                    videoGeolocationRepository.Add(videoGeolocation);
                    await  unitOfWork.CompleteAsync();
                }
            }
            catch(Exception exc)
            {
                Console.WriteLine($"[{hash}]: Get GeoInfo exception");
                Console.WriteLine(exc);
                _logger.LogInformation($"[{hash}]: Get GeoInfo exception");
                _logger.LogInformation(exc.ToString());
            }

            Console.WriteLine($"[{hash}]: Get GeoInfo finished");
            _logger.LogInformation($"[{hash}]: Get GeoInfo finished");
        }

        public async Task<VideoGeolocation> GetGeoCordinatesKlv2Json(string klv2jsonPath, string hash)
        {
            if(!File.Exists(klv2jsonPath))
            {
                return null;
            }
            string sjson = await File.ReadAllTextAsync(klv2jsonPath);

            if (!string.IsNullOrEmpty(sjson))
            {
                JToken json = null;

                try
                {
                   json = JToken.Parse(sjson);
                }
                catch (JsonReaderException jre)
                {
                    Console.WriteLine(jre);
                    return null;
                }

                if(!IsNullOrEmpty(json))
                {
                    return null;
                }
                else
                {
                    List<Coordinate> coordinates = new List<Coordinate>();

                    Polygon polygon = null;//new Polygon(null){ SRID = 4326 };

                    foreach (JToken klv in json)
                    {
                        double? x = IsNullOrEmpty(klv["frameCenterLatitude"]) ? (double?)klv["frameCenterLatitude"] : null;
                        double? y = IsNullOrEmpty(klv["frameCenterLongitude"]) ? (double?)klv["frameCenterLongitude"] : null;
                        
                        if(x!=null && y!=null)
                        {
                            double? x1 = IsNullOrEmpty(klv["deviceLatitude"]) ? (double?)klv["deviceLatitude"] : null;
                            double? y1 = IsNullOrEmpty(klv["deviceLongitude"]) ? (double?)klv["deviceLongitude"] : null;
                            if(x1!=null && y1!=null)
                            {   
                                coordinates.Add(new Coordinate(Math.Round((double)y1, 6),Math.Round((double)x1, 6)));
                            }
                            else
                            {
                                coordinates.Add(new Coordinate(Math.Round((double)y, 6),Math.Round((double)x, 6)));
                            }
                        }
                        else
                            return null;

                        double? la0 = IsNullOrEmpty(klv["cornerLatitude0"]) ? (double?)klv["cornerLatitude0"] : null;
                        double? lo0 = IsNullOrEmpty(klv["cornerLongitude0"]) ? (double?)klv["cornerLongitude0"] : null;

                        double? la1 = IsNullOrEmpty(klv["cornerLatitude1"]) ? (double?)klv["cornerLatitude1"] : null;
                        double? lo1 = IsNullOrEmpty(klv["cornerLongitude1"]) ? (double?)klv["cornerLongitude1"] : null;

                        double? la2 = IsNullOrEmpty(klv["cornerLatitude2"]) ? (double?)klv["cornerLatitude2"] : null;
                        double? lo2 = IsNullOrEmpty(klv["cornerLongitude2"]) ? (double?)klv["cornerLongitude2"] : null;

                        double? la3 = IsNullOrEmpty(klv["cornerLatitude3"]) ? (double?)klv["cornerLatitude3"] : null;
                        double? lo3 = IsNullOrEmpty(klv["cornerLongitude3"]) ? (double?)klv["cornerLongitude3"] : null;

                        if(la0==null || lo0==null
                        || la1==null || lo1==null
                        || la2==null || lo2==null
                        || la3==null || lo3==null)
                        {
                            la0 = IsNullOrEmpty(klv["offsetCornerLatitude0"]) ? ((double?)klv["offsetCornerLatitude0"]) + x : null ;
                            lo0 = IsNullOrEmpty(klv["offsetCornerLongitude0"]) ? ((double?)klv["offsetCornerLongitude0"]) + y : null;

                            la1 = IsNullOrEmpty(klv["offsetCornerLatitude1"]) ? ((double?)klv["offsetCornerLatitude1"]) + x : null;
                            lo1 = IsNullOrEmpty(klv["offsetCornerLongitude1"]) ? ((double?)klv["offsetCornerLongitude1"]) + y : null;

                            la2 = IsNullOrEmpty(klv["offsetCornerLatitude2"]) ? ((double?)klv["offsetCornerLatitude2"]) + x : null;
                            lo2 = IsNullOrEmpty(klv["offsetCornerLongitude2"]) ? ((double?)klv["offsetCornerLongitude2"]) + y : null;

                            la3 = IsNullOrEmpty(klv["offsetCornerLatitude3"]) ? ((double?)klv["offsetCornerLatitude3"]) + x : null;
                            lo3 = IsNullOrEmpty(klv["offsetCornerLongitude3"]) ? ((double?)klv["offsetCornerLongitude3"]) + y : null;
                        }

                        if(la0!=null && lo0!=null
                        && la1!=null && lo1!=null
                        && la2!=null && lo2!=null
                        && la3!=null && lo3!=null)
                        {
                            List<Coordinate> coordinatesPolygon= new List<Coordinate>();
                            coordinatesPolygon.Add(new Coordinate(Math.Round((double)lo0,6),Math.Round((double)la0,6)));
                            coordinatesPolygon.Add(new Coordinate(Math.Round((double)lo1,6),Math.Round((double)la1,6)));
                            coordinatesPolygon.Add(new Coordinate(Math.Round((double)lo2,6),Math.Round((double)la2,6)));
                            coordinatesPolygon.Add(new Coordinate(Math.Round((double)lo3,6),Math.Round((double)la3,6)));
                            coordinatesPolygon.Add(new Coordinate(Math.Round((double)lo0,6),Math.Round((double)la0,6)));

                            Coordinate[] onePoly = coordinatesPolygon.ToArray();

                            if(onePoly.Length != 0)
                            {
                                Polygon onePolygon = new Polygon(new LinearRing(onePoly)){ SRID=4326 };

                                if(polygon != null)
                                {
                                    try
                                    {   
                                        Polygon polygon1 = polygon.Union(onePolygon) as Polygon; //pravi uniju vec postojecih poligona

                                        for(int i=0; i<polygon1.Coordinates.Length; i++)
                                        {
                                            polygon1.Coordinates[i].X = Math.Round(polygon1.Coordinates[i].X, 6);
                                            polygon1.Coordinates[i].Y = Math.Round(polygon1.Coordinates[i].Y, 6);
                                        }
                                        
                                        polygon = polygon1;
                                    }
                                    catch(Exception)// exc)//non-noded intersection between
                                    {
                                        //Console.WriteLine(exc);
                                    }
                                }
                                else
                                {
                                    polygon=onePolygon;
                                }
                            }
                        }
                    }

                    LineString multipoint= null;

                    if(coordinates.Count != 0)
                    {
                        multipoint =  new LineString(coordinates.ToArray()){ SRID = 4326 };//tacke putanje kojima se kretala kamera
                    }
                    
                    //Console.WriteLine(multipoint.ToString());

                    VideoGeolocation videoGeolocation = new VideoGeolocation(){
                            Video = null,
                            CameraLine = multipoint,
                            FilmedArea = polygon,
                            VideoId = hash
                    };

                    return videoGeolocation;
                }
            }
            return null;
        }


        //for checking jsons
        public bool IsNullOrEmpty(JToken token)
        {
            return !((token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null));
        }
    }
}