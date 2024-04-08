using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        if (playerInventory!= null && other.CompareTag("Player"))
        {
            playerInventory.AddDiamond();
            Destroy(gameObject);
        }
    }
}
