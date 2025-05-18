using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Video_Library_Api.Models;

namespace Video_Library_Api.Contexts 
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :
            base(options)
        {}

        public DbSet<Video> Videos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RelatedVideos> RelatedVideos { get; set; }
        public DbSet<VideoGeolocation> VideoGeolocation {get; set;}
        public DbSet<VideosTags> VideosTags { get; set; }
        public DbSet<DirectoryEntry> DirectoryEntries { get; set; }
        public DbSet<DirectoryInf> DirectoryInfos { get; set;} 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            // Videos
            modelBuilder.Entity<Video>()
                .Property(v => v.ProcessesLeft)
                .HasDefaultValue(0);

            modelBuilder.Entity<VideoGeolocation>()
                .HasKey(vg => vg.VideoId);
                
            modelBuilder.Entity<VideoGeolocation>()
                .HasOne(vg => vg.Video)
                .WithOne(v => v.VideoGeolocation)
                .HasForeignKey<VideoGeolocation>(vg => vg.VideoId);

            // Tags
            List<Tag> tagsToSeed = TagsFromFile("classes.txt");

            modelBuilder.HasSequence<int>("Tags_Id_sequence")
                        .StartsAt(tagsToSeed.Count + 1);

            modelBuilder.Entity<Tag>()
                .Property(t => t.Id)
                .HasDefaultValueSql("nextval('\"Tags_Id_sequence\"')");
            
            modelBuilder.Entity<Tag>()
                .Property(t => t.UserAdded)
                .HasDefaultValue(false);

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();
            
            modelBuilder.Entity<Tag>()
                .HasData(tagsToSeed);

            // VideosTags
            modelBuilder.Entity<VideosTags>()
                .HasKey(vt => new { vt.VideoId, vt.TagId, vt.From, vt.To });

            modelBuilder.Entity<VideosTags>()
                .HasOne(vt => vt.Video)
                .WithMany(v => v.VideosTags)
                .HasForeignKey(vt => vt.VideoId);

            modelBuilder.Entity<VideosTags>()
                .HasOne(vt => vt.Tag)
                .WithMany(t => t.VideosTags)
                .HasForeignKey(vt => vt.TagId);


            // RelatedVideos
            modelBuilder.Entity<RelatedVideos>()
                .HasKey(vt => new { vt.Video1Id, vt.Video2Id });

            modelBuilder.Entity<RelatedVideos>()
                .HasOne(vt => vt.Video1)
                .WithMany(v => v.RelatedFromVideos)
                .HasForeignKey(vt => vt.Video1Id);

            modelBuilder.Entity<RelatedVideos>()
                .HasOne(vt => vt.Video2)
                .WithMany(t => t.RelatedToVideos)
                .HasForeignKey(vt => vt.Video2Id);


            //DirectoryEntry
            modelBuilder.Entity<DirectoryEntry>()
                .HasOne(di => di.DirectoryInf)
                .WithMany(de => de.DirectoryEntries)
                .HasForeignKey(di => di.DirectoryHash);

            modelBuilder.Entity<DirectoryEntry>()
                .HasKey(de =>  de.FilePath);
            
            modelBuilder.Entity<DirectoryInf>()
                .HasKey(di =>  di.DirectoryHash);
            
        }
        private List<Tag> TagsFromFile (string path) //Path must be .txt file//
        {
            List<string> lines = new List<string>();
            List<Tag> tags = new List<Tag>();

            using (StreamReader reader = new StreamReader(path))
            {
                int id = 1;
                while(!reader.EndOfStream)
                {
                    tags.Add(new Tag() {
                        Id = id++,
                        Name = reader.ReadLine()
                    });
                }
            }
            return tags;
        }
    }
}