using System.Linq;
using ListingActions.Contexts;
using ListingActions.Services;

namespace ListingActions.Specs
{
    public class BidAmountIsLessThan : ISpecification<IPriceContext>, IPriceRestriction
    {
        public Price MaxPrice { get; protected set; }

        public BidAmountIsLessThan(IPipelineService pipelineService)
        {
            Min = new Price(0);
            Max = pipelineService.GetSpecValues<BidAmountIsLessThan>().FirstOrDefault().Value;
        }

        public bool IsSatisfied(IPriceContext context)
        {
            return context.Price >= Min && context.Price <= Max;
        }

        public Price Min { get; private set; }
        public Price Max { get; private set; }
    }
}
