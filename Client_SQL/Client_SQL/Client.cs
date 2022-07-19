using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client_SQL
{
    public class Client
    {
        public static int Main(String[] args)
        {
            StartClient();
            return 0;
        }

        public static void StartClient()
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    String requete ="";
                    int i = 1;
                    while (true)
                    {
                        requete = Console.ReadLine();
                        requete += " FOR JSON AUTO";
                        byte[] msg = Encoding.ASCII.GetBytes(requete);

                        // Send the data through the socket.
                        int bytesSent = sender.Send(msg);

                        // Receive the response from the remote device.
                        int bytesRec = sender.Receive(bytes);
                        String response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                         Newtonsoft.Json.JsonConvert.SerializeObject(response, Formatting.Indented);
                        JArray parsed =JArray.Parse(response);
                        foreach (JObject pair in parsed) {
                            Console.WriteLine("-----------------------------Ligne " + i.ToString() +"------------------------------");
                            foreach (var item in pair) {
                                Console.WriteLine("{0}: {1}", item.Key.ToString(), item.Value.ToString());
                            }
                            i += 1;
                        }
                        i = 0;
                        if (requete.IndexOf("close") > -1)
                        {
                            break;
                        }
                    }
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
