using System.Net.WebSockets;
using WebSocketDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// �[�JWebSocket�B�z�A��
builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// WebSocket �Ѧ� https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/

// �[�J WebSocket �\��
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// ı�o�� Controller ���� WebSocket �ӽ����A��b Middleware �h�B�z
// �i�H�Ѧ� https://stackoverflow.com/questions/67768461/how-to-connect-to-websocket-through-controller
// �Pı���ӦA�z�L�@�Ӥ����h�B�z�A�Ϊ̤��|�Q swagger �Y�쪺 controller �A���� swagger ���ӴN�O���F restfull �ӥ�
app.Use(async (context, next) =>
{
    // ������o�Ӹ��|�N�O for WebSocket
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
            {
                var wsHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                await wsHandler.ProcessWebSocket(ws);
            }
        }
        else
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
    }
    else await next();
});

// ���ε{���Ұ�
app.Run();
