using ListingActions.Contexts;
using ListingActions.Specs;
using StructureMap;

namespace ListingActions.Pipeline
{
    public class AutoAcceptBid : PipelineStepWithPreconditions<BiddingContext>
    {
        public AutoAcceptBid(params ISpecification<BiddingContext>[] preconditions)
            : base(preconditions) { }

        protected override BiddingContext InnerExecute(BiddingContext context)
        {
            var db = ObjectFactory.GetInstance<IDatabase>();
            context.Listing.Status = ListingStatus.Booked;
            context.Bid.Status = BidStatus.Accepted;
            db.Upsert(context.Listing);
            db.Upsert(context.Bid);
            return context;
        }
    }
}