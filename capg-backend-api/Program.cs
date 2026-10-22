using capg_hv_backend.Application;
using capg_hv_backend.Application.Middlewares;
using capg_hv_backend.Application.Resiliency;
using capg_hv_backend.Endpoints;
using capg_hv_backend.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddResiliency()
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddEndpoints();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddleware();
app.UseInfrastructure();
app.UseHttpsRedirection();
app.UseEndpoints();
app.Run();