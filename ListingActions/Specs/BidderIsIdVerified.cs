using System;
using ListingActions.Contexts;

namespace ListingActions.Specs
{
    public class BidderIsIdVerified : ISpecification<IBidderContext>
    {
        public bool IsSatisfied(IBidderContext context)
        {
            return
                !String.IsNullOrEmpty(context.Bidder.EmailAddress)
                && !String.IsNullOrEmpty(context.Bidder.PhoneNumber)
                && !String.IsNullOrEmpty(context.Bidder.ShippingAddress);
        }
    }
}