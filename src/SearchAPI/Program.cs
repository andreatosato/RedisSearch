using Redis.OM;
using SearchAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RedisConnectionProvider>((sp) => 
    new RedisConnectionProvider("redis://redis:6379"));

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
    var init = new BootstrapData(s.ServiceProvider.GetRequiredService<RedisConnectionProvider>());
    await init.BootstrapAsync(100);
}

app.Run();
