using System;
using ListingActions.Contexts;
using ListingActions.Services;

namespace ListingActions.Specs
{
    public class ListingHasCreditCard : ISpecification<IListingContext>
    {
        private readonly IPipelineService _pipelineService;

        public ListingHasCreditCard(IPipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        public bool IsSatisfied(IListingContext context)
        {
            return !String.IsNullOrEmpty(
                context.Listing.PreAuthorizedCreditCardNumber);
        }
    }
}