using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using tsst_client;


namespace CableCloud
{
    public class InSocket
    {
        public static int PORT_NUMBER_SIZE = 4;
        private Socket inputSocket = null;
        private TcpListener listener;
        private string nodeName;
        private int port;
        private Boolean listening;

        public InSocket(int _port, String associatedNodeName)
        {
            nodeName = associatedNodeName;
            port = _port;
        }

        private void GetNewInSocket(int p)
        {
            inputSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public void ListenForConnection()
        {
            if(!listening)
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listening = true;
            }
            inputSocket = listener.AcceptSocket();
        }

        public void ListenForIncomingData()
        {
            byte[] bytes;
            while (true)
            {
                bytes = new byte[4];
                int inputSize = ReceiveInputSize();
                byte[] receivedData = receiveData(inputSize);
                int sourcePort = GetSourcePort(receivedData);
                string timeStamp = CableCloud.GetTimeStamp();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(timeStamp + " | Recieved from node " + nodeName+ " from port "+sourcePort.ToString());
                SendingManager.Send(receivedData, nodeName, sourcePort);
            }
        }
        private int getDestPort(byte[] receivedData)
        {
            byte[] sourcePort = new byte[4];
            for (int i = 0; i < PORT_NUMBER_SIZE; i++)
                sourcePort[i] = receivedData[i];
            int convertedSourcePort = BitConverter.ToInt32(sourcePort, 0);
            return convertedSourcePort;
        }
        private int ReceiveInputSize()
        {
                byte[] objectSize = new byte[4];
                inputSocket.Receive(objectSize, 0, 4, SocketFlags.None);
                int messageSize = BitConverter.ToInt32(objectSize, 0);
                //Console.WriteLine(messageSize);
                return messageSize;    
        }

        private byte[] receiveData(int inputSize)
        {
            byte[] bytes = new byte[inputSize];
            int totalReceived = 0;
            do
            {
                int received = inputSocket.Receive(bytes, totalReceived, inputSize - totalReceived, SocketFlags.Partial);
                totalReceived += received;
            } while (totalReceived != inputSize);
           
            return bytes;
        }

        private int GetSourcePort(byte[] receivedData)
        {
            byte[] sourcePort = new byte[4];
            for (int i = 0; i < PORT_NUMBER_SIZE; i++)
                sourcePort[i]= receivedData[i+4];
            int convertedSourcePort = BitConverter.ToInt32(sourcePort,0);
            return convertedSourcePort;
        }



        private Packet GetDeserializedMessage(byte[] b)
        {
            Packet m = null;
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(b, 0, b.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            m = (Packet)binForm.Deserialize(memStream);
            return m;
        }

        private byte[] GetSerializedMessage(Packet mes)    //Serializacja bajtowa
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, mes);
            return ms.ToArray();
        }

    }
}