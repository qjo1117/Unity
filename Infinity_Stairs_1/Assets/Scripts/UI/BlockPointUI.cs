using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockPointUI : MonoBehaviour
{
    PlayerController player = null;

    int currentValue = 0;

    Text text;

    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        text = transform.GetChild(0).GetComponent<Text>();
    }

    void Update()
    {
        int value = player.BlockCount;
        if(currentValue != value) {
            currentValue = value;
            text.text = $"{currentValue}";
        }
    }

    
}
