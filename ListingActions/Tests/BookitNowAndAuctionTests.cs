using System;
using System.Linq;
using ListingActions.Contexts;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class BookitNowAndAuctionTests : BaseTests
    {
        protected override IPipeline<BiddingContext> BuildPipeline()
        {
            return new Pipeline<BiddingContext>(
                new PlaceBid(
                    new BidderIsIdVerified()
                ),
                new PutListingOnHold(
                    new BidAmountIsEqualTo(new Price(999m)),
                    new UserHasStarRating(5),
                    new ListingHasCreditCard()
                ),
                new AutoAcceptBid(
                    new All()
                )
            );
        }

        [Test]
        public void Can_get_pricing_requirements()
        {
            var req = _pipeline.SpecsAs<IPriceRestriction>().LastOrDefault();
            Console.WriteLine(req.ToJson());
            Assert.NotNull(req);
            Assert.AreEqual(new Price(999m), req.Min);
            Assert.AreEqual(new Price(999m), req.Max);
        }

        [Test]
        public void Non_id_verified_user_sees_no_permissions()
        {
            var bidder = BuildBidder();
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertNoSteps(steps);
        }

        [Test]
        public void Id_verified_user_sees_bid_permission()
        {
            var bidder = BuildBidder(idVerified: true);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(new[] { PLACE_BID }, steps);
        }

        [Test]
        public void Id_verified_user_can_place_bid()
        {
            var bidder = BuildBidder(idVerified: true);
            var context = BuildContext(bidder, 999m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(new[] { PLACE_BID }, steps);
            AssertBidStatusIs(BidStatus.Pending, bidder);
            AssertListingStatusIs(ListingStatus.Active);
        }

        [Test]
        public void Five_star_user_sees_auto_accept_permission()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 10);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
        }

        [Test]
        public void Five_star_user_can_book_listing()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 10);
            var context = BuildContext(bidder, 999m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
            AssertBidStatusIs(BidStatus.Accepted, bidder);
            AssertListingStatusIs(ListingStatus.Booked);
        }

        [Test]
        public void Five_star_user_bidding_at_wrong_amount_only_places_a_bid()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 10);
            var context = BuildContext(bidder, 9999m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(
                new[] { PLACE_BID }, steps);
            AssertBidStatusIs(BidStatus.Pending, bidder);
            AssertListingStatusIs(ListingStatus.Active);
        }
    }
}
