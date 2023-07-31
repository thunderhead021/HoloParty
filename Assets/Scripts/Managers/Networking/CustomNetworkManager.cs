using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField]
    private PlayerController pcPrefab;

    public List<PlayerController> players { get; } = new List<PlayerController>();

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		if (SceneManager.GetActiveScene().name == "Lobby") 
		{
			PlayerController pcInstance = Instantiate(pcPrefab);
			pcInstance.connectID = conn.connectionId;
			pcInstance.playerIDnumber = players.Count + 1;
			pcInstance.playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, players.Count);

			NetworkServer.AddPlayerForConnection(conn, pcInstance.gameObject);
		}
	}

	public void StartGame(string sceneName) 
	{
		ServerChangeScene(sceneName);
	}
}
