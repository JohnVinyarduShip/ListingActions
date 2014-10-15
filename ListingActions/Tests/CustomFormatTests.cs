//using System.Linq;
//using ListingActions.Contexts;
//using ListingActions.Pipeline;
//using ListingActions.Specs;
//using NUnit.Framework;
//
//namespace ListingActions.Tests
//{
//    [TestFixture]
//    public class CustomFormatTests : BaseTests
//    {
//        protected override IPipeline<BiddingContext> BuildPipeline()
//        {
//            return new Pipeline<BiddingContext>(
//                new PlaceBid(
//                    new BidderIsIdVerified()
//                ),
//                new PutListingOnHold(
//                    new BidAmountIsEqualTo(new Price(100m)),
//                    new UserHasStarRating(4),
//                    new ProximityToPickup(50)
//                ),
//                new AutoAcceptBid(
//                    new UserHasStarRating(5),
//                    new ListingHasCreditCard()
//                )
//            );
//        }
//
//        [Test]
//        public void Can_query_for_pricing_requirements()
//        {
//            var req = _pipeline.SpecsAs<IPriceRestriction>().LastOrDefault();
//            Assert.NotNull(req);
//            Assert.AreEqual(new Price(100m),req.Min);
//            Assert.AreEqual(new Price(100m), req.Max);
//        }
//
//        [Test]
//        public void Non_id_verified_user_does_not_see_bid_permission()
//        {
//            var user = BuildBidder();
//            var context = BuildContext(user);
//            var permissions = _pipeline.ReachableSteps(
//                context,
//                SpecQuery<BiddingContext>
//                    .WithDimension<IPreBidContext>())
//                .ToList();
//            AssertNoSteps(permissions);
//        }
//
//        [Test]
//        public void Non_id_verified_user_cannot_place_bid()
//        {
//            var user = BuildBidder();
//            var context = BuildContext(user, 100m);
//            var steps = _pipeline.Execute(context).ToList();
//            AssertNoSteps(steps);
//        }
//
//        [Test]
//        public void Id_verified_user_sees_bid_permission()
//        {
//            var user = BuildBidder(idVerified: true);
//            var context = BuildContext(user);
//            var steps = _pipeline
//                .ReachableSteps(
//                    context,
//                    SpecQuery<BiddingContext>
//                        .WithDimension<IPreBidContext>())
//                .ToList();
//            AssertStepsEqual(new[] { PLACE_BID }, steps);
//        }
//
//        [Test]
//        public void Id_verified_user_can_place_bid()
//        {
//            var user = BuildBidder(idVerified: true);
//            var context = BuildContext(user, 200m);
//            var steps = _pipeline.Execute(context).ToList();
//            AssertStepsEqual(new[] { PLACE_BID }, steps);
//            AssertBidStatusIs(BidStatus.Pending, user);
//            AssertListingStatusIs(ListingStatus.Active);
//        }
//
//        [Test]
//        public void User_who_meets_all_on_hold_criteria_sees_on_hold_permission()
//        {
//            var user = BuildBidder(
//                idVerified: true,
//                starRating: 4.5,
//                listingProximity: 10);
//            var context = BuildContext(user);
//            var steps = _pipeline
//                .ReachableSteps(
//                    context,
//                    SpecQuery<BiddingContext>
//                        .WithDimension<IPreBidContext>())
//                .ToList();
//            AssertStepsEqual(
//                new[] { PLACE_BID, PUT_ON_HOLD }, steps);
//        }
//
//        [Test]
//        public void User_who_is_too_far_away_does_not_see_on_hold_permission()
//        {
//            var user = BuildBidder(
//                idVerified: true,
//                starRating: 4.5,
//                listingProximity: 190);
//            var context = BuildContext(user);
//            var steps = _pipeline
//                .ReachableSteps(
//                    context,
//                    SpecQuery<BiddingContext>
//                        .WithDimension<IPreBidContext>())
//                .ToList();
//            AssertStepsEqual(new[] { PLACE_BID }, steps);
//        }
//
//        [Test]
//        public void User_who_meets_all_on_hold_criteria_but_does_not_bid_at_the_offer_price_does_not_put_listing_on_hold()
//        {
//            var user = BuildBidder(
//                idVerified: true,
//                starRating: 4.5,
//                listingProximity: 10);
//            var context = BuildContext(user, 150m);
//            var steps = _pipeline.Execute(context).ToList();
//            AssertStepsEqual(new[] { typeof(PlaceBid) }, steps);
//            AssertBidStatusIs(BidStatus.Pending, user);
//            AssertListingStatusIs(ListingStatus.Active);
//        }
//
//        [Test]
//        public void User_who_meets_all_on_hold_criteria_may_put_listing_on_hold()
//        {
//            var user = BuildBidder(
//                idVerified: true,
//                starRating: 4.5,
//                listingProximity: 10);
//            var context = BuildContext(user, 100m);
//            var steps = _pipeline.Execute(context).ToList();
//            AssertStepsEqual(
//                new[] { PLACE_BID, PUT_ON_HOLD },
//                steps);
//            AssertBidStatusIs(BidStatus.Pending, user);
//            AssertListingStatusIs(ListingStatus.OnHold);
//        }
//
//        [Test]
//        public void User_who_meets_auto_accept_criteria_sees_auto_accept_permission()
//        {
//            var user = BuildBidder(
//                idVerified: true,
//                starRating: 5.5,
//                listingProximity: 10);
//
//            var context = BuildContext(user);
//
//            var steps = _pipeline.ReachableSteps(
//                context,
//                SpecQuery<BiddingContext>
//                    .WithDimension<IPreBidContext>())
//                .ToList();
//
//            AssertStepsEqual(
//                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
//        }
//
//        [Test]
//        public void User_who_meets_auto_accept_criteria_can_instantly_book()
//        {
//            var user = BuildBidder(
//                idVerified: true,
//                starRating: 5.5,
//                listingProximity: 10);
//
//            var context = BuildContext(user, 100m);
//
//            var steps = _pipeline.Execute(context).ToList();
//
//            AssertStepsEqual(
//                new[] { PLACE_BID, PUT_ON_HOLD, AUTO_ACCEPT }, steps);
//
//            AssertBidStatusIs(BidStatus.Accepted, user);
//            AssertListingStatusIs(ListingStatus.Booked);
//        }
//    }
//}