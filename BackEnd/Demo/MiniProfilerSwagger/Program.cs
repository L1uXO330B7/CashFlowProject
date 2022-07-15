using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MiniProfilerSwagger.EF;
using MiniProfilerSwagger.Filter;
using StackExchange.Profiling.Storage;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// ���Udb
var ConnectionString = builder.Configuration.GetConnectionString("MiniProfilerDb");
builder.Services.AddDbContext<MiniProfilerDbContext>(options =>
       options.UseSqlServer(ConnectionString));

// AOP �z�ﾹ
builder.Services.AddMvc(config =>
{
    config.Filters.Add(new MiniProfilerActionFilter());
});

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

// �`�JMiniProfiler
builder.Services.AddMiniProfiler(o =>
{
    // �X�ݦa�}���Ѯڥؿ��F�w�]���G/mini-profiler-resources
    o.RouteBasePath = "/profiler";
    // ��Ƨ֨��ɶ�
    (o.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);
    // sql �榡�Ƴ]�w
    o.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
    // ���ܳs�u�}������
    o.TrackConnectionOpenClose = true;
    // �����D�D�C����;�w�]�L��
    o.ColorScheme = StackExchange.Profiling.ColorScheme.Dark;
    // .net core 3.0�H�W�G��MVC�L�o���i����R
    //o.EnableMvcFilterProfiling = true;
    // ���˵��i����R
    //o.EnableMvcViewProfiling = true;

    // ����X�ݭ������v�A�w�]�Ҧ��H����X��
    //o.ResultsAuthorize;
    // �n������R���ǽШD�A�w�]�����ШD�����R
    //o.ShouldProfile;

    // �������`�B�z
    o.OnInternalError = e => Console.WriteLine(e);
})
// �ʱ� EntityFrameworkCore �ͦ��� SQL
.AddEntityFramework();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // �Ӥ�k�����bapp.UseEndpoints�H�e
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
