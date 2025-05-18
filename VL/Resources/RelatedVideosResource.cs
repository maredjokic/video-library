namespace Video_Library_Api.Resources
{
    public class RelatedVideosResource
    {
        public string Video1Id { get; set; }
        public string Video2Id { get; set; }
        public int Offset1 { get; set; }
        public int Offset2 { get; set; }
        public int Length { get; set; }
        public float Score { get; set; }
    }
}