using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterButton : MonoBehaviour
{
    //temp code
    public Color color;
    public int index;

    public void SwitchCharacter() 
    {
        LobbyManager.instance.ChangePlayerCharID(index, color);
    }
}
