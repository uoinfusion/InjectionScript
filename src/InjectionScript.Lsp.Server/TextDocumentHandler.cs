using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InjectionScript.Lsp.Server
{
    internal class TextDocumentHandler : ITextDocumentSyncHandler
    {
        private readonly OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer _router;
        private readonly IInjectionWorkspace injectionWorkspace;
        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.sc"
            }
        );

        private SynchronizationCapability _capability;

        public TextDocumentHandler(OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer router,
            IInjectionWorkspace injectionWorkspace)
        {
            _router = router;
            this.injectionWorkspace = injectionWorkspace;
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        public Task<Unit> Handle(DidChangeTextDocumentParams notification, CancellationToken token)
        {
            return Task.Run(() =>
            {
                injectionWorkspace.UpdateDocument(notification.TextDocument.Uri, notification.ContentChanges.Single().Text);

                return Unit.Value;
            });
        }

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.GetRegistrationOptions() => new TextDocumentChangeRegistrationOptions()
        {
            DocumentSelector = _documentSelector,
            SyncKind = Change
        };

        public void SetCapability(SynchronizationCapability capability) => _capability = capability;

        public Task<Unit> Handle(DidOpenTextDocumentParams notification, CancellationToken token)
        {
            return Task.Run(() =>
            {
                injectionWorkspace.UpdateDocument(notification.TextDocument.Uri, notification.TextDocument.Text);

                return Unit.Value;
            });
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions() => new TextDocumentRegistrationOptions()
        {
            DocumentSelector = _documentSelector,
        };

        public Task<Unit> Handle(DidCloseTextDocumentParams notification, CancellationToken token) => Unit.Task;

        public Task<Unit> Handle(DidSaveTextDocumentParams notification, CancellationToken token) => Unit.Task;

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions() => new TextDocumentSaveRegistrationOptions()
        {
            DocumentSelector = _documentSelector,
            IncludeText = true
        };
        public TextDocumentAttributes GetTextDocumentAttributes(Uri uri) => new TextDocumentAttributes(uri, "injection");
    }
}
