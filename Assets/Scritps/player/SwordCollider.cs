using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    public ParticleSystem[] swordImpact;

    private void OnTriggerEnter(Collider other)
    {
        // ����� �����Ҷ�
        if( other != null)
        {
            swordImpact[0].gameObject.SetActive(true);
        }
        else if( other.name == "")
        {
            swordImpact[1].gameObject.SetActive(true);
        }
    }
}
