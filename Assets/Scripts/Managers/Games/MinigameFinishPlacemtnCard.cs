using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameFinishPlacemtnCard : MonoBehaviour
{
    public Text placement;

    public void SetPlacement(string place) 
    {
        placement.text = place;
    }
}
