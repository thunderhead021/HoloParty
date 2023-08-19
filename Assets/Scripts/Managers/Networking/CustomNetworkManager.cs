using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using System;

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

	public override void Start()
	{
		base.Start();
	}

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
				GameObject player = Instantiate(playerPrefab);
				NetworkServer.AddPlayerForConnection(conn, player);

				ulong id = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.instance.currentLobbyID) - 1);
				Debug.Log(id + ", " + players.Count + ", " + conn.connectionId + ", " + SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.instance.currentLobbyID));
				foreach (KeyValuePair<ulong, GameObject> ele1 in disconnectedPlayers)
				{
					if (id == ele1.Key)
					{
						Debug.Log("Found disconnected player");
						GameObject newPC = Instantiate(ele1.Value);
						newPC.GetComponent<PlayerController>().connectID = conn.connectionId;
						newPC.GetComponent<PlayerController>().playerIDnumber = players.Count + 1;
						connectedPlayers.Add(ele1.Key, newPC.GetComponent<PlayerController>());

						//NetworkServer.AddPlayerForConnection(conn, newPC);
						//newPC.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
						newPC.SetActive(true);
						Destroy(ele1.Value, 0.1f);
						disconnectedPlayers.Remove(ele1.Key);
						Destroy(player, 0.1f);
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
			Debug.Log(pcInstance.playerSteamID + ", " + players.Count + ", " + conn.connectionId + ", " + SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.instance.currentLobbyID));
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
						if(!disconnectedPlayers.ContainsKey(ele1.Key))
							disconnectedPlayers.Add(ele1.Key, ele1.Value.gameObject);
						
						connectedPlayers.Remove(ele1.Key);
						players.Remove(ele1.Value);
						NetworkServer.RemovePlayerForConnection(conn, false);
						//ele1.Value.gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();
						ele1.Value.gameObject.SetActive(false);
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
