using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    
    public ParticleSystem[] swordImpact;

    private void OnTriggerEnter(Collider other)
    {
        // Monster�� �¾Ҵٸ�...
        if( other.CompareTag("Monster"))
        {
            DamageEffect();
        }
        else
        {
            // �ٸ� ���� �¾Ҵٸ�...
        }
    }

    
    void DamageEffect()
    {
        ParticleSystem obj = (ParticleSystem)Instantiate(swordImpact[0]);
        obj.transform.SetParent(this.transform, false);
    }

    private void OnDisable()
    {
        Destroy(this.transform.GetChild(0).gameObject);        
    }
}
