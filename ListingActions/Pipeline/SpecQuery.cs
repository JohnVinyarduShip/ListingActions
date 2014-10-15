using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ListingActions.Specs;

namespace ListingActions.Pipeline
{
    /// <summary>
    /// Create a query for different aspects of <see cref="TContext"/>
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class SpecQuery<TContext>
    {
        private readonly Expression<Func<ISpecification<TContext>, bool>> _expression;

        private SpecQuery(
            Expression<Func<ISpecification<TContext>, bool>> expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// Begin a query with aspect <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SpecQuery<TContext> WithDimension<T>()
        {
            return new SpecQuery<TContext>(Expr<T>());
        }

        /// <summary>
        /// Add aspect <see cref="T"/> to an existing query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SpecQuery<TContext> AndDimension<T>()
        {
            var body = Expression.OrElse(
                _expression.Body,
                Expression.Invoke(Expr<T>(), _expression.Parameters[0]));

            return new SpecQuery<TContext>(
                Expression.Lambda<Func<ISpecification<TContext>, 
                bool>>(body, _expression.Parameters));
        }

        /// <summary>
        /// Filter specifications based on this query
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        public IEnumerable<ISpecification<TContext>> Filter(
            IEnumerable<ISpecification<TContext>> specs)
        {
            return specs
                .Where(_expression.Compile());
        }

        /// <summary>
        /// Evaluate only the aspects of <see cref="TContext"/> specified by
        /// this query
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="specs"></param>
        /// <returns></returns>
        public IEnumerable<ISpecification<TContext>> Evaluate(
            TContext subject,
            IEnumerable<ISpecification<TContext>> specs)
        {
            return Filter(specs)
                .Where(x => !x.IsSatisfied(subject));
        } 

        private static Expression<Func<ISpecification<TContext>, bool>> Expr<T>()
        {
            return x => (x as ISpecification<T>) != null;
        }
    }
}