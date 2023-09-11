using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterMinigameCard : MonoBehaviour
{
    public Image character;
    public Text placement;
    public CharacterIconDatabase iconDatabase;
    public int playerId;

    public void SetCharImage(int charId, int id) 
    {
        character.sprite = iconDatabase.GetIcon(charId);
        playerId = id;
    }

    public void SetPlactment(int place) 
    {
        switch (place) 
        {
            case 1:
                placement.text = "1st";
                break;
            case 2:
                placement.text = "2nd";
                break;
            case 3:
                placement.text = "3rd";
                break;
            case 4:
                placement.text = "4th";
                break;
            default:
                break;
        }
    }
}
