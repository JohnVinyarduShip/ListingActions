using System.Collections.Generic;
using System.Linq;
using ListingActions.Contexts;
using ListingActions.Specs;
using ListingActions.Specs.Interfaces;
using StructureMap;

namespace ListingActions.Pipeline
{
    public class AutoAcceptBid : PipelineStepWithPreconditions<BiddingContext>
    {
        private readonly IContainer _container;

        public AutoAcceptBid(IContainer container,IEnumerable<IAutoAcceptBidSpec> preconditions) : base(preconditions.Cast<ISpecification<BiddingContext>>())
        {
            _container = container;
        }

        protected override BiddingContext InnerExecute(BiddingContext context)
        {
            var db = _container.GetInstance<IDatabase>();
            context.Listing.Status = ListingStatus.Booked;
            context.Bid.Status = BidStatus.Accepted;
            db.Upsert(context.Listing);
            db.Upsert(context.Bid);
            return context;
        }
    }
}