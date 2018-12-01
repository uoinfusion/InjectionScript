using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Linq;

namespace InjectionScript.Lsp.Server
{
    internal class DiagnosticReporter : IDiagnosticReporter
    {
        private readonly OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer router;

        public DiagnosticReporter(OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer router)
        {
            this.router = router;
        }

        public void Report(Uri documentUri, MessageCollection messages)
        {
            router.SendNotification(DocumentNames.PublishDiagnostics, new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<Diagnostic>(messages.Select(msg => new Diagnostic
                {
                    Code = msg.Code,
                    Message = msg.Text,
                    Severity = ConvertSeverity(msg.Severity),
                    Range = new Range(new Position(msg.StartLine - 1, msg.StartColumn), new Position(msg.EndLine - 1, msg.EndColumn))
                })).ToArray()
            });
        }

        private DiagnosticSeverity ConvertSeverity(MessageSeverity severity)
        {
            switch (severity)
            {
                case MessageSeverity.Error:
                    return DiagnosticSeverity.Error;
                case MessageSeverity.Warning:
                    return DiagnosticSeverity.Warning;
                default:
                    throw new NotImplementedException(severity.ToString());
            }
        }
    }
}