using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    public GameObject Player
    {
        get => player;
        set => player = value;
    }

    public GameObject barrier;

    private float yCutoffValue = 0.0f;
    public float yCutoffValueOffset = 10;

    void Update()
    {
        float x = player.transform.position.x;
        float y = this.transform.position.y;
        float z = this.transform.position.z;

        if (player.transform.position.y < this.transform.position.y)
        {
            y = player.transform.position.y;
        }

        this.transform.position = new Vector3(x, y, z);
        yCutoffValue = y + yCutoffValueOffset;
        barrier.transform.position = new Vector3(x, yCutoffValue, 0.0f);
    }

    public bool OutOfBounds(int x, int y)
    {
        if (y >= yCutoffValue)
        {
            return true;
        }

        return false;
    }
}
