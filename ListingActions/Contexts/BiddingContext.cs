using ListingActions.Entities;

namespace ListingActions.Contexts
{
    /// <summary>
    /// Encapsulate information about the potential bidder
    /// </summary>
    public interface IBidderContext
    {
        User Bidder { get; }
    }

    /// <summary>
    /// Encapsulate information about the listing
    /// </summary>
    public interface IListingContext
    {
        Listing Listing { get; }
    }

    /// <summary>
    /// Encapsulate information about the proposed bid price
    /// </summary>
    public interface IPriceContext
    {
        Price Price { get; }
    }

    /// <summary>
    /// Encapsulate information about the proposed bid
    /// </summary>
    public interface IBidContext : IPriceContext
    {
        Bid Bid { get; }
    }

    /// <summary>
    /// Encapsulate all the context we have pre-bidding
    /// </summary>
    public interface IPreBidContext :
        IListingContext, IBidderContext { }

    /// <summary>
    /// Encapsulate all the information we need to evaluate a proposed bid
    /// </summary>
    public class BiddingContext :
        IPreBidContext,
        IBidContext
    {
        public User Bidder { get; set; }
        public Listing Listing { get; set; }
        public Bid Bid { get; set; }
        public Price Price
        {
            get
            {
                return Bid == null ? null : Bid.Price;
            }
        }
    }
}