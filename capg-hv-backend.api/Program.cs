using capg_hv_backend.Application.Persistence;
using capg_hv_backend.Application.Repositories;
using capg_hv_backend.InterfaceAdapters.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddPersistence(builder.Configuration)
    .AddRepositories();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePersistence();
app.UseHttpsRedirection();
app.Run();
