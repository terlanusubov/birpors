using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Birpors.API.Filters;
using Birpors.API.Sockets;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Birpors.API.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly WebSocketHandler _webSocketHandler;

        public WebSocketController(IApplicationDbContext context, WebSocketHandler webSocketHandler)
        {
            _context = context;
            _webSocketHandler = webSocketHandler;
        }

        [HttpGet("/ws")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<string>>> Get()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return ApiResult<string>.Error(
                    new Dictionary<string, string> { { "xeta", "xeta" } },
                    (int)HttpStatusCode.BadRequest,
                    "xeta"
                );
            }

            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketHandler.OnConnected(socket, userId.ToString());

            await Receive(
                socket,
                async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var msg = _webSocketHandler.ReceiveString(result, buffer);

                        await HandleMessage(socket, msg);

                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await HandleDisconnect(socket);
                        return;
                    }
                }
            );

            return ApiResult<string>.OK("success");
        }

        private async Task HandleDisconnect(WebSocket socket)
        {
            string disconnectedUser = await _webSocketHandler.OnDisconnected(socket);

            ServerMessage disconnectMessage = new ServerMessage(
                disconnectedUser,
                true,
                _webSocketHandler.GetAllUsers()
            );

            //await _webSocketHandler.BroadcastMessage(JsonSerializer.Serialize(disconnectMessage));
        }

        private async Task HandleMessage(WebSocket socket, string message)
        {
            ClientMessage clientMessage = TryDeserializeClientMessage(message);

            if (clientMessage == null)
            {
                return;
            }

            if (clientMessage.IsTypeConnection())
            {
                // For future improvements
            }
            else if (clientMessage.IsTypeChat())
            {
                await _webSocketHandler.SendMessageAsync(clientMessage.Receiver, message);
            }
        }

        private ClientMessage TryDeserializeClientMessage(string str)
        {
            try
            {
                return JsonSerializer.Deserialize<ClientMessage>(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: invalid message format");
                return null;
            }
        }

        private async Task Receive(
            WebSocket socket,
            Action<WebSocketReceiveResult, byte[]> handleMessage
        )
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(
                        buffer: new ArraySegment<byte>(buffer),
                        cancellationToken: CancellationToken.None
                    );

                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await HandleDisconnect(socket);
            }
        }
    }
}
