using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour
{
    public Transform player;
    void Start()
    {
        transform.position = player.position;
    }

    void Update()
    {
        transform.position = player.position;
    }
}
