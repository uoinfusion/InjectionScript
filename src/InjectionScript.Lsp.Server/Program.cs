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
            Debugger.Launch();

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithHandler<CompletionHandler>()
                    .WithHandler<TextDocumentHandler>()
                    .WithServices(serviceCollection =>
                    {
                        serviceCollection.AddSingleton<IInjectionWorkspace>(new InjectionWorkspace());
                    })
                );

            await server.WaitForExit;
        }

    }
}
