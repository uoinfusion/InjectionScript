using InjectionScript.Analysis;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InjectionScript.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args[0].Equals("analyze"))
            {
                AnalyseDirectory(args[1]);

                foreach (var call in extractedCallsCount.OrderByDescending(x => x.Value))
                    Console.WriteLine($"{call.Value}\t\t\t{call.Key}");
            }
        }

        private static Dictionary<string, int> extractedCallsCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private static void AnalyseDirectory(string path)
        {
            var files = Directory.GetFiles(path, "*.sc");

            foreach (var file in files)
                AnalyzeFile(file);

            var subDirectories = Directory.GetDirectories(path);
            foreach (var subDirectory in subDirectories)
                AnalyseDirectory(subDirectory);
        }

        private static void AnalyzeFile(string file)
        {
            Console.WriteLine($"Analyzing {file}");

            var runtime = new InjectionRuntime();

            var messages = runtime.Load(file);
            var originalCollor = Console.ForegroundColor;
            foreach (var message in messages.OrderBy(m => m.StartLine))
            {
                switch (message.Severity)
                {
                    case MessageSeverity.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(message.ToString());
                        break;
                    case MessageSeverity.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(message.ToString());
                        break;
                }
            }
            Console.ForegroundColor = originalCollor;
        }
    }
}
