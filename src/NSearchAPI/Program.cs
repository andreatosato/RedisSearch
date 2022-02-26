using NRediSearch;
using NSearchAPI;
using NSearchAPI.Models;
using SearchAPI;
using StackExchange.Redis;
using static NRediSearch.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddSingleton<SearchClients>((sp) =>
{
    var con = sp.GetRequiredService<IConnectionMultiplexer>();
    var db = con.GetDatabase();
    var exists = db.KeyExists($"idx:{nameof(Customer)}");
    var customerClient = new Client(nameof(Customer), db);
    var loremClient = new Client("Lorem", db);
    try
    {
        customerClient.CreateIndex(Customer.Schema(), new ConfiguredIndexOptions());
    }
    catch { }
    try
    {
        var schema = new Schema();
        schema.AddSortableTextField("Text");
        loremClient.CreateIndex(schema, new ConfiguredIndexOptions());
    }
    catch { }
    return new SearchClients
    {
        CustomeClient = customerClient,
        LoremClient = loremClient
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using var s = app.Services.CreateScope();
{
    var init = new BootstrapData(s.ServiceProvider.GetRequiredService<SearchClients>().CustomeClient);
    await init.BootstrapAsync(100);
}
app.Run();
