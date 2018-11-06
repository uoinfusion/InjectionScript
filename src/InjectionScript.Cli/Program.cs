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

                foreach (var call in extractedCalls.OrderBy(x => x))
                    Console.WriteLine(call);
            }
        }

        private static HashSet<string> extractedCalls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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
                var ns = context.callNamespace()?.SYMBOL()?.GetText();
                var name = context.SYMBOL().GetText();
                if (string.IsNullOrEmpty(ns) && runtime.Metadata.SubrutineExists(name))
                    return;

                var builder = new StringBuilder();

                if (!string.IsNullOrEmpty(ns))
                {
                    builder.Append(ns);
                    builder.Append('.');
                }

                builder.Append(name);

                var argsCount = context.argumentList()?.arguments()?.argument()?.Count() ?? 0;
                builder.Append('`');
                builder.Append(argsCount);

                extractedCalls.Add(builder.ToString());
            };

            walker.Walk(runtime.CurrentFileSyntax);
        }
    }
}
