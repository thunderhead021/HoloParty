using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneBtn : MonoBehaviour
{
    private PlayerController localPC;

    public bool allPlayerReady = false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject localPlayer = GameObject.Find("LocalPlayer");
        localPC = localPlayer.GetComponent<PlayerController>();
        gameObject.SetActive(localPC.playerIDnumber == 1 || allPlayerReady == true);
    }

    public void UpdateReady() 
    {
        localPC.ChangeMinigameReady();
    }

    public void StartGame(string sceneName)
    {
        if(localPC.playerIDnumber == 1)
            localPC.CanStartGame(sceneName);
    }
}
