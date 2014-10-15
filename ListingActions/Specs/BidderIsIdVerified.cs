using System;
using ListingActions.Contexts;
using ListingActions.Services;
using ListingActions.Specs.Interfaces;

namespace ListingActions.Specs
{
    public class BidderIsIdVerified : ISpecification<IBidderContext>, IPlaceBidSpec
    {
        private readonly IPipelineService _pipelineService;

        public BidderIsIdVerified(IPipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        public bool IsSatisfied(IBidderContext context)
        {
            return
                !String.IsNullOrEmpty(context.Bidder.EmailAddress)
                && !String.IsNullOrEmpty(context.Bidder.PhoneNumber)
                && !String.IsNullOrEmpty(context.Bidder.ShippingAddress);
        }
    }
}