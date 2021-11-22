using UnityEngine;


public class Calibration : MonoBehaviour
{
    public int[] Limits;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("calibration");
        
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }        DontDestroyOnLoad(this.gameObject);
    }

}


