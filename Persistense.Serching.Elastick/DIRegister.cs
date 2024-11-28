using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Persistense.Common.Cassandra.Interfaces;
using Persistense.Serching.Elastick.Stores;

namespace Persistense.Serching.Elastick;

public static class DIRegister
{
    public static IServiceCollection AddElastickSearch(this IServiceCollection services, string url, string indexName)
    {
        var uri = new Uri(url);

        services.AddScoped<ElasticsearchClient>(ex => new ElasticsearchClient(uri));
        services.AddScoped<IDocumentSearcher, DocumentsSearcher>(ex =>
        {
            var elasticClient = ex.GetRequiredService<ElasticsearchClient>();
            return new DocumentsSearcher(elasticClient, indexName);
        });
        services.AddScoped<IDocumentStore, DocumentStore>(
            ex =>
            {
                var elasticClient = ex.GetRequiredService<ElasticsearchClient>();
                return new DocumentStore(elasticClient, indexName);
            }
                );

        return services;
    }
}