using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panta.Indexing.Correctors;

namespace Panta.Indexing.Expressions
{
    public static class SearchExpression
    {
        /// <summary>
        /// Parse the search query by building a logic expression tree of single terms
        /// </summary>
        /// <param name="query">Original search query</param>
        /// <param name="corrector">Correct single term into an expression</param>
        /// <returns>Expression tree (Join AndExpression for different terms and join OrExpression for corrections)</returns>
        public static IExpression Parse(string query, ITermCorrector corrector)
        {
            if (String.IsNullOrEmpty(query)) return new TermExpression(String.Empty);

            query = query.ToLowerInvariant();
            string[] pieces = query.Split(' ', '\t', '\n', '\r');

            IExpression result = null;

            foreach (string piece in pieces)
            {
                IExpression expr = null;

                // Scan the possibilities of AndNot and Or
                if (piece.Length > 1)
                {
                    if (piece.Contains('|'))
                    {
                        string[] orPieces = piece.Split('|');
                        foreach (string orPiece in orPieces)
                        {
                            expr = LogicOrExpression.Join(expr, Parse(orPiece, corrector));
                        }
                        result = LogicAndExpression.Join(result, expr);
                        continue;
                    }
                    else if (piece[0] == '-')
                    {
                        expr = ParseTerm(piece.Substring(1), corrector);
                        result = LogicAndNotExpression.Join(result, expr);
                        continue;
                    }
                }

                // By default, use And to connect
                expr = ParseTerm(piece, corrector);
                result = LogicAndExpression.Join(result, expr);
            }
            return result;
        }

        /// <summary>
        /// Parse a single piece of the entire search query into an Expression form using corrector
        /// </summary>
        /// <param name="term">A truncated piece contains no delimiters</param>
        /// <param name="corrector">Corrector to be used to add suggestions to the expression</param>
        /// <returns>IExpression contains the original term and the suggestions of the word</returns>
        private static IExpression ParseTerm(string term, ITermCorrector corrector)
        {
            IExpression result=null;
            if (term.Length >= 2)
            {
                result = new TermExpression(term);
            }
            if (corrector != null)
            {
                foreach (string correction in corrector.Correct(term))
                {
                    IExpression expr = new TermExpression(correction);
                    result = LogicOrExpression.Join(result, expr);
                }
            }
            return result;
        }

        public static IExpression ParseEachTermWithPrefix(string query, string prefix, ITermCorrector corrector)
        {
            IExpression result=null;
            string[] pieces = query.Split(' ', '\t', '\n', '\r');
            foreach (string piece in pieces)
            {
                // Make sure no already prefixed item to be add a new prefix
                if (piece.Split(':').Length == 1)
                {
                    if (piece.Length >= 2)
                    {
                        string newQuery = query + " " + prefix + piece;
                        IExpression part = Parse(newQuery, corrector);
                        result = LogicAndExpression.Join(result, part);
                    }
                }
                else
                {
                    result = LogicAndExpression.Join(result, new TermExpression(piece));
                }
            }
            return result;
        }
    }
}
