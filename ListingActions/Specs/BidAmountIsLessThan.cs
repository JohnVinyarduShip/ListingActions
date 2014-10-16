using ListingActions.Contexts;

namespace ListingActions.Specs
{
    public class BidAmountIsLessThan : ISpecification<IPriceContext>, IPriceRestriction
    {
        public Price MaxPrice { get; protected set; }

        public BidAmountIsLessThan(Price maxPrice)
        {
            Min = new Price(0);
            Max = maxPrice;
        }

        public bool IsSatisfied(IPriceContext context)
        {
            return context.Price >= Min && context.Price <= Max;
        }

        public Price Min { get; private set; }
        public Price Max { get; private set; }
    }
}
