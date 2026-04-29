using capg_hv_backend.Application.Helpers;
using capg_hv_backend.Application.Persistence;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Application.Validators;
using capg_hv_backend.InterfaceAdapters.Middleware;
using Carter;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddPersistence(builder.Configuration)
    .AddRepositories()
    .AddValidators()
    .AddHelpers(builder.Configuration)
    .AddCarter();

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UsePersistence();
app.UseHttpsRedirection();
app.MapCarter();
app.Run();
