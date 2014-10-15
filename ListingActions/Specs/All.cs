using ListingActions.Services;
using ListingActions.Specs.Interfaces;

namespace ListingActions.Specs
{
    /// <summary>
    /// Everything satisfies this specification
    /// </summary>
    public class All : ISpecification<object>, IPutListingOnHoldSpec
    {
        private readonly IPipelineService _pipelineService;

        public All(IPipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        public bool IsSatisfied(object context)
        {
            return true;
        }
    }
}