using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ValueComparer : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        return x.GetComponent<PlayerCharacterMinigameCard>().playerId.CompareTo(y.GetComponent<PlayerCharacterMinigameCard>().playerId);
    }
}

public class BaseGameManager : NetworkBehaviour
{
    public GameObject countdown;
    public GameObject finish;
    public GameObject playerList;
    public List<PlayerCharacterMinigameCard> allCard = new List<PlayerCharacterMinigameCard>();
    public GameObject playerCardPrefab;

    internal CustomNetworkManager networkManager;

    [HideInInspector]
    public enum GameState
    {
        demo = 0,
        countingDown = 1,
        playing = 2,
        finished = 3,
    }

    [SyncVar]
    public GameState state;

    [SyncVar]
    private bool finishedCountdown = false;

    [SyncVar]
    [HideInInspector]
    public bool gameStart = false;

    [SyncVar]
    [HideInInspector]
    public bool gameEnd = false;

    [SyncVar]
    PlayerController winner;

    public readonly SyncList<GameObject> cardList = new SyncList<GameObject>();

    internal CustomNetworkManager CustomNetworkManager
    {
        get
        {
            if (networkManager != null)
                return networkManager;
            return networkManager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

	private void Start()
	{
        countdown.SetActive(false);
        finish.SetActive(false);
        playerList.SetActive(false);
        state = GameState.demo;
    }

	private void FixedUpdate()
	{
        switch (state)
        {
            case GameState.countingDown:
                Countdown();
                break;
			case GameState.playing:
                if (gameEnd)
                {
                    finishedCountdown = false;
                    state = GameState.finished;
                }
				break;
			case GameState.finished:
				ShowGameResult();
				break;
		}
    }

    /// <summary>
    /// For executing before the minigame starts
    /// </summary>
    [ServerCallback]
    public virtual void EndDemo() 
    {
        Debug.Log("Demo");
        state = GameState.countingDown;
    }

    /// <summary>
    /// For executing the countdown to start the minigame.
    /// </summary>
    [ServerCallback]
    public virtual void Countdown() {
        
    }


    [ClientRpc]
    internal void CountdownWithPlayerCards() 
    {
        if (finishedCountdown)
            return;

        playerList.SetActive(true);
        for (int i = 0; i < CustomNetworkManager.players.Count; i++) 
        {
            PlayerController player = CustomNetworkManager.players[i];
            allCard[i].SetCharImage(player.charID, player.connectID);
            allCard[i].gameObject.SetActive(true);
        }
        countdown.SetActive(true);
        Debug.Log("Countdown");
        finishedCountdown = true;
    }

    /// <summary>
    /// For executing the minigame's core gameplay loop.
    /// </summary>
    [ServerCallback]
    public virtual void GameIsEnded(){}

    [ClientRpc]
    internal void LastManStandingMinigameTypeWinCondition() 
    {
        foreach (PlayerController player in CustomNetworkManager.players)
        {
            if (!player.gameObject.GetComponent<PlayerDataForMap>().isLose)
            {
                if (winner != null)
                {
                    winner = null;
                    gameEnd = false;
                }
                else
                    winner = player;
            }
        }
        winner.gameObject.GetComponent<PlayerDataForMap>().SetPlacement();
        Debug.Log("End game");
        gameEnd = true;
    }

    /// <summary>
    /// For executing after the minigame finishes
    /// </summary>
    /// 
    [ServerCallback]
    public virtual void ShowGameResult() 
    {
        
    }

    [ClientRpc]
    internal void ShowResultHelper() 
    {
        if (finishedCountdown)
            return;

        //get the player in the order of 1st -> last
        List<GameObject> tmpList = new List<GameObject>();
        foreach (GameObject card in cardList)
        {
            tmpList.Add(card);
        }
        tmpList.Sort(new ValueComparer());
        cardList.Clear();
        foreach (GameObject card in tmpList)
        {
            cardList.Add(card);
        }


        Debug.Log("End");
        finish.SetActive(true);
        finishedCountdown = true;
    }
}
