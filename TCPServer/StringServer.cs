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
    public class StringServer
    {
        public int port = 81;

        public void Start()
        {
            TcpListener stringServer = new TcpListener(System.Net.IPAddress.Any, port);
            stringServer.Start();
            Console.WriteLine("Server started on port " + port);

            while (true)
            {
                TcpClient socket = stringServer.AcceptTcpClient();
                IPEndPoint remoteIpEndPoint = socket.Client.RemoteEndPoint as System.Net.IPEndPoint;
                Console.WriteLine("Connection from " + remoteIpEndPoint.Address.ToString());

                Task.Run(() => HandleClient(socket));
            }
            stringServer.Stop();
        }

        public void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            while (socket.Connected)
            {
                string? method = reader.ReadLine();

                if (method == null)
                {
                    // Close socket if null is received
                    socket.Close();
                    break;
                }

                if (method == "Stop")
                {
                    socket.Close();
                    break;
                }

                if (method == "RandomNr" || method == "Add" || method == "Subtract")
                {
                    // Ask for input numbers
                    writer.WriteLine("Input request: ");
                    writer.Flush();

                    // Read the numbers from client
                    string numbersString = reader.ReadLine();
                    if (numbersString == null) continue;

                    string[] numbers = numbersString.Split(" ");
                    if (numbers.Length != 2)
                    {
                        writer.WriteLine("Invalid number of arguments.");
                        writer.Flush();
                        continue;
                    }

                    // Parse numbers
                    if (!int.TryParse(numbers[0], out int number1) || !int.TryParse(numbers[1], out int number2))
                    {
                        writer.WriteLine("Invalid number format.");
                        writer.Flush();
                        continue;
                    }

                    // Perform the requested operation
                    string result = method switch
                    {
                        "RandomNr" => new Random().Next(number1, number2).ToString(),
                        "Add" => (number1 + number2).ToString(),
                        "Subtract" => (number1 - number2).ToString(),
                        _ => "Invalid command"
                    };

                    Console.WriteLine($"Command: {method} Result: {result}");

                    // Send result back to client
                    writer.WriteLine(result);
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine("Invalid command");
                    writer.Flush();
                }
            }
        }

    }
}
