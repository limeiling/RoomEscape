using InTheHand.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System.Threading;
using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BTClient
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Connector c = new Connector();
            while (true) { }
        }
    }

    public class Server
    {
        Thread serverThread;
        TcpListener server;
        GloveDataPacket packet = null;
        public Server() {
            StartServer();
        }

        public void ServeGlovePacket(GloveDataPacket packet)
        {
            this.packet = packet;
        }

        public void StartServer()
        {
            serverThread = new Thread(RunServer);
            serverThread.Start();
        }
        public void RunServer()
        {
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();


                    // Loop to receive all the data sent by the client.
                    while (true)
                    {
                        if(packet != null)
                        {
                            data = "{ 'finger0':" + packet.finger0 + ", 'finger1': " + packet.finger1 + ", 'finger2':" + packet.finger2 + ", 'heading':" + packet.heading + ", 'pitch':" + packet.pitch + ", 'roll':" + packet.roll + "}";

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            packet = null;
                        }
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }

    public class Connector {
        Thread btThread;
        bool CancellationPending = false;
    Server server;
        public Connector() {
            server = new Server();
            btThread = new Thread(Connect);
            btThread.Start();
        }
        public void Connect()
        {//Gets called at Start()

            Console.WriteLine("Started");
            int result = 0;
            Stream TransportStream = null;
            string pDeviceAddress = "000666662480";
            BluetoothManager _BluetoothManager = new BluetoothManager();
            BluetoothClient cli = new BluetoothClient();
            var peerDevice = cli.DiscoverDevices().Where(d => d.DeviceAddress.ToString() == pDeviceAddress).FirstOrDefault();
            if (peerDevice == null)
            {
                Console.WriteLine("Device not found!!!");
            }
            else
            {
                Console.WriteLine("Found: " + peerDevice.DeviceName);
            }
            var addr = peerDevice.DeviceAddress;
            var rep = new BluetoothEndPoint(addr, BluetoothService.SerialPort);
            cli = new BluetoothClient();
            while (!CancellationPending)
            {
                int ConnectAttemptCount = 1;
                while (!cli.Connected & !CancellationPending)
                {
                    try
                    {
                        cli.Connect(rep);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Connection error" + e);
                    }
                    ConnectAttemptCount++;
                    Thread.Sleep(20);
                }
                Console.WriteLine("Connection established");
                if (CancellationPending)
                {
                    result = 2;
                }
                if (cli.Connected)
                {
                    try
                    {
                        TransportStream = cli.GetStream();
                        Console.WriteLine("Got stream!");
                    }
                    catch (Exception Ex3)
                    {
                        Console.WriteLine("Stream lost!");
                    }
                    Byte[] MyBuffer;
                    int BytesRead = 0;
                    while (cli.Connected)
                    {
                        /*try
                        {
                            byte[] TestByte = new byte[1] { 0x01 };
                            TransportStream.Write(TestByte, 0, 1);   // This is to test if the connection is intact
                                                                     // Be aware that one byte is sent to the other client
                                                                     // the other client has to process (e.g. ignore) this byte
                        }
                        catch
                        {
                            Console.WriteLine("Connection lost!");
                            Thread.Sleep(100);

                        }*/
                        if (cli.Connected)
                        {
                            //BytesRead = TransportStream.ReadByte();
                            MyBuffer = new byte[20];
                            BytesRead = TransportStream.Read(MyBuffer, 0, 19);
                            var packet = new GloveDataPacket(MyBuffer, BytesRead);
                            server.ServeGlovePacket(packet);

                        }
                        else
                        {
                            Console.WriteLine("Connection lost");
                        }
                    }
                    if (CancellationPending)
                    {
                        result = 3;
                    }
                }
                else
                {
                    Console.WriteLine("Connection lost!");
                }
            }
        }
    }

    class BluetoothManager : IDisposable
    {
        private byte[] OutData;

        public BluetoothManager()
        {
        }

        ~BluetoothManager()
        {
            Dispose(false);
        }

        public event EventHandler<BluetoothDataEventArgs> NewBluetoothDataReceived;

        public void Dispose()
        {
            Dispose(true);
        }

        public void DataReceived(byte[] data, int offset, int count)
        {
            OutData = new byte[count];
            if (count == 0)
            {
                return;
            }
            OutData = new byte[count];
            Array.Copy(data, offset, OutData, 0, count);


           Console.WriteLine(OutData);
            Console.WriteLine(data);
        }

        // Part of basic design pattern for implementing Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                NewBluetoothDataReceived -= new EventHandler<BluetoothDataEventArgs>(NewBluetoothDataReceived);
            }
            // Here release unmanaged objects
        }
    }

    public class BluetoothDataEventArgs : EventArgs
    {

        public BluetoothDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }
        /// Byte array containing serial data
        public byte[] Data;
    }
}
