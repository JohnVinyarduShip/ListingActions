using System.Linq;
using ListingActions.Contexts;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class BookItNowOnlyTests : BaseTests
    {
        protected override IPipeline<BiddingContext> BuildPipeline()
        {
            return new Pipeline<BiddingContext>(
                new PlaceBid(
                    new BidderIsIdVerified(),
                    new UserHasStarRating(5),
                    new BidAmountIsEqualTo(new Price(999m)),
                    new ListingHasCreditCard()
                ),
                new PutListingOnHold(
                    new All()
                ),
                new AutoAcceptBid(
                    new All()
                )
            );
        }

        [Test]
        public void Can_fetch_pricing_requirements()
        {
            var req = _pipeline.SpecsAs<IPriceRestriction>().FirstOrDefault();
            Assert.NotNull(req);
            Assert.AreEqual(new Price(999m), req.Max);
            Assert.AreEqual(new Price(999m), req.Min);
        }

        [Test]
        public void Non_id_verified_bidder_sees_no_bid_permission()
        {
            var bidder = BuildBidder();
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertNoSteps(steps);
        }

        [Test]
        public void Id_verified_bidder_with_low_star_rating_cannot_place_bid()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 1);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertNoSteps(steps);
        }

        [Test]
        public void Five_star_bidder_sees_auto_accept_permission()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 5.5);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
        }

        [Test]
        public void Five_star_bidder_who_bids_at_wrong_price_fails_to_place_bid()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 5.5);
            var context = BuildContext(bidder, 997m);
            var steps = _pipeline.Execute(context).ToList();
            AssertNoSteps(steps);
            AssertListingStatusIs(ListingStatus.Active);
            AssertNoBidWasPlaced(bidder);
        }

        [Test]
        public void Five_star_bidder_who_bids_at_correct_price_books_listing()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 5.5);
            var context = BuildContext(bidder, 999m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
            AssertListingStatusIs(ListingStatus.Booked);
            AssertBidStatusIs(BidStatus.Accepted, bidder);
        }
    }
}
