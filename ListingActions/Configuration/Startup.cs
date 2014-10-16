using Newtonsoft.Json;
using StructureMap;

namespace ListingActions.Configuration
{
    public static class Startup
    {
        public static void Configure()
        {
            ObjectFactory.Initialize(x => 
                x.For<IDatabase>().Use<Database>().Singleton());

            JsonConvert.DefaultSettings = () => 
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
        }
    }
}
