using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Video_Library_Api.Paging
{
    /// <summary>
    /// Params for sorting, searching and paging
    /// </summary>
    public class PagingParams
    {
        //for paging
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        [DefaultValue(5)]
        public int PageSize { get; set; } = 5;


        /// <summary>Parameter to use to sort videos (case-sensitive).</summary>
        public string PropertySort { get; set; } = "FileName";

        /// <summary>Sorting order ("ASC" or "DESC").</summary>
        [DefaultValue("ASC")]
        public string SortDirection { get; set; } = "ASC";


        /// <summary>List of tags separated by "," that are associated with videos (e.g. Tags=tagname1,tagname2,tagname3...).</summary>
        public string Tags { get; set; } = ""; 


        //for substring search
        public string FileName { get; set; } = "";
        public string FormatLongName { get; set; } = "";
        public string Duration { get; set; } = "";
        public string Size { get; set; } = "";
        public string BitRate { get; set; } = "";
        public string Width { get; set; } = "";
        public string Height { get; set; } = "";
        public string StreamsJSON { get; set; } = "";
        public string CodecName { get; set; } = "";
        
        //for search between
        public double DurationFrom { get; set; } = 0;
        public double DurationTo { get; set; } = double.MaxValue;
        public double SizeFrom { get; set; } = 0;
        public double SizeTo { get; set; } = double.MaxValue;
        public double BitRateFrom { get; set; } = 0;
        public double BitRateTo { get; set; } = double.MaxValue;
        public double WidthFrom { get; set; } = 0;
        public double WidthTo { get; set; } = double.MaxValue;
        public double HeightFrom { get; set; } = 0;
        public double HeightTo { get; set; } = double.MaxValue;

        
        /// <summary>Latitude of location on video.</summary>
        public double Latitude { get; set; } = double.MaxValue;

        /// <summary>Longitude of location on video.</summary>
        public double Longitude { get; set; } = double.MaxValue;

        /// <summary>Distance from given point in kilometers.</summary>
        [DefaultValue(0)]
        public double Distance { get; set; } = 0; 


        /// <summary>List done or currently processing videos.</summary>
        [DefaultValue(true)]
        public bool FinishedProcessing  { get; set; } = true;
    }
}
