using System.Linq;
using ListingActions.Contexts;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class OfferPriceAndBookItNowTests : BaseTests
    {
        protected override IPipeline<BiddingContext> BuildPipeline()
        {
            return new Pipeline<BiddingContext>(
                new PlaceBid(
                    new BidderIsIdVerified(),
                    new BidAmountIsEqualTo(new Price(456m))
                    ),
                new PutListingOnHold(
                    new All()
                    ),
                new AutoAcceptBid(
                    new ListingHasCreditCard(),
                    new UserHasStarRating(5)
                    )
                );
        }

        [Test]
        public void Can_fetch_pricing_requirements()
        {
            var req = _pipeline.SpecsAs<IPriceRestriction>().FirstOrDefault();
            Assert.NotNull(req);
            Assert.AreEqual(new Price(456m), req.Min);
            Assert.AreEqual(new Price(456m), req.Max);
        }

        [Test]
        public void Non_id_verified_user_sees_no_bid_step()
        {
            var bidder = BuildBidder();
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertNoSteps(steps);
        }

        [Test]
        public void Id_verified_user_sees_on_hold_step()
        {
            var bidder = BuildBidder(idVerified: true);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(new[] { PLACE_BID, PUT_ON_HOLD }, steps);
        }

        [Test]
        public void Five_star_user_sees_auto_accept_step()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 5.1);
            var context = BuildContext(bidder);
            var steps = _pipeline.ReachableSteps(
                context, SpecQuery<BiddingContext>.WithDimension<IPreBidContext>())
                .ToList();
            AssertStepsEqual(
                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
        }

        [Test]
        public void Bid_at_the_wrong_price_is_not_placed()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 5.1);
            var context = BuildContext(bidder, 500m);
            var steps = _pipeline.Execute(context).ToList();
            AssertNoSteps(steps);
            AssertListingStatusIs(ListingStatus.Active);
            AssertNoBidWasPlaced(bidder);
        }

        [Test]
        public void Bid_at_the_right_price_by_four_star_user_puts_listing_on_hold()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 4.1);
            var context = BuildContext(bidder, 456m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(new[] { PLACE_BID, PUT_ON_HOLD }, steps);
            AssertListingStatusIs(ListingStatus.OnHold);
            AssertBidStatusIs(BidStatus.Pending, bidder);
        }

        [Test]
        public void Bid_at_the_right_price_by_five_star_user_books_listing()
        {
            var bidder = BuildBidder(idVerified: true, starRating: 5.1);
            var context = BuildContext(bidder, 456m);
            var steps = _pipeline.Execute(context).ToList();
            AssertStepsEqual(new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
            AssertListingStatusIs(ListingStatus.Booked);
            AssertBidStatusIs(BidStatus.Accepted, bidder);
        }
    }
}
