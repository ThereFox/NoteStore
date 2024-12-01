using Application.Interfaces.Stores;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Persistense.Common.Cassandra.Store;

namespace Persistense.Common.DI;

public static class DIRegister
{
    public static IServiceCollection AddCassandra(
        this IServiceCollection serviceCollection,
        string address,
        int port
        )
    {
        var cluster = Cluster
            .Builder()
            .AddContactPoint(address)
            .WithPort(port)
            .WithCompression(CompressionType.LZ4)
            .WithApplicationName("NoteApp")
            .Build();

        serviceCollection.AddSingleton<ICluster>(cluster);
        serviceCollection.AddScoped<ISession>(
            ex =>
            {
            var clusterService = ex.GetRequiredService<ICluster>();
            return clusterService.Connect();
        }
        );
        serviceCollection.AddScoped<Mapper>(
            ex =>
            {
                var connection = ex.GetRequiredService<ISession>();
                var mapper = new Mapper(connection);
                return mapper;
            }
        );
        
        serviceCollection.AddTransient<ICqlQueryAsyncClient>(
            ex => ex.GetRequiredService<Mapper>()
        );
        
        serviceCollection.AddTransient<ICqlWriteAsyncClient>(
            ex => ex.GetRequiredService<Mapper>()
        );

        return serviceCollection;
    }

    public static IServiceCollection AddNoteStores(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<INoteQueryStore, NoteQueryStore>();
        serviceCollection.AddScoped<INoteCommandStore, NoteCommandStore>();

        return serviceCollection;
    }
    
}