namespace ListingActions.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public double StarRating { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }

        /// <summary>
        /// The user's most recent coordinate (we live in a 1D universe)
        /// </summary>
        public int CurrentLocation { get; set; }
    }
}