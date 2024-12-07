using Cassandra.Mapping;
using CSharpFunctionalExtensions;

namespace Persistense.Common.DI.SchemeInitialiser;

public class CassandraSchemeInitialiser
{
    private readonly ICqlWriteAsyncClient _client;

    private const string CassandraKeyspaceName = "noteStore";
    private const string NoteTableName = "notes";
    
    public CassandraSchemeInitialiser(ICqlWriteAsyncClient client)
    {
        _client = client;
    }

    public async Task<Result> InitializeAsync()
    {
        try
        {
            await CreateKeyspace();
            await SwitchKeyspace();
            await CreateNoteTable();
            await CreateIndexes();
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
        
    }

    private async Task CreateKeyspace()
    {
        var cql = new Cql(
            @$"
                CREATE KEYSPACE IF NOT EXISTS {CassandraKeyspaceName}
                WITH replication = {"{'class'"} : 'SimpleStrategy', 
                    'replication_factor' : 1 {"}"}
                AND durable_writes = true;
            "
            );
        
        await _client.ExecuteAsync(cql);
    }
    private async Task SwitchKeyspace()
    {
        var cql = new Cql(
            @$"
                USE {CassandraKeyspaceName};
            "
        );
        
        await _client.ExecuteAsync(cql);
    }
    private async Task CreateNoteTable()
    {
        var cql = new Cql(
            $@"
                CREATE TABLE IF NOT EXISTS {NoteTableName}
                (
                Id Uuid,
                PartitionId int,
                Version int,
                ContentId Uuid,
                CreatorId Uuid,
                CreatorName text,
                Header text,
                PRIMARY KEY(PartitionId, Id, Version)
                )
                WITH Clustering ORDER BY Id ASC, Version DESC;
            "
            );

        await _client.ExecuteAsync(cql);
    }

    private async Task CreateIndexes()
    {
        var cql = new Cql(
            @$"
            CREATE INDEX IF NOT EXISTS IdIndex
            ON {NoteTableName} (Id);
            "
        );
        
        await _client.ExecuteAsync(cql);
    }
}