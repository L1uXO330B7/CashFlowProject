using API.Filter;
using BLL.IServices;
using BLL.Services.AdminSide;
using Common.Model;
using DPL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 註冊 AOP Filters
builder.Services.AddMvc(config =>
{
    config.Filters.Add(new ExceptionFilter());
    config.Filters.Add(new MiniProfilerActionFilter());
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 自動產生 Swagger Xml 設定 https://www.tpisoftware.com/tpu/articleDetails/1372
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
                    // 攸關 SwaggerDocument 的 URL 位置。
                    name: "v1",
                    // 是用於 SwaggerDocument 版本資訊的顯示 ( 內容非必填 )。
                    info: new OpenApiInfo
                    {
                    }
    );

    var FilePath = Path.Combine(AppContext.BaseDirectory, "API.xml");
    c.IncludeXmlComments(FilePath);
});

// 註冊 DbContext
builder.Services.AddDbContext<CashFlowDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineCashFlow")));

// 註冊 Services
builder.Services.AddScoped<IUsersService<CreateUserArgs, int?, UpdateUserArgs, int?>, UsersService>();

// 註冊 MiniProfiler 如果更改設定需產生 MiniProfiler Script 貼於於 Swagger Index 內
builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler";
    options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
    options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
    options.EnableServerTimingHeader = true;
})
.AddEntityFramework(); // 監控 EntityFrameworkCore 生成的 SQL

var app = builder.Build();

// 該方法必須在 app.UseEndpoints 以前
app.UseMiniProfiler();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment()) // 開發者模式
// {
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        // 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
        url: "/swagger/v1/swagger.json",
        // 於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
        name: "RESTful API v1.0.0"
    );

    c.IndexStream = () => typeof(Program).GetTypeInfo()
                                          .Assembly
                                          .GetManifestResourceStream("API.index.html");
});
// }

app.UseAuthorization();

app.MapControllers();

app.Run();
