using System;

namespace ListingActions.Entities
{
    public class Listing : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public string PreAuthorizedCreditCardNumber { get; set; }
        public ListingStatus Status { get; set; }

        /// <summary>
        /// The listing's coordinate (we live in a 1D universe)
        /// </summary>
        public int PickupLocation { get; set; }
    }
}