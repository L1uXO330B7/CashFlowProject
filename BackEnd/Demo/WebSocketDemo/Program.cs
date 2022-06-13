using System.Net.WebSockets;
using WebSocketDemo;

var Builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
Builder.Services.AddEndpointsApiExplorer();
Builder.Services.AddSwaggerGen();
// �[�JWebSocket�B�z�A��
Builder.Services.AddSingleton<WebSocketHandler>();

Builder.Services.AddSignalR();

var App = Builder.Build();
// �����h�Ѧ� https://blog.darkthread.net/blog/aspnetcore-middleware-lab/

// Configure the HTTP request pipeline.
if (App.Environment.IsDevelopment())
{
    App.UseSwagger();
    App.UseSwaggerUI();
}

App.UseAuthorization();

App.MapControllers();

//�s�� SignalR �����ѻP�t�諸 Hub
App.MapHub<SignalRHub>("/SignalRHub"); 

// WebSocket �Ѧ� https://blog.darkthread.net/blog/aspnet-core-websocket-chatroom/

// �[�J WebSocket �\��
App.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// ı�o�� Controller ���� WebSocket �ӽ����A��b Middleware �h�B�z
// �i�H�Ѧ� https://stackoverflow.com/questions/67768461/how-to-connect-to-websocket-through-controller
// �Pı���ӦA�z�L�@�Ӥ����h�B�z�A�Ϊ̤��|�Q swagger �Y�쪺 controller �A���� swagger ���ӴN�O���F restfull �ӥ�
App.Use(async (context, next) =>
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
App.Run();
