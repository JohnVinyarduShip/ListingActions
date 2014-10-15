using System.Linq;
using ListingActions.Contexts;
using ListingActions.Services;
using ListingActions.Specs.Interfaces;

namespace ListingActions.Specs
{
    public interface IPriceRestriction
    {
        Price Min { get; }
        Price Max { get; }
    }

    public class BidAmountIsEqualTo : ISpecification<IPriceContext>, IPriceRestriction, IPlaceBidSpec
    {
        public BidAmountIsEqualTo(IPipelineService pipelineService)
        {
            Min = pipelineService.GetSpecValues<BidAmountIsEqualTo>().FirstOrDefault().Value;
            Max = pipelineService.GetSpecValues<BidAmountIsEqualTo>().FirstOrDefault().Value;
        }

        public bool IsSatisfied(IPriceContext context)
        {
            return context.Price >= Min && context.Price <= Max;
        }

        public Price Min { get; private set; }
        public Price Max { get; private set; }
    }
}