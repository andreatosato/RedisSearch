using Redis.OM;

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

app.Run();
