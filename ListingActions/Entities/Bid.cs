using System;

namespace ListingActions.Entities
{
    public class Bid : BaseEntity
    {
        public Guid ListingId { get; set; }
        public Guid BidderId { get; set; }
        public Price Price { get; set; }
        public BidStatus Status { get; set; }
    }
}