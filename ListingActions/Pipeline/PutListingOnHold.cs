using System.Collections.Generic;
using System.Linq;
using ListingActions.Contexts;
using ListingActions.Specs;
using ListingActions.Specs.Interfaces;
using StructureMap;

namespace ListingActions.Pipeline
{
    public class PutListingOnHold : PipelineStepWithPreconditions<BiddingContext>
    {
        private readonly IContainer _container;

        public PutListingOnHold(IContainer container,IEnumerable<IPutListingOnHoldSpec> preconditions) : base(preconditions.Cast<ISpecification<BiddingContext>>())
        {
            _container = container;
        }

        protected override BiddingContext InnerExecute(BiddingContext context)
        {
            var db = _container.GetInstance<IDatabase>();
            context.Listing.Status = ListingStatus.OnHold;
            db.Upsert(context.Listing);
            return context;
        }
    }
}