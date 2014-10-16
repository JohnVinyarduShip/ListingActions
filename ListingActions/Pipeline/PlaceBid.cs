using ListingActions.Contexts;
using ListingActions.Specs;
using StructureMap;

namespace ListingActions.Pipeline
{
    public class PlaceBid : PipelineStepWithPreconditions<BiddingContext>
    {
        public PlaceBid(params ISpecification<BiddingContext>[] preconditions)
            : base(preconditions) { }

        protected override BiddingContext InnerExecute(BiddingContext context)
        {
            var db = ObjectFactory.GetInstance<IDatabase>();
            context.Bid.ListingId = context.Listing.Id;
            context.Bid.BidderId = context.Bidder.Id;
            context.Bid.Status = BidStatus.Pending;
            db.Upsert(context.Bid);
            return context;
        }
    }
}