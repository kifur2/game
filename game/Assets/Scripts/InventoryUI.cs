using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI _diamondText;
    
    void Start()
    {
        _diamondText = GetComponent<TextMeshProUGUI>();
    }
    
    public void UpdateDiamonds(PlayerProperties playerProperties)
    {
        _diamondText.text = playerProperties.Diamonds.ToString();
    }
}
