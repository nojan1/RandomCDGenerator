using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCDGenerator.Engine
{
    public class M3UCreator
    {
        private TrackList tracklist;

        public M3UCreator(TrackList trackListArg)
        {
            this.tracklist = trackListArg;
        }

        public async Task Write(string path)
        {
            using (var stream = new StreamWriter(path))
            {
                await stream.WriteLineAsync("#EXTM3U");

                foreach (var track in tracklist.Tracks)
                {
                    await stream.WriteLineAsync(string.Format("#EXTINF:{0},{1} - {2}", (int)track.TaglibFile.Properties.Duration.TotalSeconds
                                                                                     , string.IsNullOrEmpty(track.TaglibFile.Tag.JoinedAlbumArtists) ? track.TaglibFile.Tag.JoinedPerformers : track.TaglibFile.Tag.JoinedAlbumArtists
                                                                                     , track.TaglibFile.Tag.Title));

                    await stream.WriteLineAsync(track.Path);
                }
            }
        }
    }
}
