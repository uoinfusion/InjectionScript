using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InjectionScript.Lsp.Server
{
    internal class NavigationHandler : IDefinitionHandler
    {
        private readonly IInjectionWorkspace injectionWorkspace;

        public NavigationHandler(IInjectionWorkspace injectionWorkspace)
        {
            this.injectionWorkspace = injectionWorkspace;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions()
            {
                DocumentSelector = new DocumentSelector(new DocumentFilter()
                {
                    Pattern = "**/*.sc"
                })
            };
        }

        public Task<LocationOrLocations> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            var navigator = new Navigator(injectionWorkspace);
            var location = navigator.GetDefinition(request.TextDocument.Uri,
                (int)request.Position.Line + 1, (int)request.Position.Character + 1);

            if (location != null)
            {
                location.Range.Start.Line--;
                location.Range.Start.Character--;
                location.Range.End.Line--;
                location.Range.End.Character--;
                return Task.FromResult(new LocationOrLocations(location));
            }

            return Task.FromResult(new LocationOrLocations(Array.Empty<Location>()));
        }

        public void SetCapability(DefinitionCapability capability)
        {

        }
    }
}