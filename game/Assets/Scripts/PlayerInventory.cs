using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerInventory : MonoBehaviour
{
    public int diamonds { get; private set; }
    
    public UnityEvent<PlayerInventory> OnDiamondCollected;
    
    public void AddDiamond()
    {
        diamonds++;
        OnDiamondCollected.Invoke(this);
    }
}
