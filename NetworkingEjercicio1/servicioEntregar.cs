using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServidorMultihilo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Creamos el endpoint con los datos de conexión
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, 22222);

            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    server.Bind(ie);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }

                server.Listen(10);
                Console.WriteLine("Listening at {0}:{1}.", ie.Address, ie.Port);

                while (true)
                {
                    try
                    {
                        Socket client = server.Accept();
                        Thread thread = new Thread(() => ConnectionCreated(client, server));
                        thread.Start();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        break;
                    }
                }
            }
        }

        private static void ConnectionCreated(Socket client, Socket server)
        {
            //Endpoint con el cliente recién conectado
            IPEndPoint clientEp = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("Connected by {0}:{1}.", clientEp.Address, clientEp.Port);

            try
            {
                ClientResponse(client, server);
            }
            finally
            {
                client.Close();
                Console.WriteLine("Closed socket for {0}:{1}.", clientEp.Address, clientEp.Port);
            }
        }

        private static void ClientResponse(Socket client, Socket server)
        {

            using (NetworkStream ns = new NetworkStream(client))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                IPEndPoint clientEp = (IPEndPoint)client.RemoteEndPoint;
                string command;

                try
                {
                    command = sr.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: "+e.Message);
                    return;
                }

                if (command == null)
                {
                    Console.WriteLine("Connection closed");
                }
                else
                {
                    string information = CommandMannager(command, server);

                    try
                    {
                        sw.WriteLine(information);
                        sw.Flush();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: "+e.Message);
                    }
                }
            }
        }

        private static string CommandMannager(string command, Socket server)
        {
            switch (command.ToUpper())
            {
                case "HORA":
                    return DateTime.Now.ToString("HH:mm:ss");
                case "FECHA":
                    return DateTime.Now.ToString("yyyy/MM/dd");
                case "TODO":
                    return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                case "APAGAR":
                    server.Close();
                    return "OK";
                default:
                    return "COMMAND NOT FOUND";
            }
        }
    }
}
