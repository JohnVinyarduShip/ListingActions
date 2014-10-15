using ListingActions.Contexts;
using ListingActions.Entities;
using ListingActions.Pipeline;
using NUnit.Framework;

namespace ListingActions.Tests
{
    [TestFixture]
    public class BookItNowOnlyTests
    {

        private IPipeline<BiddingContext> _pipeline;
        private IDatabase _database;
        private User _shipper;
        private Listing _listing;

        [SetUp]
        public void SetUp()
        {
                
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
