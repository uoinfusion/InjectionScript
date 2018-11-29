using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using InjectionScript.Parsing.Syntax;
using InjectionScript.Runtime;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectionScript.Lsp
{
    public class Completer
    {
        private static readonly CompletionItem[] statementKeywords = new[]
        {
            new CompletionItem() { Label = "if", InsertText = "if", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "else", InsertText = "else", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "end if", InsertText = "end if", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "repeat", InsertText = "repeat", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "until", InsertText = "until", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "for", InsertText = "for", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "next", InsertText = "next", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "while", InsertText = "while", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "wend", InsertText = "wend", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "return", InsertText = "return", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "goto", InsertText = "goto", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "dim", InsertText = "dim", Kind = CompletionItemKind.Keyword },
            new CompletionItem() { Label = "var", InsertText = "var", Kind = CompletionItemKind.Keyword },
        };

        private bool IsEmpty(ParserRuleContext syntax)
        {
            if (syntax == null)
                return true;

            if (syntax is injectionParser.ArgumentListContext argumentList && argumentList.ChildCount <= 1)
                return true;
            

            return false;
        }

        private bool CanContainStatementKeyword(ParserRuleContext syntax)
            => syntax == null || (!(syntax is injectionParser.OperandContext) && !(syntax is injectionParser.ArgumentListContext));

        public CompletionList GetCompletions(injectionParser.FileContext fileSyntax, Metadata metadata, int line, int column)
        {
            var lineSyntax = FindLine(fileSyntax, line, column);

            var subrutineSuggestions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var namespaceSuggestions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (IsEmpty(lineSyntax))
            {
                foreach (var subrutine in metadata.Subrutines)
                {
                    subrutineSuggestions.Add(subrutine.Name);
                }

                foreach (var subrutine in metadata.NativeSubrutines)
                {
                    if (!subrutine.Name.StartsWith("UO.", StringComparison.OrdinalIgnoreCase))
                        subrutineSuggestions.Add(subrutine.Name);
                }

                namespaceSuggestions.Add("UO");

                var suggestions = subrutineSuggestions.Select(x => CreateSubrutineCompletion(x))
                    .Concat(namespaceSuggestions.Select(x => CreateNamespaceCompletion(x)));

                if (CanContainStatementKeyword(lineSyntax))
                    suggestions = suggestions.Concat(statementKeywords);

                return new CompletionList(suggestions);
            }

            var prefix = lineSyntax.GetText().Trim();
            bool isUONamespace = prefix.StartsWith("UO.", StringComparison.OrdinalIgnoreCase);

            foreach (var subrutine in metadata.NativeSubrutines)
            {
                if (!isUONamespace && subrutine.Name.StartsWith("UO."))
                    continue;

                if (subrutine.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    if (isUONamespace)
                        subrutineSuggestions.Add(subrutine.Name.Substring(3));
                    else
                        subrutineSuggestions.Add(subrutine.Name);
                }
            }

            if ("UO".StartsWith(prefix))
                namespaceSuggestions.Add("UO");

            foreach (var subrutine in metadata.Subrutines)
            {
                if (subrutine.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    subrutineSuggestions.Add(subrutine.Name);
                }
            }

            var completions = subrutineSuggestions.Select(x => CreateSubrutineCompletion(x))
                    .Concat(namespaceSuggestions.Select(x => CreateNamespaceCompletion(x)));

            if (!prefix.StartsWith("UO.", StringComparison.OrdinalIgnoreCase) && CanContainStatementKeyword(lineSyntax))
                completions = completions.Concat(statementKeywords);

            return new CompletionList(completions);
        }

        private CompletionItem CreateSubrutineCompletion(string subrutineName)
            => new CompletionItem() { Label = subrutineName, InsertText = subrutineName, Kind = CompletionItemKind.Function };
        private CompletionItem CreateNamespaceCompletion(string ns)
            => new CompletionItem() { Label = ns, InsertText = ns, Kind = CompletionItemKind.Class };

        public CompletionList GetCompletions(string fileContent, int line, int column)
        {
            var runtime = new InjectionRuntime();
            runtime.Load(fileContent, "test.sc");

            return GetCompletions(runtime.CurrentFileSyntax, runtime.Metadata, line, column);
        }

        private ParserRuleContext FindLine(IParseTree syntax, int line, int column)
        {
            for (var i = 0; i < syntax.ChildCount; i++)
            {
                var child = syntax.GetChild(i);
                var foundChild = FindLine(child, line, column);
                if (foundChild != null)
                    return foundChild;

                switch (child)
                {
                    case injectionParser.ArgumentListContext argumentList:
                        if (line == argumentList.Start.Line && line == argumentList.Stop.Line)
                            return argumentList;
                        break;
                    case injectionParser.StatementContext statement:
                        if (line == statement.Start.Line && line == statement.Stop.Line)
                            return statement;
                        break;
                    case Antlr4.Runtime.ParserRuleContext rule:
                        if (line == rule.Start.Line && line == rule.Stop.Line && column >= rule.Start.Column && column <= rule.Stop.Column)
                        {
                            return rule;
                        }
                        break;
                }
            }

            return null;
        }
    }
}
