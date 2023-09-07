using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBtn : MonoBehaviour
{
    private PlayerController localPC;
    // Start is called before the first frame update
    void Start()
    {
        GameObject localPlayer = GameObject.Find("LocalPlayer");
        localPC = localPlayer.GetComponent<PlayerController>();
        gameObject.SetActive(localPC.playerIDnumber == 1);
    }

    public void StartGame(string sceneName)
    {
        localPC.CanStartGame(sceneName);
    }
}
