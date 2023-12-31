﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerController : NetworkBehaviour
{
    //Network Player data
    [SyncVar] 
    public int connectID;
    [SyncVar]
    public int playerIDnumber;
    [SyncVar]
    public ulong playerSteamID;
    [SyncVar(hook = nameof(playerSteamNameUpdate))] public string playerSteamName;

    [SyncVar(hook = nameof(playerReadyUpdate))] public bool ready;

    //Character data
    [SyncVar(hook = nameof(playerCharID))] public int charID = 0;
    [SyncVar(hook = nameof(playerCharModel))] public Color charModel;

    private CustomNetworkManager networkManager;

	private void Start()
	{
        DontDestroyOnLoad(gameObject);
	}

	private CustomNetworkManager CustomNetworkManager 
    {
        get 
        {
            if (networkManager != null)
                return networkManager;
            return networkManager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

	public override void OnStartAuthority()
	{
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalPlayer";
        LobbyManager.instance.FindLocalPlayer();
        LobbyManager.instance.UpdateLobbyName();

    }

	public override void OnStartClient()
	{
        CustomNetworkManager.players.Add(this);
        LobbyManager.instance.UpdateLobbyName();
        LobbyManager.instance.UpdatePlayerList();
    }

	public override void OnStopClient()
	{
        CustomNetworkManager.players.Remove(this);
        LobbyManager.instance.UpdatePlayerList();
    }

    [Command]
    public void CmdCanStartGame(string sceneName)
    {
        networkManager.StartGame(sceneName);
    }

    public void CanStartGame(string sceneName) 
    {
        if (isOwned)
            CmdCanStartGame(sceneName);
    }


    [Command]
    private void CmdSetPlayerReady()
    {
        playerReadyUpdate(ready, !ready);
    }

    public void ChangeReady() 
    {
        if (isOwned)
            CmdSetPlayerReady();
    }

    private void playerReadyUpdate(bool oldVal, bool newVal)
    {
        if (isServer)
            ready = newVal;
        if (isClient)
            LobbyManager.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName) 
    {
        playerSteamNameUpdate(playerSteamName, playerName);
    }

    public void playerSteamNameUpdate(string tempName, string playerName) 
    {
        if (isServer)
            playerSteamName = playerName;
        if (isClient)
            LobbyManager.instance.UpdatePlayerList();
    }

    [Command]
    public void CmdUpdatePlayerChar(int value, Color model) 
    {
        playerCharID(charID, value);
        playerCharModel(charModel, model);
        LobbyManager.instance.UpdatePlayerList();
    }

    public void playerCharModel(Color oldVal, Color newVal)
    {
        if (isServer)
            charModel = newVal;
        else if (isClient && (oldVal != newVal))
            ClientCharModelUpdate(newVal);
    }

    private void ClientCharModelUpdate(Color model)
    {
        charModel = model;
    }

    public void playerCharID(int oldVal, int newVal) 
    {
        if (isServer)
            charID = newVal;
        else if (isClient && (oldVal != newVal))
            ClientCharIDUpdate(newVal);
    }

    private void ClientCharIDUpdate(int id) 
    {
        charID = id;
    }

    public void ChangeCharSelected(int id, Color model)
    {
        if (isOwned)
            CmdUpdatePlayerChar(id, model);
    }
}
