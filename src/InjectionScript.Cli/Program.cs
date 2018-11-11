using InjectionScript.Analysis;
using InjectionScript.Interpretation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InjectionScript.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args[0].Equals("extract-unkwnown-calls"))
            {
                ExtractCallsFromDirectory(args[1]);

                foreach (var call in extractedCallsCount.OrderByDescending(x => x.Value))
                    Console.WriteLine($"{call.Value}\t\t\t{call.Key}");
            }
        }

        private static Dictionary<string, int> extractedCallsCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private static void ExtractCallsFromDirectory(string path)
        {
            var files = Directory.GetFiles(path, "*.sc");

            foreach (var file in files)
                ExtractCallsFromFile(file);

            var subDirectories = Directory.GetDirectories(path);
            foreach (var subDirectory in subDirectories)
                ExtractCallsFromDirectory(subDirectory);
        }

        private static void ExtractCallsFromFile(string file)
        {
            Console.WriteLine($"Extracting file {file}");

            var runtime = new Runtime();
            runtime.Load(file);
            var walker = new CallWalker();
            walker.VisitCall = (context) =>
            {
                var name = context.SYMBOL().GetText();
                if (runtime.Metadata.SubrutineExists(name))
                    return;

                var builder = new StringBuilder();

                builder.Append(name);

                var argsCount = context.argumentList()?.arguments()?.argument()?.Count() ?? 0;
                builder.Append('`');
                builder.Append(argsCount);

                string key = builder.ToString();
                if (extractedCallsCount.TryGetValue(key, out int count))
                    extractedCallsCount[key] = count + 1;
                else
                    extractedCallsCount[key] = 1;
            };

            walker.Walk(runtime.CurrentFileSyntax);
        }
    }
}
