using System;
using ListingActions.Contexts;

namespace ListingActions.Specs
{
/// <summary>
/// Ensure that a potential bidder is within a specified range of the listing's
/// pickup coordinate
/// </summary>
public class ProximityToPickup : ISpecification<IPreBidContext>
{
    public int MaxDistance { get; protected set; }

    public ProximityToPickup(int maxDistance)
    {
        MaxDistance = maxDistance;
    }

    public bool IsSatisfied(IPreBidContext context)
    {
        var bidder = context.Bidder;
        var listing = context.Listing;
        return Math.Abs(bidder.CurrentLocation - listing.PickupLocation) <= MaxDistance;
    }
}
}