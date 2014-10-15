using System.Collections.Generic;
using ListingActions.Specs;

namespace ListingActions.Services
{
    public interface IPipelineService
    {
        IDictionary<string, dynamic> GetSpecValues<T>();
        IEnumerable<string> GetSpecsToExecute();
    }

    public class PipelineService : IPipelineService
    {
        public IDictionary<string, dynamic> GetSpecValues<TSpecType>()
        {
            if (typeof(TSpecType) == typeof(BidAmountIsEqualTo))
            {
                return new Dictionary<string, dynamic>
                {
                    {"Min",new Price(101)}
                };
            }

            return new Dictionary<string, dynamic>();
        }

        public IEnumerable<string> GetSpecsToExecute()
        {
            return new List<string>();
        }
    }
}