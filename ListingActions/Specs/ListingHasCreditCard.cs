using System;
using ListingActions.Contexts;

namespace ListingActions.Specs
{
    public class ListingHasCreditCard : ISpecification<IListingContext>
    {
        public bool IsSatisfied(IListingContext context)
        {
            return !String.IsNullOrEmpty(
                context.Listing.PreAuthorizedCreditCardNumber);
        }
    }
}