using System.Collections.Generic;
using System.IO;

namespace cg_singlefile_for_cpp
{
    public class FileService : IFileService
    {
        public string[] ReadAllLines(string filepath)
        {
            return File.ReadAllLines(filepath);
        }

        public void WriteAllLines(string filepath, IEnumerable<string> lines)
        {
            File.WriteAllLines(filepath, lines);
        }
    }
}
