using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TCPServer
{
    public class ServerJson
    {
        public int port = 80;

        public void Start()
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Server started on port " + port);

            while (true)
            {
                TcpClient socket = server.AcceptTcpClient();
                IPEndPoint remoteIpEndPoint = socket.Client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("Connection from " + remoteIpEndPoint.Address.ToString());

                Task.Run(() => HandleClient(socket));
            }
        }

        public void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            while (socket.Connected)
            {
                string? jsonString = reader.ReadLine();

                try
                {
                    JsonRequest kommandoObject = JsonSerializer.Deserialize<JsonRequest>(jsonString);

                    string request = kommandoObject.RequestMethod;

                    if (request == "Stop")
                    {
                        socket.Close();
                        break;
                    }

                    if (request == "RandomNr" || request == "Add" || request == "Subtract")
                    {
                        int number1 = kommandoObject.Number1;
                        int number2 = kommandoObject.Number2;

                        string result = "";

                        switch (request)
                        {
                            case "RandomNr":
                                Random rand = new Random();
                                result = rand.Next(number1, number2 + 1).ToString();
                                break;
                            case "Add":
                                result = (number1 + number2).ToString();
                                break;
                            case "Subtract":
                                result = (number1 - number2).ToString();
                                break;
                        }

                        Console.WriteLine($"Command: {request} Result: {result}");

                        writer.WriteLine(result);
                        writer.Flush();
                    }
                    else
                    {
                        writer.WriteLine("Invalid command");
                        writer.Flush();
                    }
                }
                catch (JsonException ex)
                {
                    writer.WriteLine(ex.Message);
                    writer.Flush();
                }
            }
        }
    }
}


