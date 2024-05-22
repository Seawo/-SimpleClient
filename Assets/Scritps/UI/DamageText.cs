using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageText : MonoBehaviour
{
    public float m_moveSpeed; // 텍스트 이동속도
    public float m_alphaSpeed; // 투명도 변환속도
    public float m_DestroyTime;

    public GameObject thisObject;
    //public TextMesh m_text;
    public TextMeshProUGUI m_text;
    public Color m_alpha;


    void Start()
    {
        // Color.a 값에 따라 투명도가 변한다
        // 0 에 가까워 질수록 투명해진다
        // 범위 : 0.0f ~ 1.0f
        m_text = GetComponent<TextMeshProUGUI>();
        m_alpha = m_text.color;
        Invoke("DestroyObject", m_DestroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, m_moveSpeed * Time.deltaTime, 0));
        m_alpha.a = Mathf.Lerp(m_alpha.a, 0, Time.deltaTime * m_alphaSpeed);
        m_text.color = m_alpha;
    }

    private void DestroyObject()
    {
        //Destroy(this.gameObject);
        Destroy(thisObject);
    }
}
