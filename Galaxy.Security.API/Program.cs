using Galaxy.Security.Infraestructure;
using Galaxy.Security.Application;
using Galaxy.Security.Infraestructure.Configurations.IdentitySeed;
using Galaxy.Security.API.Middlewares;
using Galaxy.Security.Infraestructure.Configurations.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfraestructure(builder.Configuration);
builder.Services.AddJwtWithCookies(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",policy =>
    {
        policy.WithOrigins("https://localhost:7178")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); //COOKIES
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseRefreshTokenMiddleware();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();



// Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentityDataSeeder.SeedAsync(services);
}

app.Run();
