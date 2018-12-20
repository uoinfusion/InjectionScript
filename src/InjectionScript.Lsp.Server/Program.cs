using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Server;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InjectionScript.Lsp.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.Debug)
                        Debugger.Launch();
                });

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithHandler<CompletionHandler>()
                    .WithHandler<TextDocumentHandler>()
                    .WithServices(serviceCollection =>
                    {
                        serviceCollection.AddSingleton(typeof(IDiagnosticReporter), typeof(DiagnosticReporter));
                        serviceCollection.AddSingleton(typeof(IInjectionWorkspace), typeof(InjectionWorkspace));
                    })
                );

            await server.WaitForExit;
        }

        private class Options
        {
            [Option('d', "debug", Required = false)]
            public bool Debug { get; set; }
        }

    }
}
