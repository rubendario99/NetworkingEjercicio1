using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingEjercicio1
{
    class Program
    {
        static void Main(string[] args)
        {

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
                    Environment.Exit(1);
                }

                server.Listen(10);
                Console.WriteLine("Listening at-> {0}:{1}", ie.Address, ie.Port);

                Socket client = server.Accept();
                IPEndPoint clientIE = (IPEndPoint)client.RemoteEndPoint;
                Console.WriteLine("Client connected at-> {0}:{1}", clientIE.Address, clientIE.Port);

                using (NetworkStream ns = new NetworkStream(client))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {

                    switch (sr.ReadLine().ToUpper())
                    {
                        case "HORA":
                            string hora = DateTime.Now.ToString("HH:mm:ss");
                            sw.WriteLine(hora);
                            server.Close();
                            break;
                        case "FECHA":
                            string fecha = DateTime.Now.ToString("dd/MM/yyyy");
                            sw.WriteLine(fecha);
                            server.Close();
                            break;
                        case "TODO":
                            string todo = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                            sw.WriteLine(todo);
                            server.Close();
                            break;
                        case "APAGAR":
                            sw.WriteLine("CLOSED");
                            server.Close();
                            break;
                        default:
                            sw.WriteLine("Unrecognized command");
                            break;
                    }

                }
                Console.WriteLine("Todas las conexiones cerradas" +"");
                Console.WriteLine("Pulsa una tecla para salir");
                Console.ReadKey();
            }
        }
    }
}
