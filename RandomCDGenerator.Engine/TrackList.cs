using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCDGenerator.Engine
{
    public class TrackList
    {
        public double Duration { get; private set; }
        public List<Mp3FileInfo> Tracks { get; private set; }
        public double TimeRemaining
        {
            get
            {
                return Duration - Tracks.Sum((t) => t.TaglibFile.Properties.Duration.TotalSeconds);
            }
        }

        public TrackList(double targetDuration)
        {
            Duration = targetDuration;
            Tracks = new List<Mp3FileInfo>();
        }

        public bool Add(Mp3FileInfo file)
        {
            if(file.TaglibFile.Properties.Duration.TotalSeconds <= TimeRemaining)
            {
                Tracks.Add(file);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
