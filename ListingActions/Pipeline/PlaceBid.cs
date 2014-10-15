using System.Collections.Generic;
using System.Linq;
using ListingActions.Contexts;
using ListingActions.Specs;
using ListingActions.Specs.Interfaces;
using StructureMap;

namespace ListingActions.Pipeline
{
    public class PlaceBid : PipelineStepWithPreconditions<BiddingContext>
    {
        private readonly IContainer _container;


        public PlaceBid(IContainer container, IEnumerable<IPlaceBidSpec> preconditions) : base(preconditions.Cast<ISpecification<BiddingContext>>())
        {
            _container = container;
        }

        protected override BiddingContext InnerExecute(BiddingContext context)
        {
            var db = _container.GetInstance<IDatabase>();
            context.Bid.ListingId = context.Listing.Id;
            context.Bid.BidderId = context.Bidder.Id;
            context.Bid.Status = BidStatus.Pending;
            db.Upsert(context.Bid);
            return context;
        }
    }
}