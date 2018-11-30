using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InjectionScript.Lsp.Server
{
    internal class CompletionHandler : ICompletionHandler
    {
        private static readonly CompletionRegistrationOptions options = new CompletionRegistrationOptions()
        {
            DocumentSelector = new DocumentSelector(new DocumentFilter()
            {
                Pattern = "**/*.sc"
            }),
            TriggerCharacters = new Container<string>("."),
            ResolveProvider = true
        };
        private readonly IInjectionWorkspace injectionWorkspace;

        public CompletionHandler(IInjectionWorkspace injectionWorkspace)
        {
            this.injectionWorkspace = injectionWorkspace;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => options;

        public Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            if (!injectionWorkspace.TryGetDocument(request.TextDocument.Uri, out var fileContent))
                return Task.FromResult(new CompletionList());

            var completer = new Completer();

            var completions = completer.GetCompletions(fileContent,
                (int)request.Position.Line + 1, (int)request.Position.Character + 1);

            return Task.FromResult(completions);
        }

        public void SetCapability(CompletionCapability capability)
        {

        }
    }
}