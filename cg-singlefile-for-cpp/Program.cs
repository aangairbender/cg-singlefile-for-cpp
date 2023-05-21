using System;
using Microsoft.Extensions.CommandLineUtils;

namespace cg_singlefile_for_cpp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = new CommandLineApplication();
            var dirArg = cmd.Option("-d | --dir <value>", "directory with the main file", CommandOptionType.SingleValue);
            var mainArg = cmd.Option("-m | --main <value>", "main file name (relative)", CommandOptionType.SingleValue);
            var outputArg = cmd.Option("-o | --output <value>", "output file", CommandOptionType.SingleValue);

            cmd.OnExecute(() =>
            {
                if (!dirArg.HasValue() || !mainArg.HasValue() || !outputArg.HasValue())
                {
                    Console.WriteLine("Invalid arguments. See --help for usage guide.");
                    return -1;
                }
                var sourceProcessor = new SourceProcessor(new System.IO.DirectoryInfo(dirArg.Value()));
                sourceProcessor.Process(mainArg.Value(), outputArg.Value());
                return 0;
            });

            cmd.HelpOption("-h | --help");
            cmd.Execute(args);
        }
    }
}
