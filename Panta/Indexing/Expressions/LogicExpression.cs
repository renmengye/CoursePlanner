using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing.Expressions
{
    public abstract class LogicExpression : IExpression
    {
        protected IExpression Left { get; set; }
        protected IExpression Right { get; set; }
        public IEnumerable<string> Terms
        {
            get
            {
                return Left.Terms.Concat<string>(Right.Terms);
            }
        }

        public LogicExpression(IExpression left, IExpression right)
        {
            this.Left = left;
            this.Right = right;
        }

        public abstract HashSet<uint> Evaluate(IIdProvider provider);

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
