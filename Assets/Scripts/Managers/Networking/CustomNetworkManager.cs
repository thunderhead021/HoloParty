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
	public bool hasSessionStarted = false;
	[SerializeField]
	private string curScene;
	public List<PlayerController> players { get; } = new List<PlayerController>();

	private Dictionary<object, GameObject> disconnectedPlayers { get; } = new Dictionary<object, GameObject>();

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		if (hasSessionStarted) 
		{	
			if (disconnectedPlayers.Count == 0)
			{
				NetworkServer.RemoveConnection(conn.connectionId);
				return;
			}
			else 
			{
				Debug.Log((ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, players.Count));
				foreach (KeyValuePair<object, GameObject> ele1 in disconnectedPlayers)
				{
					if (conn.authenticationData == ele1.Key)
					{
						Debug.Log("Found disconnected player");
						NetworkServer.ReplacePlayerForConnection(conn, ele1.Value);
						break;
					}
				}
			}
		}
		else if (SceneManager.GetActiveScene().name == "Lobby")
		{
			PlayerController pcInstance = Instantiate(pcPrefab);
			pcInstance.connectID = conn.connectionId;
			pcInstance.playerIDnumber = players.Count + 1;
			pcInstance.playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, players.Count);

			NetworkServer.AddPlayerForConnection(conn, pcInstance.gameObject);
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		object data = conn.authenticationData;
		if (hasSessionStarted)
		{
			foreach (PlayerController player in players)
			{
				if (player.connectID == conn.connectionId)
				{
					if (hasSessionStarted)
					{
						Debug.Log(player.playerSteamName + " ID: "  + player.playerSteamID + " has disconnected!");
						disconnectedPlayers.Add(data, player.gameObject);
						player.gameObject.SetActive(false);
						NetworkServer.RemovePlayerForConnection(conn, false);
					}
					break;
				}
			}
		}
		//base.OnServerDisconnect(conn);
	}

	public void StartGame(string sceneName) 
	{
		ServerChangeScene(sceneName);
		curScene = sceneName;
		hasSessionStarted = true;
	}
}
