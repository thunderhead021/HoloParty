using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    //UI elements
    public Text lobbyName;
    public List<GameObject> playerCards;
    public Button startGameButton;
    public Button readyBtn;

    //Networking elements
    public GameObject pcPrefab;
    public GameObject playerCardPrefab;
    public GameObject localPlayer;

    public ulong lobbyID;
    public bool playerInfoCreated = false;
    private List<PlayerLobbyInfo> playerLobbyInfos = new List<PlayerLobbyInfo>();
    public PlayerController localPC;

    

    private CustomNetworkManager networkManager;

    public void StartGame(string sceneName) 
    {
        localPC.CanStartGame(sceneName);
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

	private void Awake()
	{
        if (instance == null)
            instance = this;
	}

    public void ReadyPlayer() 
    {
        localPC.ChangeReady();
    }

    public void ChangePlayerCharID(int id, Color charModel) 
    {
        localPC.ChangeCharSelected(id, charModel);
    }

    public void CheckIfAllReady() 
    {
        bool allready = false;
        foreach (PlayerController playerController in CustomNetworkManager.players) 
        {
            allready = playerController.ready;
            if (playerController.ready == false)
                break;
        }

        startGameButton.gameObject.SetActive(false);
        if (allready && localPC.playerIDnumber == 1)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    public void UpdateLobbyName() 
    {
        lobbyID = CustomNetworkManager.GetComponent<SteamLobby>().currentLobbyID;
        lobbyName.text = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyID), "name");
    }

    public void UpdatePlayerList() 
    {
        if (!playerInfoCreated)
            CreateHostPlayerInfo();

        if (playerLobbyInfos.Count < networkManager.players.Count)
            CreateClientPlayerInfo();
        else if (playerLobbyInfos.Count > networkManager.players.Count)
            RemovePlayerInfo();
        else
            UpdatePlayerInfo();
    }

    public void FindLocalPlayer() 
    {
        localPlayer = GameObject.Find("LocalPlayer");
        localPC = localPlayer.GetComponent<PlayerController>();
    }


    private void CreatePlayerInfoCard(PlayerController playerController) 
    {
        GameObject playerInfo = Instantiate(playerCardPrefab);
        PlayerLobbyInfo newPlayerInfo = playerInfo.GetComponent<PlayerLobbyInfo>();

        newPlayerInfo.playerName = playerController.playerSteamName;
        newPlayerInfo.connectionID = playerController.connectID;
        newPlayerInfo.steamID = playerController.playerSteamID;
        newPlayerInfo.playerReady = playerController.ready;


        newPlayerInfo.SetPlayerInfo();

        foreach (GameObject card in playerCards)
        {
            if (card.transform.childCount == 0)
            {
                playerInfo.transform.SetParent(card.transform);
                playerInfo.transform.localScale = Vector3.one;
                break;
            }
        }

        playerLobbyInfos.Add(newPlayerInfo);
    }

    public void CreateHostPlayerInfo() 
    {
        foreach (PlayerController playerController in CustomNetworkManager.players) 
        {
            CreatePlayerInfoCard(playerController);
        }
        playerInfoCreated = true;
    }

    public void CreateClientPlayerInfo() 
    {
        foreach (PlayerController playerController in networkManager.players)
        {
            if (!playerLobbyInfos.Any(b => b.connectionID == playerController.connectID)) 
            {
                CreatePlayerInfoCard(playerController);

            }
        }
    }

    public void UpdatePlayerInfo() 
    {
        foreach (PlayerController playerController in networkManager.players) 
        {
            foreach (PlayerLobbyInfo lobbyInfo in playerLobbyInfos) 
            {
                if (lobbyInfo.connectionID == playerController.connectID) 
                {
                    lobbyInfo.playerName = playerController.playerSteamName;
                    lobbyInfo.playerReady = playerController.ready;

                    //test only
                    lobbyInfo.testMode = playerController.charModel;
                    lobbyInfo.SetPlayerInfo();
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerInfo() 
    {
        List<PlayerLobbyInfo> playerLobbyInfosToRemove = new List<PlayerLobbyInfo>();

        foreach (PlayerLobbyInfo lobbyInfo in playerLobbyInfos)
        {
            if (!networkManager.players.Any(b => b.connectID == lobbyInfo.connectionID))
                playerLobbyInfosToRemove.Add(lobbyInfo);
        }

        if (playerLobbyInfosToRemove.Count > 0) 
        {
            foreach (PlayerLobbyInfo lobbyInfo in playerLobbyInfosToRemove) 
            {
                GameObject gameObject2Remove = lobbyInfo.gameObject;
                playerLobbyInfos.Remove(lobbyInfo);
                Destroy(gameObject2Remove);
                gameObject2Remove = null;
            }
        }
    }
}
