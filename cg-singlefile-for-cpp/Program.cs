using System;
using Microsoft.Extensions.CommandLineUtils;

namespace cg_singlefile_for_cpp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = new CommandLineApplication();
            cmd.Name = System.AppDomain.CurrentDomain.FriendlyName;
            cmd.FullName = "Singlefile for cpp";
            cmd.Description = "Converts mutiple cpp files into a single one";
            var mainArg = cmd.Argument("main file", "The path to the entry point file (usually main.cpp)");
            var outputArg = cmd.Option("-o | --output <value>", "output file path", CommandOptionType.SingleValue);

            cmd.OnExecute(() =>
            {
                if (!outputArg.HasValue())
                {
                    Console.WriteLine("Invalid arguments. See --help for usage guide.");
                    return -1;
                }
                var sourceProcessor = new SourceProcessor(new FileService());
                sourceProcessor.Process(mainArg.Value, outputArg.Value());
                return 0;
            });

            cmd.HelpOption("-h | --help");
            cmd.Execute(args);
        }
    }
}
