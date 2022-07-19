using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace Serveur_SQL
{
    public class Server
    {
        public static int Main(String[] args)
        {
            StartServer();
            return 0;
        }

        public static void StartServer()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 1234);
            // Connection serveur SQL
            SqlConnection conn = new SqlConnection(@"Server=localhost\SQLEXPRESS;Database=Extranet;Trusted_Connection=True;");
            string script = "";
                try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for a connection...");
                Socket handler = listener.Accept();

                // Incoming data from the client.
                string data = null;
                byte[] bytes = null;

                string jsonresult = "";
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("Text received : {0}", data);
                    try
                    {
                        conn.Open();
                        script = data;
                        SqlCommand commande = conn.CreateCommand();
                        commande.CommandText = script;
                        SqlDataReader read = commande.ExecuteReader();
                        data = "";
                         var ColumnSchema = read.GetColumnSchema();
                           
                        while (read.Read())
                        {
                             jsonresult = read[0].ToString();
                         
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        // Console.WriteLine("Can not open connection ! ", ex);
                    }
                    
                    byte[] msg = Encoding.ASCII.GetBytes(jsonresult);
                    handler.Send(msg);
                    if (data.IndexOf("close") > -1)
                    {
                        break;
                    }
                }

                Console.WriteLine("Text received : {0}", data);
                
                //byte[] msg = Encoding.ASCII.GetBytes(data);
                //handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }
    }
}