using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    public float yCutoffValue = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<DrillController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float x = player.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        if (player.transform.position.y > this.transform.position.y)
        {
            y = player.transform.position.y;
        }

        this.transform.position = new Vector3(x, y, z);
        yCutoffValue = y - 6;
    }
}
