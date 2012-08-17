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
            string[] pieces = query.Split(' ', '\t', '\n', '\r');

            IExpression result = null;

            foreach (string piece in pieces)
            {
                IExpression expr = corrector.Correct(piece);
                result = LogicAndExpression.Join(result, expr);
            }

            return result;
        }
    }
}
