using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.WebSockets;


namespace chatserver
{
    public class MessageApi
    {
        public int Port = 8081;

        private HttpListener _listener;

        public void Start()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");
            _listener.Start();
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback_NoAuth), _listener);
        }

        private void ListenerCallback_NoAuth(IAsyncResult result)
        {
            if (_listener.IsListening)
            {
                Console.WriteLine("listening...");
                var context = _listener.EndGetContext(result);
                var request = context.Request;
                var response = context.Response;

                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                // Gestionar sol·licituds POST a /sendMessage
                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/sendMessage")
                {
                    if (request.HasEntityBody)
                    {
                        var body = request.InputStream;
                        var encoding = request.ContentEncoding;
                        var reader = new StreamReader(body, encoding);
                        if (request.ContentType != null)
                        {
                            Console.WriteLine("Client data content type {0}", request.ContentType);
                        }
                        Console.WriteLine("Client data content length {0}", request.ContentLength64);

                        Console.WriteLine("Start of data:");
                        string s = reader.ReadToEnd();
                        Console.WriteLine(s);
                        Console.WriteLine("End of data:");
                        reader.Close();
                        body.Close();
                    }
                }

                // Respondre amb un missatge OK
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                string responseBody = "Missatge rebut!";
                byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                Console.WriteLine($"{request.HttpMethod} {request.Url} - OK");

                Receive();
            }
        }

    }

}
