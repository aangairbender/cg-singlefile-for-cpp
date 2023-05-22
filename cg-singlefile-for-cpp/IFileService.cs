using System.Collections.Generic;

namespace cg_singlefile_for_cpp
{
    public interface IFileService
    {
        string[] ReadAllLines(string filepath);
        void WriteAllLines(string filepath, IEnumerable<string> lines);
    }
}