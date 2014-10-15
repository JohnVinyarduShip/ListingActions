using System;
using System.Collections.Generic;
using System.Linq;
using ListingActions.Configuration;
using ListingActions.Contexts;
using ListingActions.Entities;
using ListingActions.Pipeline;
using NUnit.Framework;
using StructureMap;

namespace ListingActions.Tests
{
    [TestFixture]
    public abstract class BaseTests
    {
        protected IPipeline<BiddingContext> _pipeline;
        protected IDatabase _database;
        protected User _shipper;
        protected Listing _listing;

        protected static readonly Type PLACE_BID = typeof (PlaceBid);
        protected static readonly Type PUT_ON_HOLD = typeof (PutListingOnHold);
        protected static readonly Type AUTO_ACCEPT = typeof (AutoAcceptBid);
        protected IContainer Container;
        [SetUp]
        public void SetUp()
        {
            Container = Startup.Configure();

            _database = Container.GetInstance<IDatabase>();
            _database.Clear();

            _pipeline = BuildPipeline();

            _shipper = new User
            {
                Name = "Shipper",
                EmailAddress = "shipper@hotmail.com"
            };
            _database.Upsert(_shipper);

            _listing = new Listing
            {
                OwnerId = _shipper.Id,
                PickupLocation = 10,
                PreAuthorizedCreditCardNumber = "xxxx",
                Status = ListingStatus.Active
            };
            _database.Upsert(_listing);
        }

        [TearDown]
        public void TearDown()
        {
            _database.Clear();
        }

        protected abstract IPipeline<BiddingContext> BuildPipeline();

        protected User BuildBidder(
            bool idVerified = false,
            double starRating = 0,
            int listingProximity = 1000,
            string emailAddress = null)
        {
            var user = new User
            {
                StarRating = starRating,
                CurrentLocation = _listing.PickupLocation + listingProximity,
                EmailAddress = emailAddress ?? "bidder@uship.com"
            };
            if (idVerified)
            {
                user.PhoneNumber = "123-456-7890";
                user.ShippingAddress = "123 Blah";
            }
            _database.Upsert(user);
            return user;
        }

        protected BiddingContext BuildContext(User bidder, decimal? amount = null)
        {
            return new BiddingContext
            {
                Bidder = bidder,
                Listing = _listing,
                Bid = amount.HasValue ? 
                    new Bid { Price = new Price(amount.Value) } : null
            };
        }

        protected void AssertListingStatusIs(ListingStatus status)
        {
            var l = _database.Get<Listing>(_listing.Id);
            Assert.NotNull(l);
            Assert.AreEqual(status, l.Status);
        }

        protected void AssertBidStatusIs(BidStatus status, User bidder)
        {
            var b = FetchBid(bidder);
            Assert.NotNull(b);
            Assert.AreEqual(status, b.Status);
        }

        protected void AssertNoBidWasPlaced(User bidder)
        {
            Assert.Null(FetchBid(bidder));
        }

        protected Bid FetchBid(User bidder)
        {
            return _database.FindAll<Bid>(x =>
                x.ListingId == _listing.Id && x.BidderId == bidder.Id)
                .FirstOrDefault();
        }

        protected void AssertNoSteps(IEnumerable<IPipelineStep<BiddingContext>> actual)
        {
            AssertStepsEqual(new Type[] {}, actual);
        }

        protected void AssertStepsEqual(
            IEnumerable<Type> expected, 
            IEnumerable<IPipelineStep<BiddingContext>> actual)
        {
            CollectionAssert.AreEqual(
                expected, 
                actual.Select(x => x.GetType()));
        }
    }
}
