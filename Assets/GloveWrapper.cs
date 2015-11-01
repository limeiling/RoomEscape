using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using System.Threading;
using System.IO;
using System;
using System.Linq;

public class GloveWrapper : MonoBehaviour
{

    Thread btThread;
    bool CancellationPending = false;

    // Use this for initialization
    void Start()
    {
        //Starts bluetooth listener/connector as a thread
        btThread = new Thread(Connect);
        btThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void Connect()
    {//Gets called at Start()

        Debug.Log("Started");
        int result = 0;
        Stream TransportStream = null;
        string pDeviceAddress = "000666662480";
        BluetoothManager _BluetoothManager = new BluetoothManager();
        BluetoothClient cli = new BluetoothClient();
        var devices = cli.DiscoverDevices();
        foreach (var device in devices)
        {
            Debug.Log(device.DeviceName);
        }
        var peerDevice = cli.DiscoverDevices().Where(d => d.DeviceAddress.ToString() == pDeviceAddress).FirstOrDefault();
        if (peerDevice == null){
            Debug.Log("Oh no");
            Debug.Log(cli.DiscoverDevices()[0].DeviceAddress.ToString());
        }
        else
        {
            Debug.Log(peerDevice.Connected);
        }
        var addr = peerDevice.DeviceAddress;
        var rep = new BluetoothEndPoint(addr, BluetoothService.SerialPort);
        cli = new BluetoothClient();
        cli.SetPin(addr, "1234");
        while (!CancellationPending)
        {
            int ConnectAttemptCount = 1;
            while (!cli.Connected & !CancellationPending)
            {
                try
                {
                    cli.Connect(rep);
                }
                catch
                {

                }
                ConnectAttemptCount++;
                Thread.Sleep(200);
            }
            if (CancellationPending)
            {
                result = 2;
            }
            if (cli.Connected)
            {
                try
                {
                    TransportStream = cli.GetStream();
                }
                catch (Exception Ex3)
                {
                }
                Byte[] MyBuffer;
                int BytesRead = 0;
                while (cli.Connected & !CancellationPending)
                {
                    try
                    {
                        byte[] TestByte = new byte[1] { 0x01 };
                        TransportStream.Write(TestByte, 0, 1);   // This is to test if the connection is intact
                                                                 // Be aware that one byte is sent to the other client
                                                                 // the other client has to process (e.g. ignore) this byte
                    }
                    catch
                    {
                        Thread.Sleep(3000);

                    }
                    if (cli.Connected)
                    {
                        MyBuffer = new byte[200];
                        BytesRead = TransportStream.Read(MyBuffer, 0, 199);
                        _BluetoothManager.DataReceived(MyBuffer, 0, BytesRead);
                    }
                }
                if (CancellationPending)
                {
                    result = 3;
                }
                try
                {
                    TransportStream.Close();
                }
                catch
                {
                }
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

        // Send data to whom ever interested
        if (NewBluetoothDataReceived != null)
        {
            NewBluetoothDataReceived(this, new BluetoothDataEventArgs(OutData));
        }
        else
        {
            Debug.Log(OutData);
        }
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
