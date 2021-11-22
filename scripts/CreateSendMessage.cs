using UnityEngine;
using ArduinoBluetoothAPI;
using System.Threading.Tasks;

public class CreateSendMessage : MonoBehaviour
{
    public BLEManager BLE;

    private string[] ArmData = {"0", "0"};
    private BluetoothHelperCharacteristic[] contacts;
    private GameObject CalibrationManager;
    private Calibration cal;

    void Start()
    {
        contacts = new BluetoothHelperCharacteristic[4];
        CalibrationManager = GameObject.Find("CalibrationManager");
        cal = CalibrationManager.GetComponent<Calibration>();
        contacts[0] = BLE.contact1;
        contacts[1] = BLE.contact2;
        contacts[2] = BLE.contact3;
        contacts[3] = BLE.contact4;
    }

    public void CreateMessage(Vector3 force, bool isLeft) //GameObject hand)
    {
        Debug.Log("CreateMessage running.. ");
        //create amplitude message
        if (!isLeft)
        {
            Debug.Log("Right side");
            ArmData[0] = mappedForce(0, force).ToString(); //message for UpArm contact;
            ArmData[1] = mappedForce(1, force).ToString(); //message for DownArm contact;
        }
        else
        {
            Debug.Log("Left side");
            ArmData[0] = mappedForce(1, force).ToString(); //message for UpArm contact; //2 changed for 1 for first test only
            ArmData[1] = mappedForce(3, force).ToString(); //message for DownArm contact;
        }
        
        //Debug.Log("ArmData[0], ArmData[1]: " + ArmData[0] + ", " + ArmData[1]);
        sendToBLEManager(ArmData, isLeft);
    }

    public void CreateMaxMessage(bool isLeft) //GameObject hand)
    {
        //create amplitude message using the maxima of calibration(hit force not used, no mapping)
        if (!isLeft)
        {
            Debug.Log("Right side");
            ArmData[0] = cal.Limits[4].ToString(); //send max of muscle 1 
            ArmData[1] = cal.Limits[5].ToString(); //send max of muscle 2
        }
        else
        {
            Debug.Log("Left side");
            ArmData[0] = cal.Limits[5].ToString(); //send max of muscle 3 -  6 changed to 5 for testing only
            ArmData[1] = cal.Limits[7].ToString(); //send max of muscle 4
        }

        //Debug.Log("ArmData[0], ArmData[1]: " + ArmData[0] + ", " + ArmData[1]);
        sendToBLEManager(ArmData, isLeft);
    }

    public float mappedForce(int contactIndex, Vector3 force)
    {
        Debug.Log("mappedForce running.. ");

        //MEASUREMENTS
        //force meaured when hand hits the ball with no speed, (-3.5, -4.3, 0.3) magn = 0
        //force meaured when hand hits the ball with highest speed (-27.3, -73.7, -26.5) magn = 82.9

        //maps amplitude depending on calibration values
        float normal = Mathf.InverseLerp(0f, 82.9f, force.magnitude);
        float remap = Mathf.Lerp(cal.Limits[contactIndex], cal.Limits[contactIndex + 4], normal);
        Debug.Log("remap: " + remap);
        return (int) remap;
    }

    public void sendToBLEManager(string[] message, bool isLeft)
    {
        Debug.Log("CreateMessage running.. ");
        if (!isLeft)  //send feedback to right arm
        {
            BLE.SendData(BLE.contact1, message[0]);

            //second arm contact not tested yet
            //Task.Delay(10); //needed for second message to be detected by ESP32.
            //BLE.sendData(BLE.contact2, message[1]);

        }
        else   //send feedback to left arm
        {
            BLE.SendData(BLE.contact2, message[0]); //contact3 changed for contact2 for first test only

            //second arm contact not tested yet
            //Task.Delay(10); //needed for second message to be detected by ESP32.
            //BLE.sendData(BLE.contact4, message[1]);
        }

    }



}
