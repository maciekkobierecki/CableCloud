using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CableCloud
{
    public class CableCloud
    {

        static void Main(string[] args)
        {
            Console.Title = "Cable Cloud";
            ConnectionsTable.init();
            SendingManager.init();
            initListeningOnSockets();
        }

        private static void initListeningOnSockets()
        {
            List<CloudPortsRow> networkTopology = ConnectionsTable.getPortsList();
            foreach (CloudPortsRow row in networkTopology)
            {
                initListenThread(row);
            }
        }
        
        private static Thread initListenThread(CloudPortsRow row)
        {
            var t = new Thread(() => RealStart(row));
            t.Start();
            return t;
        }
        private static void RealStart(CloudPortsRow row)
        {
            int outputPort = row.getOutPort();
            String nodeName = row.getNodeName();
            InSocket inputSocket = new InSocket(row.getInPort(), nodeName);
            OutSocket outputSocket = new OutSocket(outputPort, nodeName);
            while (true)
            {
                inputSocket.ListenForConnection();
                outputSocket.ListenForConnection();
                SendingManager.addOutSocket(outputSocket);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Connected with node: " + nodeName);
                try
                {
                    inputSocket.ListenForIncomingData();
                }
                catch (SocketException e)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Node " + nodeName + " disconnected.");
                }
            }            
        }

        public static string GetTimeStamp()
        {
            DateTime dateTime = DateTime.Now;
            return dateTime.ToString("HH:mm:ss.ff");
        }
    }
}
