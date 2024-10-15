using System.Net.WebSockets;
using System.Net;
using System.Text;
using System;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace chatserver
{
    class Program
    {
        // Especifica l'adreça IP i el port on el servidor WebSocket escoltarà
        private static readonly string serverAddress = "http://localhost:5000/";

        static async Task Main(string[] args)
        {
            // Crear un HttpListener per rebre sol·licituds HTTP i WebSocket
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(serverAddress);
            httpListener.Start();
            Console.WriteLine("Servidor WebSocket en execució a: " + serverAddress);

            // Bucle infinit per acceptar connexions
            while (true)
            {
                // Acceptar sol·licitud de connexió
                HttpListenerContext context = await httpListener.GetContextAsync();

                // Comprovar si la sol·licitud és una sol·licitud WebSocket
                if (context.Request.IsWebSocketRequest)
                {
                    // Gestionar connexió WebSocket
                    await HandleWebSocketConnectionAsync(context);
                }
                else
                {
                    // Retornar una resposta HTTP 400 si no és una sol·licitud WebSocket
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        // Funció per gestionar la connexió WebSocket
        private static async Task HandleWebSocketConnectionAsync(HttpListenerContext context)
        {
            // Acceptar la sol·licitud WebSocket i obtenir el WebSocket
            HttpListenerWebSocketContext wsContext = null;
            try
            {
                wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                Console.WriteLine("Connexió WebSocket acceptada.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en acceptar connexió WebSocket: " + e.Message);
                context.Response.StatusCode = 500;
                context.Response.Close();
                return;
            }

            // Obtenir el WebSocket actiu
            WebSocket webSocket = wsContext.WebSocket;

            // Bucle per rebre missatges del client
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    // Esperar missatge del client
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    // Si la connexió és tancada pel client
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("El client ha tancat la connexió.");
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connexió tancada pel client", CancellationToken.None);
                    }
                    // Si es rep un missatge de text
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Llegir i convertir el missatge rebut
                        string clientMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Missatge rebut del client: " + clientMessage);

                        // Enviar resposta al client
                        string responseMessage = clientMessage;
                        byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Netejar el buffer per preparar-lo per al següent missatge
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine("Error en la comunicació WebSocket: " + e.Message);
                    break;
                }
            }

            // Tancar el WebSocket si es surt del bucle
            if (webSocket.State != WebSocketState.Closed)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connexió finalitzada", CancellationToken.None);
            }

            webSocket.Dispose();
        }

    }
}