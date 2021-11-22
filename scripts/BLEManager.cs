using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Text;
using System.Threading;
using System.Linq;
using TMPro;


//adb logcat | find " I Unity"
//adb logcat | find " D BluetoothAPI"

public class BLEManager : MonoBehaviour
{
    private BluetoothHelper helper;
    private bool isScanning;
    private bool Available;

    private string data;
    public BluetoothHelperCharacteristic contact1;
    public BluetoothHelperCharacteristic contact2;
    public BluetoothHelperCharacteristic contact3;
    public BluetoothHelperCharacteristic contact4;

    public UI UI;

    //UI
    public GameObject ConnectionState;
    public GameObject DropdownObj;
    public GameObject ScanningObj;
    private string connectedDevice;

    public enum connectionState { notConnected, connecting, connected};
    public connectionState conState;

    private LinkedList<BluetoothDevice> devices;

    void OnEnable()
    {
        try
        {
            BluetoothHelper.BLE = true;

            helper = BluetoothHelper.GetInstance("ESP32test");
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.OnScanEnded += OnScanEnded;
            helper.OnCharacteristicChanged += OnCharacteristicChanged;

            contact1 = new BluetoothHelperCharacteristic("c25ca89e-1479-11ec-82a8-0242ac130003");
            contact2 = new BluetoothHelperCharacteristic("263b4998-1d43-11ec-9621-0242ac130002");
            contact3 = new BluetoothHelperCharacteristic("2b10fc38-1d43-11ec-9621-0242ac130002");
            contact4 = new BluetoothHelperCharacteristic("2eba72a6-1d43-11ec-9621-0242ac130002");

            contact1.setService("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
            contact2.setService("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
            contact3.setService("4fafc201-1fb5-459e-8fcc-c5c9c331914b");
            contact4.setService("4fafc201-1fb5-459e-8fcc-c5c9c331914b");

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
    {
        Debug.Log("Scan done");
        this.isScanning = false;
        this.devices = devices;

        UI.genDropdown(devices); //generate UI dropdown of devices

    }

    void OnConnected(BluetoothHelper _helper)
    {
        Debug.Log("Connected");
        conState = connectionState.connected;
        UI.ChangeState(ConnectionState);

        List<BluetoothHelperService> services = _helper.getGattServices();
        
        //log list of devices
        /*
        foreach (BluetoothHelperService s in services)
        {
            Debug.Log("Service : " + s.getName());
            foreach (BluetoothHelperCharacteristic item in s.getCharacteristics())
            {
                Debug.Log("Caract : " + item.getName());
            }
        }*/
    }

    void OnCharacteristicChanged(BluetoothHelper _helper, byte[] value, BluetoothHelperCharacteristic characteristic)
    {
        Debug.Log("Value: " + Encoding.Default.GetString(value)); //converts bytes to string
        data += "\n<" + Encoding.Default.GetString(value);
    }

    void OnConnectionFailed(BluetoothHelper helper)
    {
        Debug.Log("Connection lost");
        conState = connectionState.notConnected;
        UI.ChangeState(ConnectionState);
    }

    

    public void Scan() //called when Scan button pressed
    {
        Debug.Log("Scanning...");
        isScanning = helper.ScanNearbyDevices();
        if (ScanningObj.GetComponentInChildren<TextMeshProUGUI>() != null)
            ScanningObj.GetComponentInChildren<TextMeshProUGUI>().text = "Scanning..";
    }

    public void Connect() //called when a device is selected in dropdown
    {
        connectedDevice = UI.Dropdown.options[UI.Dropdown.value].text;
        helper.setDeviceName(connectedDevice);
        Debug.Log("connecting to..." + connectedDevice);
        try
        {
            helper.Connect();
            conState = connectionState.connecting;
            UI.ChangeState(ConnectionState);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    
    void OnApplicationQuit()
    {
        if (helper != null)
            helper.Disconnect();
            Debug.Log("Disconnected");
    }

    public void SendData(BluetoothHelperCharacteristic contact, string message)
    {
        Debug.Log("sendData running");
        helper.WriteCharacteristic(contact, message); //for now we use a single characteristic
    }
}
