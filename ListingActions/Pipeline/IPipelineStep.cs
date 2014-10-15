using System;
using System.Collections.Generic;
using System.Linq;
using ListingActions.Specs;

namespace ListingActions.Pipeline
{
    public interface IPipelineStep<TContext> where TContext : class
    {
        /// <summary>
        /// Execute each step in the pipeline, until we reach a step where
        /// one or more preconditions are not met
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        TContext Execute(TContext context);

        ///// <summary>
        ///// Return all the preconditions for this step
        ///// </summary>
        IEnumerable<ISpecification<TContext>> Specifications { get; }
    }

    public abstract class PipelineStepWithPreconditions<TContext>
        : IPipelineStep<TContext> where TContext : class
    {
        private readonly IList<ISpecification<TContext>> _preconditions;

        protected PipelineStepWithPreconditions(IEnumerable<ISpecification<TContext>> preconditions)
        {
            _preconditions = preconditions.ToList();
        }

        public TContext Execute(TContext context)
        {
            if (!_preconditions.All(x => x.IsSatisfied(context)))
            {
                throw new Exception("precondition(s) failed");
            }
            return InnerExecute(context);
        }

        public IEnumerable<ISpecification<TContext>> Specifications
        {
            get { return _preconditions; }
        }

        protected abstract TContext InnerExecute(TContext context);
    }
}
