using NRediSearch;
using NSearchAPI.Models;
using SearchAPI;
using StackExchange.Redis;
using static NRediSearch.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddSingleton<Client>((sp) =>
{
    var con = sp.GetRequiredService<IConnectionMultiplexer>();
    var db = con.GetDatabase();
    var exists = db.KeyExists($"idx:{nameof(Customer)}");
    var client = new Client(nameof(Customer), db);
    if(client.DropIndex())
        client.CreateIndex(Customer.Schema(), new ConfiguredIndexOptions());
    return client;
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
    var init = new BootstrapData(s.ServiceProvider.GetRequiredService<Client>());
    await init.BootstrapAsync(100);
}
app.Run();
