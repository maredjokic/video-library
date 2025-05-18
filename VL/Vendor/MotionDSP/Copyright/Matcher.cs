using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Video_Library_Api.Vendor.MotionDSP.Copyright
{
    public class Matcher
    {
        #if Linux
        [DllImport("libfpmatch.so", EntryPoint = "matcher_add_video", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddVideo(ulong videoID, byte[] fingerprint, ushort length);
        public static string GenerateFp(string videoFilePath, string videoDirectory, string videoName)
        {
            string videoPath = videoFilePath;
            string fpPath = Path.Combine(videoDirectory, $"{videoName}.fp");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/opt/copyright/fpgen/fpgen",
                    Arguments = $"\"{videoPath}\" \"{fpPath}\"",
                    RedirectStandardOutput = false,
                    CreateNoWindow = true,
                }

            };

            process.Start();
            process.WaitForExit();
            return fpPath;
        }

        [DllImport("libfpmatch.so", EntryPoint = "matcher_match", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Match(ulong videoID, byte[] fingerprint, ushort length, float rejectThreshold);

        [DllImport("libfpmatch.so", EntryPoint = "matcher_remove_video", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RemoveVideo(ulong videoID);

        [DllImport("libfpmatch.so", EntryPoint = "matcher_remove_all", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RemoveAll();
        
        [DllImport("libfpmatch.so", EntryPoint = "matcher_size", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Size();

        [DllImport("libfpmatch.so", EntryPoint = "matches_video1", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong Video1(int index);

        [DllImport("libfpmatch.so", EntryPoint = "matches_video2", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong Video2(int index);

        [DllImport("libfpmatch.so", EntryPoint = "matches_offset1", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Offset1(int index);
        
        [DllImport("libfpmatch.so", EntryPoint = "matches_offset2", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Offset2(int index);

        [DllImport("libfpmatch.so", EntryPoint = "matches_length", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Length(int index);

        [DllImport("libfpmatch.so", EntryPoint = "matches_score", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        private static extern float Score(int index);

        [DllImport("libfpmatch.so", EntryPoint = "matches_size", 
            ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MatchesSize();

        public static Match GetMatch(int index) {

            ulong videoID1 = Video1(index);
            ulong videoID2 = Video2(index);
            int offset1 = Offset1(index);
            int offset2 = Offset2(index);
            int length = Length(index);
            float score = Score(index);

            return new Match(videoID1, videoID2, offset1, offset2, length, score);
        }

        #endif
    }
}