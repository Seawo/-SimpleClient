using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");

        transform.Translate(0, 0, v* speed * Time.deltaTime);
        transform.Rotate(0, h* rotateSpeed * Time.deltaTime * 180, 0);
    }
}
