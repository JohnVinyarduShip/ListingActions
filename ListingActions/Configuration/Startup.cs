using Newtonsoft.Json;
using StructureMap;

namespace ListingActions.Configuration
{
    public static class Startup
    {
        public static IContainer Configure()
        {
            var container = new Container(x => 
                x.For<IDatabase>().Use<Database>().Singleton());

            
            JsonConvert.DefaultSettings = () => 
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

            return container;
        }
    }
}
