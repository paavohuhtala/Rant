﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using Rant.Stringes;

namespace Rant.Vocabulary
{
    internal static class DicLexer
    {
        private const RegexOptions DicRegexOptions = RegexOptions.Compiled;

        private static readonly LexerRules<DicTokenType> Rules;

        static DicLexer()
        {
            Rules = new LexerRules<DicTokenType>
            {
                {new Regex(@"\#\s*(?<value>.*?)[\s\r]*(?=\#|\||\>{1,2}|\@|\^>|$)", DicRegexOptions), DicTokenType.Directive, 2},
                {new Regex(@"\|\s*(?<value>.*?)[\s\r]*(?=\#|\||\>{1,2}|\@|\^>|$)", DicRegexOptions), DicTokenType.Property, 2},
                {new Regex(@"\>\s*(?<value>.*?)[\s\r]*(?=\#|\||\>{1,2}|\@|\^>|$)", DicRegexOptions), DicTokenType.Entry, 2},
                {new Regex(@">>\s*(?<value>.*?)[\s\r]*(?=\#|\||\>{1,2}|\@|\^>|$)", DicRegexOptions), DicTokenType.DiffEntry, 2},
                {new Regex(@"\^>\s*(?<value>.*?)[\s\r]*(?=\#|\||\>{1,2}|\@|\^>|$)", DicRegexOptions), DicTokenType.RefEntry, 2},
                {new Regex(@"\@.*?$", DicRegexOptions | RegexOptions.Multiline), DicTokenType.Ignore, 2},
                {new Regex(@"\s+"), DicTokenType.Ignore}
            };
            Rules.AddEndToken(DicTokenType.EOF);
            Rules.IgnoreRules.Add(DicTokenType.Ignore);
        }

        public static IEnumerable<Token<DicTokenType>> Tokenize(string data)
        {
            Token<DicTokenType> token;
            var reader = new StringeReader(data);
            while ((token = reader.ReadToken(Rules)).ID != DicTokenType.EOF)
            {
                yield return token;
            }
        }
    }

    internal enum DicTokenType
    {
        Directive,
        Entry,
        DiffEntry,
        RefEntry,
        Property,
        Ignore,
        EOF
    }
}