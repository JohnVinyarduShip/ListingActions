namespace ListingActions.Specs
{
    /// <summary>
    /// Everything satisfies this specification
    /// </summary>
    public class All : ISpecification<object>
    {
        public bool IsSatisfied(object context)
        {
            return true;
        }
    }
}