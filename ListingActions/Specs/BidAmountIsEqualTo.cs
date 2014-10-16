using ListingActions.Contexts;

namespace ListingActions.Specs
{
    public interface IPriceRestriction
    {
        Price Min { get; }
        Price Max { get; }
    }

    public class BidAmountIsEqualTo : ISpecification<IPriceContext>, IPriceRestriction
    {
        public BidAmountIsEqualTo(Price amount)
        {
            Min = amount;
            Max = amount;
        }

        public bool IsSatisfied(IPriceContext context)
        {
            return context.Price >= Min && context.Price <= Max;
        }

        public Price Min { get; private set; }
        public Price Max { get; private set; }
    }
}