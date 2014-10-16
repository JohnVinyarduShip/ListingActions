using System.Linq;
using ListingActions.Contexts;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class OfferPriceTests : BaseTests
    {
        protected override IPipeline<BiddingContext> BuildPipeline()
        {
            return new Pipeline<BiddingContext>(

                new PlaceBid(
                    new BidderIsIdVerified(),
                    new BidAmountIsEqualTo(new Price(101m))),

                new PutListingOnHold(
                    new All()),
                new AutoAcceptBid(
                    new None())
            );
        }

        [Test]
        public void Can_query_for_pricing_requirements()
        {
            var req = _pipeline.SpecsAs<IPriceRestriction>().LastOrDefault();
            Assert.NotNull(req);
            Assert.AreEqual(new Price(101m),req.Min);
            Assert.AreEqual(new Price(101m), req.Max);
        }

        [Test]
        public void Non_id_verified_users_see_no_permissions()
        {
            var bidder = BuildBidder();
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context,
                SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertNoSteps(steps);
        }

        [Test]
        public void Id_verified_users_see_on_hold_permissions()
        {
            var bidder = BuildBidder(idVerified: true);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context,
                SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD }, steps);
        }

        [Test]
        public void Id_verified_users_may_put_the_listing_on_hold()
        {
            var bidder = BuildBidder(idVerified: true);
            var context = BuildContext(bidder, 101m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD }, steps);
            AssertListingStatusIs(ListingStatus.OnHold);
            AssertBidStatusIs(BidStatus.Pending, bidder);
        }

        [Test]
        public void Bid_at_wrong_price_is_not_placed()
        {
            var bidder = BuildBidder(idVerified: true);
            var context = BuildContext(bidder, 201m);
            var steps = _pipeline.Execute(context).ToList();
            AssertNoSteps(steps);
            AssertListingStatusIs(ListingStatus.Active);
            AssertNoBidWasPlaced(bidder);
        }
    }
}
