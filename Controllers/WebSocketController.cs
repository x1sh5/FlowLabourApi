using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace WebSocketsSample.Controllers;

// <snippet>
public class WebSocketController : ControllerBase
{
    private static List<WebSocket> _WebSockets = new List<WebSocket>();

    [HttpGet("/ws")]
    public async Task Get()
    {
        
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            if(!_WebSockets.Contains(webSocket))
            {
                _WebSockets.Add(webSocket);
                var message = System.Text.Encoding.Default.GetBytes("xxx连接成功");
                await webSocket.SendAsync(
                        new ArraySegment<byte>(message, 0, message.Length),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
            }
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    // </snippet>

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        Console.WriteLine(_WebSockets.Count);

        while (!receiveResult.CloseStatus.HasValue)
        {
            foreach(var c in _WebSockets)
            {
                if (c != webSocket &&(c.State==WebSocketState.Open|| c.State == WebSocketState.CloseReceived))
                {
                    await c.SendAsync(
                        new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);
                }

            }


            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}