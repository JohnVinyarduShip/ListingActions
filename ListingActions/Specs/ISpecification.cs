namespace ListingActions.Specs
{
    public interface ISpecification<in T>
    {
        bool IsSatisfied(T context);
    }
}
