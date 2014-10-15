using System.Collections.Generic;
using System.Linq;
using ListingActions.Specs;

namespace ListingActions.Pipeline
{
    public interface IPipeline<TContext> where TContext : class
    {
        /// <summary>
        /// Execute the pipeline, and return a list of all successfully 
        /// executed steps, in order.
        /// </summary>
        /// <param name="context">All the data needed to execute the pipeline</param>
        /// <returns>A list of all successfully executed steps, in order</returns>
        IEnumerable<IPipelineStep<TContext>> Execute(TContext context);


        /// <summary>
        /// Return a report about reachable pipeline steps, given available data
        /// </summary>
        /// <param name="context"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<IPipelineStep<TContext>> ReachableSteps(
            TContext context, SpecQuery<TContext> query);


        /// <summary>
        /// Return all specs pertaining to a particular dimension of the context
        /// KLUDGE: It would probably make more sense to return a report that 
        /// is hierarchical, e.g.:
        /// 
        /// -Step 1
        /// -- Spec 1
        /// -- Spec 2
        /// -Step 2
        /// -- Spec 1
        /// etc...
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<T> SpecsAs<T>() where T : class;
    }

    public class Pipeline<TContext> : IPipeline<TContext> where TContext : class
    {
        private readonly IList<IPipelineStep<TContext>> _steps;

        public Pipeline(params IPipelineStep<TContext>[] steps)
        {
            _steps = new List<IPipelineStep<TContext>>(steps);
        }

        public IEnumerable<IPipelineStep<TContext>> Execute(TContext context)
        {
            var c = context;
            foreach (var step in _steps)
            {
                try
                {
                    c = step.Execute(c);
                }
                catch
                {
                    yield break;
                }
                yield return step;
            }
        }

        public IEnumerable<IPipelineStep<TContext>> ReachableSteps(
            TContext context, SpecQuery<TContext> query)
        {
            return _steps.TakeWhile(step => 
                !query.Evaluate(context, step.Specifications).Any());
        }

        public IEnumerable<T> SpecsAs<T>() where T : class
        {
            return _steps.SelectMany(step => step.Specifications.OfType<T>());
        }
    }
}
