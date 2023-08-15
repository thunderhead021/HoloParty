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
	private List<PlayerController> disconnectedPlayers { get; } = new List<PlayerController>();

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		PlayerController pcInstance = Instantiate(pcPrefab);
		pcInstance.connectID = conn.connectionId;
		pcInstance.playerIDnumber = players.Count + 1;
		pcInstance.playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, players.Count);

		NetworkServer.AddPlayerForConnection(conn, pcInstance.gameObject);
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
				foreach (PlayerController controller in disconnectedPlayers) 
				{
					if (controller.playerSteamID == pcInstance.playerSteamID)
					{
						Debug.Log("Found disconnected player");
						pcInstance.Copy(controller);
						disconnectedPlayers.Remove(controller);
						Destroy(controller);
						break;
					}
				}
			}
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		if (hasSessionStarted)
		{
			foreach (PlayerController player in players)
			{
				if (player.connectID == conn.connectionId)
				{
					if (hasSessionStarted)
					{
						Debug.Log(player.playerSteamName + " ID: "  + player.playerSteamID + " has disconnected!");
						player.gameObject.SetActive(false);
						NetworkServer.RemovePlayerForConnection(conn, false);
						disconnectedPlayers.Add(player);
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
