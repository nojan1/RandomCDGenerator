using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCDGenerator.Engine
{
    public class NoMoreFilesException : Exception { }

    public class Mp3FileInfo
    {
        public string Path { get; set; }
        public TagLib.File TaglibFile { get; set; }
    }

    public class Mp3FilePathProvider
    {
        private List<string> files;

        public int FileCount
        {
            get
            {
                if(files != null)
                {
                    return files.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public async Task Init(string folderPath)
        {
            files = new List<string>();
            await Task.Run(() => {
                files.AddRange(Directory.GetFiles(folderPath, "*.mp3", SearchOption.AllDirectories));     
            });
        }

        public Mp3FileInfo Next()
        {
            if (files == null)
                throw new Exception("Not initialized!");

            if (files.Count == 0)
                throw new NoMoreFilesException();

            var rnd = new Random();
            string path = "";
            TagLib.File taglibFile = null;

            while(taglibFile == null)
            { 
                int index = rnd.Next(0, files.Count);
                path = files[index];
                files.RemoveAt(index);

                try
                {
                    taglibFile = TagLib.File.Create(path);
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    //Don't care
                }
            }

            return new Mp3FileInfo
            {
                Path = path,
                TaglibFile = taglibFile
            };
        }

        public void ReInsert(Mp3FileInfo file)
        {
            files.Add(file.Path);
        }
    }
}
