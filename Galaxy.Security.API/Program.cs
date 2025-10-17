using Galaxy.Security.Infraestructure;
using Galaxy.Security.Application;
using Galaxy.Security.Infraestructure.Configurations.IdentitySeed;
using Galaxy.Security.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfraestructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentityDataSeeder.SeedAsync(services);
}

app.Run();
