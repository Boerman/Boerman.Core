using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Boerman.Core.RuleEngine
{
    public class RuleEvaluator<T>
    {
        public RuleEvaluator()
        {
            
        }

        private ICollection<Func<T, bool>> Rules { get; set; }

        public void AddRule(Func<T, bool> rule)
        {
            Rules.Add(rule);
        }

        public void AddRule(Expression<Func<T, bool>> rule)
        {
            Rules.Add(rule.Compile());
        }
    }
}
