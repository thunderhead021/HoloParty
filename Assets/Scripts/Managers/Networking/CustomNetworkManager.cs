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

	private Dictionary<ulong, GameObject> disconnectedPlayers { get; } = new Dictionary<ulong, GameObject>();
	private Dictionary<ulong, PlayerController> connectedPlayers { get; } = new Dictionary<ulong, PlayerController>();

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
				
				ulong id = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.instance.currentLobbyID));
				Debug.Log(id + ", " + players.Count + ", " + conn.connectionId + ", " + SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.instance.currentLobbyID));
				foreach (KeyValuePair<ulong, GameObject> ele1 in disconnectedPlayers)
				{
					if (id == ele1.Key)
					{
						Debug.Log("Found disconnected player");
						ele1.Value.SetActive(true);
						connectedPlayers.Add(ele1.Key, ele1.Value.GetComponent<PlayerController>());
						NetworkServer.ReplacePlayerForConnection(conn, ele1.Value);
						ele1.Value.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
						disconnectedPlayers.Remove(ele1.Key);

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

			connectedPlayers.Add(pcInstance.playerSteamID, pcInstance);
			NetworkServer.AddPlayerForConnection(conn, pcInstance.gameObject);
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		if (hasSessionStarted)
		{
			foreach (KeyValuePair<ulong, PlayerController> ele1 in connectedPlayers)
			{
				if (ele1.Value.connectID == conn.connectionId)
				{
					if (hasSessionStarted)
					{
						Debug.Log(ele1.Value.playerSteamName + " ID: " + ele1.Value.playerSteamID + " has disconnected!");
						Debug.Log(ele1.Key.ToString() + " in loop");
						disconnectedPlayers.Add(ele1.Key, ele1.Value.gameObject);
						ele1.Value.gameObject.SetActive(false);
						connectedPlayers.Remove(ele1.Key);
						//NetworkServer.RemovePlayerForConnection(conn, false);
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
