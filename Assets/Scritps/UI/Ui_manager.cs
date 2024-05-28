using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ui_manager : MonoBehaviour
{
    private bool m_option_check;

    private void Start()
    {
        m_option_check = false;
    }

    public void OnClickOption()
    {
        // Option button click
        if (!m_option_check)
        {
            m_option_check = true;
            Debug.Log("open option");
        }
        else
        {
            m_option_check = false;
            Debug.Log("close option");
        }
    }
}
