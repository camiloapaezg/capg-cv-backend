using capg_hv_backend.Application.Persistence;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.Application.Validators;
using capg_hv_backend.InterfaceAdapters.Middleware;
using Carter;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddPersistence(builder.Configuration)
    .AddRepositories()
    .AddValidators()
    .AddCarter();

var app = builder.Build();
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
