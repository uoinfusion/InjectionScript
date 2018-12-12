using System;

namespace InjectionScript.Lsp
{
    public interface IInjectionWorkspace
    {
        void UpdateDocument(Uri uri, string content);

        bool TryGetDocument(Uri uri, out string content);
    }
}
