using System;

namespace ListingActions
{
    public class Price : IEquatable<Price>, IComparable<Price>
    {
        public Price(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; protected set; } 

        public bool Equals(Price other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Amount == other.Amount;
        }

        public int CompareTo(Price other)
        {
            return Amount.CompareTo(other.Amount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Price) obj);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }

        public static bool operator ==(Price left, Price right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Price left, Price right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(Price left, Price right)
        {
            return left.Amount < right.Amount;
        }

        public static bool operator >(Price left, Price right)
        {
            return left.Amount > right.Amount;
        }

        public static bool operator <=(Price left, Price right)
        {
            return left.Amount <= right.Amount;
        }

        public static bool operator >=(Price left, Price right)
        {
            return left.Amount >= right.Amount;
        }
    }
}