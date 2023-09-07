using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            bool connection = SteamLobby.instance.CanReconnet();
            gameObject.SetActive(connection);
            GetComponent<Button>().interactable = connection;
            started = true;
        }
    }

    public void OnClick() 
    {
        SteamMatchmaking.JoinLobby(new CSteamID(Convert.ToUInt64(PlayerPrefs.GetString("currentLobbyID"))));
    }
}
