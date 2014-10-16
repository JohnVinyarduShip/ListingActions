using ListingActions.Contexts;
using ListingActions.Entities;
using ListingActions.Pipeline;
using ListingActions.Specs;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class OfferPriceAndBookItNowTests
    {
        private IPipeline<BiddingContext> _pipeline;
        private IDatabase _database;
        private User _shipper;
        private Listing _listing;

        [SetUp]
        public void SetUp()
        {
            _pipeline = new Pipeline<BiddingContext>(
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

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void Write_me()
        {
            Assert.Fail();
        }
    }
}
