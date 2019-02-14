using System.Collections.Generic;

namespace rpi_dotnet{
    public interface IFileWrapper{
        string Read(string fileName);
        void Write(string fileName, string text);
        List<string> ListDirectories(string path);
    }
}