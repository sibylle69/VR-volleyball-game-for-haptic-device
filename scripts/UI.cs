using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using TMPro;
using System.Threading;
using System.Linq;

public class UI : MonoBehaviour
{
    public BLEManager BLEManager;

    public GameObject ConnectionState;
    public GameObject DropdownObj;
    public GameObject ScanningObj;
    public TMP_Dropdown Dropdown;

    //public enum connectionState { notConnected, connecting, connected };
    //private connectionState conState;

    public void ChangeState(GameObject ConnectionState)
    {
        if (ConnectionState.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            string state = "";
            switch (BLEManager.conState)
            {
                case BLEManager.connectionState.connected:
                    state = "Connected";
                    break;
                case BLEManager.connectionState.connecting:
                    state = "Connecting..";
                    break;
                case BLEManager.connectionState.notConnected:
                    state = "Not Connected";
                    break;
            }
            ConnectionState.GetComponentInChildren<TextMeshProUGUI>().text = state;
        }
    }

    public void genDropdown(LinkedList<BluetoothDevice> devices)
    {
        //create array of available bluetooth devices
        LinkedListNode<BluetoothDevice> node = devices.First;
        List<string> deviceList = new List<string>(); // List allows creating an array without knowing its size (better than string[] in this situation)

        foreach (BluetoothDevice btdevice in devices)
        {
            string bluetoothName = node.Value.DeviceName;
            Debug.Log(bluetoothName);
            deviceList.Add(bluetoothName);
            node = node.Next;
        }

        //populate dropdown with devices
        if (devices != null && devices.First != null)
        {
            Debug.Log("Starting dropdown");
            Dropdown = DropdownObj.GetComponent<TMP_Dropdown>();
            Dropdown.options.Clear();
            Dropdown.AddOptions(deviceList);
            Debug.Log("Devices found");
        }
        if (ScanningObj.GetComponentInChildren<TextMeshProUGUI>() != null)
            ScanningObj.GetComponentInChildren<TextMeshProUGUI>().text = "Scan";
    }
}
