using System;
using System.Linq;
using ListingActions.Contexts;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class PrivateNetworkTests : BaseTests
    {
        protected override IPipeline<BiddingContext> BuildPipeline()
        {
            return new Pipeline<BiddingContext>(
                new PlaceBid(
                    new BidderIsInShippersNetwork()
                ),
                new PutListingOnHold(
                    new BidAmountIsEqualTo(new Price(3456m))
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
            Assert.NotNull(req);
            Assert.AreEqual(new Price(3456m),req.Max);
            Assert.AreEqual(new Price(3456m), req.Min);
        }

        [Test]
        public void User_not_in_network_sees_no_permissions()
        {
            var bidder = BuildBidder(emailAddress: "bidder@uship.com");
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertNoSteps(steps);
        }

        [Test]
        public void User_in_network_sees_auto_accept_permission()
        {
            var bidder = BuildBidder(emailAddress: "bidder@hotmail.com");
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
        }

        [Test]
        public void User_placing_bid_at_named_price_books_the_listing()
        {
            var bidder = BuildBidder(emailAddress: "bidder@hotmail.com");
            var context = BuildContext(bidder, 3456m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
            AssertBidStatusIs(BidStatus.Accepted, bidder);
            AssertListingStatusIs(ListingStatus.Booked);
        }

        [Test]
        public void User_placing_bid_above_the_named_price_does_not_book_the_listing()
        {
            var bidder = BuildBidder(emailAddress: "bidder@hotmail.com");
            var context = BuildContext(bidder, 4456m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(new[] { PLACE_BID }, steps);
            AssertBidStatusIs(BidStatus.Pending, bidder);
            AssertListingStatusIs(ListingStatus.Active);
        }

    }
}
