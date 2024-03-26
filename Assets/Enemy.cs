using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float curHealth = 0.0f;

    private Rigidbody rigid;
    private BoxCollider boxCollider;
    public  SkinnedMeshRenderer meshRenderer;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        curHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(OnDamage());
        if (other.tag == "Weapon")
        {
            //아직 미정
        }
    }

   IEnumerator OnDamage()
   {
        curHealth -= 10.0f;

        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (  curHealth > 0 ) 
        {
            meshRenderer.material.color = Color.white;
        }
        else if ( curHealth <= 0 ) 
        {
            meshRenderer.material.color = Color.black;
        }
    }
}
