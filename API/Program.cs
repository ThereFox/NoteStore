using Application;
using Persistense.Common.DI;
using Persistense.Serching.Elastick;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers();

builder
    .Services
    .AddApplicationServices()
    .AddCassandra("localhost", 9042)
    .AddNoteStores()
    .AddElastickSearch("http://127.0.0.1:9200", "default_index_for_app");

var app = builder.Build();

app.MapControllers();

app.Run();