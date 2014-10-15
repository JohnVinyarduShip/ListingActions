namespace ListingActions
{
    public enum ListingStatus
    {
        /// <summary>
        /// The listing is active, and visible to the entire marketplace
        /// </summary>
        Active = 1,

        /// <summary>
        /// The listing has received a highly desrable bid, and has been
        /// removed from the marketplace until the shipper either accepts
        /// or rejects the pending bid
        /// </summary>
        OnHold = 2,

        /// <summary>
        /// The listing is permanently removed from the marketplace.  An acceptable
        /// bid has been placed and accepted.
        /// </summary>
        Booked = 3
    }

    public enum BidStatus
    {
        /// <summary>
        /// The bid is awaiting a response from the shipper
        /// </summary>
        Pending = 1,

        /// <summary>
        /// The bid has been accepted by the shipper
        /// </summary>
        Accepted = 2,

        /// <summary>
        /// The bid has been rejected by the shipper
        /// </summary>
        Rejected = 3
    }
}
