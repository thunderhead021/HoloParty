using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconnectBtn : MonoBehaviour
{
    private bool started = false;
    // Start is called before the first frame update
    void Update()
    {
        if (started == true)
            return;
        if (SteamLobby.instance.finishStart == true)
        {
            Debug.Log("Checked reconnect");
            gameObject.SetActive(SteamLobby.instance.CanReconnet());
            started = true;
        }
    }

    public void OnClick() 
    {
        SteamMatchmaking.JoinLobby(new CSteamID(Convert.ToUInt64(PlayerPrefs.GetString("currentLobbyID"))));
    }
}
