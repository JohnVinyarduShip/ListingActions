using ListingActions.Contexts;
using ListingActions.Specs;
using StructureMap;

namespace ListingActions.Pipeline
{
    public class PutListingOnHold : PipelineStepWithPreconditions<BiddingContext>
    {
        public PutListingOnHold(params ISpecification<BiddingContext>[] preconditions)
            : base(preconditions) { }

        protected override BiddingContext InnerExecute(BiddingContext context)
        {
            var db = ObjectFactory.GetInstance<IDatabase>();
            context.Listing.Status = ListingStatus.OnHold;
            db.Upsert(context.Listing);
            return context;
        }
    }
}