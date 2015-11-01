using InTheHand.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System.Threading;
using System.IO;
using System;
using System.Linq;
using System.Text;

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
    public class Connector {
        Thread btThread;
        bool CancellationPending = false;
        public Connector() {
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
                Console.WriteLine("Connection estabilished");
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
                        try
                        {
                            //byte[] TestByte = new byte[1] { 0x01 };
                            //TransportStream.Write(TestByte, 0, 1);   // This is to test if the connection is intact
                                                                     // Be aware that one byte is sent to the other client
                                                                     // the other client has to process (e.g. ignore) this byte
                        }
                        catch
                        {
                            Console.WriteLine("Connection lost!");
                            Thread.Sleep(100);

                        }
                        if (cli.Connected)
                        {
                            BytesRead = TransportStream.ReadByte();
                            Console.WriteLine("Read bytes");
                            Console.WriteLine(BytesRead);
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
