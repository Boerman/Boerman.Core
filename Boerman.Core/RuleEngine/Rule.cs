using System.Linq.Expressions;

namespace Boerman.Core.RuleEngine
{
    public class Rule
    {
        public string ComparisonPredicate { get; set; }
        public ExpressionType ComparisonOperator { get; set; }
        public string ComparisonValue { get; set; }

        public Rule(string comparisonPredicate, ExpressionType comparisonOperator, string comparisonValue)
        {
            ComparisonPredicate = comparisonPredicate;
            ComparisonOperator  = comparisonOperator;
            ComparisonValue     = comparisonValue;
        }
    }
}
