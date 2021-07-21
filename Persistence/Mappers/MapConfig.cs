using Mapster;

namespace Persistence.Mappers
{
    public static class MapConfig
    {
        public static void Configure()
        {
            ProductMapper.Configure();
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true; 
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);

            TypeAdapterConfig.GlobalSettings.Compile();
        }
    }
}