using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InjectionScript.Lsp.Tests
{
    internal static class CompletionListTestExtensions
    {
        public static void ShouldContainLabel(this CompletionList list, params string[] expectedSuggestions)
        {
            foreach (var suggestion in expectedSuggestions)
            {
                if (!list.Any(x => x.Label.Equals(suggestion, StringComparison.OrdinalIgnoreCase)))
                {
                    Assert.Fail($"CompletionList doesn't contain expected completion labeled '{suggestion}'");
                }
            }
        }

        public static void ShouldNotContainLabel(this CompletionList list, params string[] expectedSuggestions)
        {
            foreach (var suggestion in expectedSuggestions)
            {
                if (list.Any(x => x.Label.Equals(suggestion, StringComparison.OrdinalIgnoreCase)))
                {
                    Assert.Fail($"CompletionList contains completion labeled '{suggestion}' that should not be int the list.");
                }
            }
        }
    }
}
