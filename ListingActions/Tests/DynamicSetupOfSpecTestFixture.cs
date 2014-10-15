using System.Linq;
using ListingActions.Contexts;
using ListingActions.Pipeline;
using ListingActions.Services;
using ListingActions.Specs;
using ListingActions.Specs.Interfaces;
using NUnit.Framework;
using StructureMap;

namespace ListingActions.Tests
{
    [TestFixture]
    public class DynamicSetupOfSpecTestFixture
    {
        private IContainer _container;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _container = new Container(x =>
            {
                x.For(typeof(IPipeline<>)).Use(typeof(Pipeline<>));
                x.For<IPipelineService>().Use<PipelineService>();

                x.Scan(s =>
                {
                    s.AssemblyContainingType<BiddingContext>();
                    s.AddAllTypesOf<IAutoAcceptBidSpec>();
                    s.AddAllTypesOf<IPlaceBidSpec>();
                    s.AddAllTypesOf<IPutListingOnHoldSpec>();
                    s.AddAllTypesOf<PipelineStepWithPreconditions<BiddingContext>>();
                });
            });
        }


        [Test]
        public void can_get_place_bid_specs()
        {
            var specs = _container.GetAllInstances<IPlaceBidSpec>();

            Assert.That(specs, Is.Not.Null);
            Assert.That(specs.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void can_get_a_pipeline()
        {
            var context = _container.GetInstance<IPipeline<BiddingContext>>();

            Assert.That(context, Is.Not.Null);
        }

        [Test]
        public void Can_query_for_pricing_requirements()
        {
            var req = _container.GetInstance<IPipeline<BiddingContext>>().SpecsAs<IPriceRestriction>().LastOrDefault();
            Assert.NotNull(req);
            Assert.AreEqual(new Price(101m), req.Min);
            Assert.AreEqual(new Price(101m), req.Max);
        }

    }
}