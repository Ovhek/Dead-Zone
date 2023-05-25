using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWall : MonoBehaviour
{
    public GameObject bullet;

    public void enableCollider(bool enabled)
    {
        GetComponent<BoxCollider>().enabled = enabled;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
