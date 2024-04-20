using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var playerProperties = other.GetComponent<PlayerProperties>();
        if (playerProperties == null || !other.CompareTag("Player")) return;
        playerProperties.AddDiamond();
        Destroy(gameObject);
    }
}