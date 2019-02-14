using System.IO;
using System.Text;
using System.Collections.Generic;

namespace rpi_dotnet
{
    public class FileWrapper : IFileWrapper
    {

        public string Read(string filePath)
        {
            return File.ReadAllText(filePath);
        }
        public void Write(string filePath, string text)
        {
            File.OpenWrite(filePath).Write(Encoding.ASCII.GetBytes(text));
        }
        public List<string> ListDirectories(string path)
        {
            var directories = new List<string>();
            foreach(var dir in Directory.GetDirectories(path)){
                var dirInfo = new DirectoryInfo(dir);
                directories.Add(dirInfo.Name);
            }
            return directories;
        }
    }
}