using System.Linq;
using ListingActions.Contexts;
using ListingActions.Entities;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class AuctionTests : BaseTests
    {
        protected override IPipeline<BiddingContext> BuildPipeline()
        {
            return new Pipeline<BiddingContext>(
                new PlaceBid(new All()),
                new PutListingOnHold(new None()),
                new AutoAcceptBid(new None())
            );
        }

        [Test]
        public void No_pricing_restrictions()
        {
            var req = _pipeline.SpecsAs<IPriceRestriction>().LastOrDefault();
            Assert.Null(req);
        }

        [Test]
        public void All_users_see_bid_permission_only()
        {
            var bidder = BuildBidder();
            var context = BuildContext(bidder);

            var steps = _pipeline.ReachableSteps(
                context,
                SpecQuery<BiddingContext>
                    .WithDimension<IPreBidContext>())
                .ToList();

            Assert.AreEqual(1, steps.Count);
            Assert.AreEqual(typeof(PlaceBid), steps[0].GetType());
        }

        [Test]
        public void All_users_may_place_bid()
        {
            var bidder = BuildBidder();
            var context = BuildContext(bidder, 50m);

            var steps = _pipeline.Execute(context).ToList();

            Assert.AreEqual(1, steps.Count);
            Assert.AreEqual(typeof(PlaceBid), steps[0].GetType());

            var bid = _database.FindAll<Bid>(
                x => x.BidderId == bidder.Id
                    && x.ListingId == _listing.Id).FirstOrDefault();

            Assert.NotNull(bid);
            Assert.AreEqual(BidStatus.Pending, bid.Status);
        }
    }
}
