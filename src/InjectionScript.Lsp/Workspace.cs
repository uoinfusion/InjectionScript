using System;
using System.Collections.Generic;

namespace InjectionScript.Lsp
{
    public class InjectionWorkspace : IInjectionWorkspace
    {
        private readonly Dictionary<string, string> documents = new Dictionary<string, string>();

        public void UpdateDocument(Uri uri, string content) => documents[uri.AbsoluteUri] = content;

        public bool TryGetDocument(Uri uri, out string content)
            => documents.TryGetValue(uri.AbsoluteUri, out content);
    }

    public interface IInjectionWorkspace
    {
        void UpdateDocument(Uri uri, string content);

        bool TryGetDocument(Uri uri, out string content);
    }
}
