using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LookAtCamera : MonoBehaviour
{
    public GameObject Taget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var position = new Vector3();
        var rotation = new Vector3(0,180,0);
        position.x = Taget.transform.position.x;
        position.y = Taget.transform.position.y;
        position.z = Taget.transform.position.z;
        this.transform.LookAt(position);

        this.transform.Rotate(rotation);
    }
}
