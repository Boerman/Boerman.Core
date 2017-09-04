/*
 * ToDo: Allow `IEnumerable` object as target parameter
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Boerman.Core.RuleEngine
{
    public static class RuleEngine
    {
        public static ICollection<Func<T, bool>> CompileRule<T>(ICollection<T> target, ICollection<Rule> rules)
        {
            var compiledRules = new Collection<Func<T, bool>>();

            foreach (var rule in rules)
            {
                var genericType      = Expression.Parameter(typeof(T));
                var key              = Expression.Property(genericType, rule.ComparisonPredicate);
                var propertyType     = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;
                var value            = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
                var binaryExpression = Expression.MakeBinary(rule.ComparisonOperator, key, value);

                compiledRules.Add(Expression.Lambda<Func<T, bool>>(binaryExpression, genericType).Compile());
            }
            
            return compiledRules;
        }

        public static bool EvaluateRules<T>(ICollection<T> target, ICollection<Rule> rules)
        {
            var compiledRules = CompileRule(target, rules);

            foreach (var rule in compiledRules)
            {
                return target.Any(rule);
            }

            return true;
        }
    }
}
