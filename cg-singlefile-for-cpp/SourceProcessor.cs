using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cg_singlefile_for_cpp
{
    public class SourceProcessor
    {
        private readonly Regex includeSrcRegex = new Regex("#include \"(.*)\"");
        private readonly Regex includeLibRegex = new Regex("#include <(.*)>");

        private readonly HashSet<string> includedSrc = new HashSet<string>();
        private readonly HashSet<string> includedLib = new HashSet<string>();

        public SourceProcessor()
        {
        }

        public void Process(string rootFile, string outputFile)
        {
            includedSrc.Clear();
            includedLib.Clear();
            var lines = UnfoldIncludes(new FileInfo(rootFile));
            File.WriteAllLines(outputFile, lines);
        }

        private List<string> UnfoldIncludes(FileInfo file)
        {
            includedSrc.Add(file.FullName);
            var result = new List<string>();
            var lines = ReadLines(file.FullName);
            foreach (var line in lines)
            {
                if (line.EndsWith("singlefile-skip-line")) continue;

                if (line.TrimStart().StartsWith("//")) continue;

                if (IsInclude(line, includeSrcRegex, out var includeFilename))
                {
                    var includeFile = new FileInfo(Path.Combine(file.DirectoryName, includeFilename));
                    if (includedSrc.Contains(includeFile.FullName)) continue;
                    result.AddRange(UnfoldIncludes(includeFile));
                }
                else if (IsInclude(line, includeLibRegex, out var includeLibFile))
                {
                    if (includedLib.Contains(includeLibFile)) continue;
                    includedLib.Add(includeLibFile);
                    result.Add(line);
                }
                else if (IsPragmaOnce(line))
                {
                    continue;
                }
                else
                {
                    result.Add(line);
                }
            }
            return result;
        }

        private static bool IsPragmaOnce(string line)
        {
            return line.Trim() == "#pragma once";
        }

        private static bool IsInclude(string line, Regex regex, out string includeFile)
        {
            var includeMatch = regex.Match(line);
            if (includeMatch.Success)
            {
                includeFile = includeMatch.Groups[1].Value;
                return true;
            } else
            {
                includeFile = null;
                return false;
            }
        }

        private string[] ReadLines(string filepath)
        {
            return File.ReadAllLines(filepath);
        }
    }
}
