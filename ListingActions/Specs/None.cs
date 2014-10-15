using ListingActions.Services;
using ListingActions.Specs.Interfaces;

namespace ListingActions.Specs
{
    /// <summary>
    /// Nothing satisfies this specification
    /// </summary>
    public class None : ISpecification<object>, IAutoAcceptBidSpec
    {
        private readonly IPipelineService _pipelineService;

        public None(IPipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        public bool IsSatisfied(object context)
        {
            return false;
        }
    }
}