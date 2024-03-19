using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNickName : MonoBehaviour
{
    instance_NickName mynickname;
    TextMesh mynickname_text;

    // Start is called before the first frame update
    void Start()
    {
        mynickname_text = GetComponent<TextMesh>();

        mynickname_text.text = mynickname.charater_nicName;
    }

    // Update is called once per frame
    void Update()
    {
        if( mynickname != null ) 
        {
            mynickname_text.text = mynickname.charater_nicName;
        }
    }
}
