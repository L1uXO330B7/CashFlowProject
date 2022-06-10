using BaseCommon;
using Microsoft.AspNetCore.Mvc;

namespace WebSocketDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerHandler
    {
        private readonly ILogger<WebSocketController> _logger;

        /// <summary>
        /// ���o WebSocket �s�u�ƶq
        /// </summary>
        /// <returns></returns>
        [Route("GetConnectionsCount")]
        [HttpGet]
        public int Get()
        {
            return ConnectionsCount;
        }
    }
}