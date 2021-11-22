using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using TMPro;
using UnityEngine.EventSystems;

public class SliderValue : MonoBehaviour, IDeselectHandler
{
    public BLEManager BLE;
    public Calibration cal;
    private Slider currentSlider;
    private int _index;
    private BluetoothHelperCharacteristic[] contacts;

    void Start()
    {
       contacts = new BluetoothHelperCharacteristic[4];
       currentSlider = GetComponent<Slider>();
       contacts[0] = BLE.contact1;
       contacts[1] = BLE.contact2;
       contacts[2] = BLE.contact3;
       contacts[3] = BLE.contact4;
    }

    public void UpdateEMS(int index)
    {
        UpdateUIValue();
        cal.Limits[index] = (int)currentSlider.value;
        //printArray();
        //Debug.Log("message: " + cal.Limits[index]);
        //Debug.Log("sent to: " + getContact(index));
        BLE.SendData(getContact(index), cal.Limits[index].ToString());
        _index = index;
    }

    public void UpdateUIValue()
    {
        if (GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = currentSlider.value.ToString();
        }
    }
    
    public BluetoothHelperCharacteristic getContact(int index)
    {
        int newIndex = index % 4;
        return contacts[newIndex];
    }

    private void printArray()
    {
         foreach(int i in cal.Limits)
        {
            Debug.Log(i);
        }
    }

    public void OnDeselect(BaseEventData data) // called when you press somewhere outside the slider
    {
        Debug.Log(data);
        BLE.SendData(getContact(_index), "0"); //reset the characteristic value to 0, stop EMS signal
        Debug.Log("slider OnDeselect");
    }
}


