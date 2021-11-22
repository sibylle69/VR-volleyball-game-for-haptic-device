using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Scene1()
    {
        SceneManager.LoadScene("bluetooth");
    }
    public void Scene2()
    {
        SceneManager.LoadScene("calibration");
    }
    public void Scene3()
    {
        SceneManager.LoadScene("game");
    }
}
