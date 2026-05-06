using capg_hv_backend.Application;
using capg_hv_backend.Application.Middlewares;
using capg_hv_backend.Infrastructure;
using Carter;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddCustomMiddleware()
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddCarter();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddleware();
app.UseInfrastructure();
app.UseHttpsRedirection();
app.MapCarter();
app.Run();