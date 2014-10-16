namespace ListingActions.Specs
{
    /// <summary>
    /// Nothing satisfies this specification
    /// </summary>
    public class None : ISpecification<object>
    {
        public bool IsSatisfied(object context)
        {
            return false;
        }
    }
}