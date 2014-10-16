using ListingActions.Contexts;
using ListingActions.Entities;
using StructureMap;

namespace ListingActions.Specs
{
    public class BidderIsInShippersNetwork : ISpecification<IPreBidContext>
    {
        public bool IsSatisfied(IPreBidContext context)
        {
            var db = ObjectFactory.GetInstance<IDatabase>();
            var shipper = db.Get<User>(context.Listing.OwnerId);
            return Domain(shipper.EmailAddress) == Domain(context.Bidder.EmailAddress);
        }

        private static string Domain(string email)
        {
            var index = email.IndexOf("@");
            return email.Substring(index);
        }
    }
}