namespace Video_Library_Api.Vendor.MotionDSP.Copyright
{
    public class Match
    {
        #if Linux
        public ulong VideoID1
        { 
            get; 
            set;
        }
        public ulong VideoID2
        { 
            get; 
            set;
        }
        public int Offset1 
        { 
            get;
            set;
        }
        public int Offset2
        { 
            get;
            set;
        }
        public int Length
        { 
            get;
            set;
        }
        public float Score
        { 
            get;
            private set;
        }

        public Match(ulong VideoID1, ulong VideoID2, int Offset1, int Offset2, int Length, float Score)
        {
            this.VideoID1 = VideoID1;
            this.VideoID2 = VideoID2;
            this.Offset1 = Offset1;
            this.Offset2 = Offset2;
            this.Length = Length;
            this.Score = Score;
        }
        #endif
    }
}