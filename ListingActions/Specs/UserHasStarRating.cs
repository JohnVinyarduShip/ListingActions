using System.Linq;
using ListingActions.Contexts;
using ListingActions.Services;

namespace ListingActions.Specs
{
    /// <summary>
    /// Ensure that a potential bidder has gte a specified star rating
    /// </summary>
    public class UserHasStarRating : ISpecification<IBidderContext>
    {
        public int MinimumStarRating { get; protected set; }

        public UserHasStarRating(IPipelineService pipelineService)
        {
            MinimumStarRating = pipelineService.GetSpecValues<UserHasStarRating>().FirstOrDefault().Value;
        }

        public bool IsSatisfied(IBidderContext context)
        {
            return context.Bidder.StarRating >= MinimumStarRating;
        }
    }
}