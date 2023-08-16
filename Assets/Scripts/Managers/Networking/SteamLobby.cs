using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System;

public class SteamLobby : MonoBehaviour
{

	public static SteamLobby instance; 

	//Callback
	protected Callback<LobbyCreated_t> lobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequest;
	protected Callback<LobbyEnter_t> lobbyEnter;

	[HideInInspector]
	public bool finishFindLobby = false;
	protected Callback<LobbyMatchList_t> lobbyList;
	protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;
	List<CSteamID> lobbyIDs = new List<CSteamID>();

	public ulong currentLobbyID;
	private const string hostAddressKey = "HostAddress";
	private CustomNetworkManager networkManager;

	[HideInInspector]
	public bool finishStart = false;

	private void Start()
	{
		if (!SteamManager.Initialized) { return; }
		if (instance == null) { instance = this;  }
		networkManager = GetComponent<CustomNetworkManager>();

		lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		lobbyJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
		lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);

		lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
		Debug.Log("Avaliable lobbies: " + lobbyIDs.Count);
		lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
		finishStart = true;
	}

	public bool CanReconnet() 
	{
		if (PlayerPrefs.HasKey("currentLobbyID") == false)
			return false;
		//CSteamID curlobbyID = new CSteamID(Convert.ToUInt64(PlayerPrefs.GetString("currentLobbyID")));
		//Debug.Log(curlobbyID + ", " + SteamMatchmaking.GetNumLobbyMembers(curlobbyID));
		//if (SteamMatchmaking.JoinLobby(curlobbyID) > 0)
		return true;
		//Debug.Log("Game is ended/Lobby not exist");
		//PlayerPrefs.DeleteKey("currentLobbyID");
		//PlayerPrefs.Save();
		//return false;
	}

	private void OnLobbyCreated(LobbyCreated_t lobbyCreated_T)
	{
		if (lobbyCreated_T.m_eResult != EResult.k_EResultOK) 
		{
			return;
		}

		Debug.Log("Lobby created");

		networkManager.StartHost();

		SteamMatchmaking.SetLobbyData(new CSteamID(lobbyCreated_T.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
		SteamMatchmaking.SetLobbyData(new CSteamID(lobbyCreated_T.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");
	}

	public void OnHostBtnClick() 
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
	}


	private void OnJoinRequest(GameLobbyJoinRequested_t gameLobbyJoinRequested_T) 
	{
		SteamMatchmaking.JoinLobby(gameLobbyJoinRequested_T.m_steamIDLobby);
	}

	private void OnLobbyEnter(LobbyEnter_t lobbyEnter_T) 
	{
		currentLobbyID = lobbyEnter_T.m_ulSteamIDLobby;
		PlayerPrefs.SetString("currentLobbyID", currentLobbyID.ToString());
		PlayerPrefs.Save();
		//Client
		if (NetworkServer.active) { return; }

		networkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyEnter_T.m_ulSteamIDLobby), hostAddressKey);

		networkManager.StartClient();
	}


	private void OnGetLobbyList(LobbyMatchList_t lobbyMatchList) 
	{
		if (lobbyIDs.Count > 0)
			lobbyIDs.Clear();
		for (int i = 0; i < lobbyMatchList.m_nLobbiesMatching; i++) 
		{
			CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
			lobbyIDs.Add(lobbyID);
			SteamMatchmaking.RequestLobbyData(lobbyID);
		}
		Debug.Log("3 " + lobbyIDs.Count);
		finishFindLobby = true;
	}

	private void OnGetLobbyData(LobbyDataUpdate_t lobbyData) 
	{

	}

	public void GetLobbies() 
	{
		if (lobbyIDs.Count > 0)
			lobbyIDs.Clear();
		SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
		SteamMatchmaking.RequestLobbyList();
		finishFindLobby = false;
		Debug.Log("1 "+ lobbyIDs.Count);
	}

	public void FindMatchBtnClick() 
	{
		StartCoroutine(FindMatch());	
	}

	IEnumerator FindMatch() 
	{
		GetLobbies();
		yield return new WaitWhile(() => finishFindLobby == false);
		if (lobbyIDs.Count > 0)
		{
			SteamMatchmaking.JoinLobby(lobbyIDs[UnityEngine.Random.Range(0, lobbyIDs.Count)]);
		}
		else
			OnHostBtnClick();
	}
}
