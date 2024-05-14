using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningArea : MonoBehaviour
{
    [SerializeField] Enemy m_enemy = null;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player Enter");
            m_enemy.SetTarget(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player Exit");
            m_enemy.RemoveTarget();
        }
    }
}
