using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MMX4.WebAPI.Helper
{
    public class SocketHandler
    {
        public const int BufferSize = 4096;

        WebSocket socket;

        SocketHandler(WebSocket socket)
        {
            this.socket = socket;
        }
        

        //private async void LightController_ReceiveData(object sender, Type type)
        //{
        //    if (this.socket.State == WebSocketState.Open)
        //    {
        //        var bytes = System.Text.Encoding.ASCII.GetBytes((typeof(LightSensorRequest).Equals(type) ? "1" : "0") + Newtonsoft.Json.JsonConvert.SerializeObject(sender));
        //        await this.socket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        //    }
        //}

        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (this.socket.State == WebSocketState.Open)
            {
                try
                {
                    var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                    var outgoing = new ArraySegment<byte>(buffer, 0, incoming.Count);
                    await this.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch { }
            }
        }

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new SocketHandler(socket);
            await h.EchoLoop();
        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
    }
}
