using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class instance_NickName : MonoBehaviour
{
    private static instance_NickName instance = null;
    public static instance_NickName GetInstance() 
    {
        if (instance == null)
        {
            instance = new instance_NickName();
        }
        return instance; 
    }

    [SerializeField]
    public string charater_nicName = "";
    [SerializeField]
    TextMesh charaterNameMesh;


    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Environment_Free")
        {
            charaterNameMesh = GameObject.Find("CharaterName").GetComponent<TextMesh>();
            charaterNameMesh.text = charater_nicName;
        }
    }
}
