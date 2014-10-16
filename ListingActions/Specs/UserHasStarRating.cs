using ListingActions.Contexts;

namespace ListingActions.Specs
{
    /// <summary>
    /// Ensure that a potential bidder has gte a specified star rating
    /// </summary>
    public class UserHasStarRating : ISpecification<IBidderContext>
    {
        public int MinimumStarRating { get; protected set; }

        public UserHasStarRating(int minimumStarRating)
        {
            MinimumStarRating = minimumStarRating;
        }

        public bool IsSatisfied(IBidderContext context)
        {
            return context.Bidder.StarRating >= MinimumStarRating;
        }
    }
}