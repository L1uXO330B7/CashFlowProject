using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
                    // ���� SwaggerDocument �� URL ��m�C
                    name: "v1",
                    // �O�Ω� SwaggerDocument ������T����� ( ���e�D���� )�C
                    info: new OpenApiInfo
                    {
                    }
    );
});

builder.Services.AddMiniProfiler(o =>
{
    o.RouteBasePath = "/profiler";
})
.AddEntityFramework();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMiniProfiler();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(
                // �ݰt�X SwaggerDoc �� name�C "/swagger/{SwaggerDoc name}/swagger.json"
                url: "/swagger/v1/swagger.json",
                // �� Swagger UI �k�W����ܤ��P������ SwaggerDocument ��ܦW�٨ϥΡC
                name: "RESTful API v1.0.0"
            );

            c.IndexStream = () => typeof(Program).GetTypeInfo()
                                                  .Assembly
                                                  .GetManifestResourceStream("MiniProfilerSwagger.index.html");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
