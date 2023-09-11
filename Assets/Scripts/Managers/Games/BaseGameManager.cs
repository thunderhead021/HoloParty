using Mirror;
using UnityEngine;

public class BaseGameManager : NetworkBehaviour
{
    public GameObject countdown;
    public GameObject finish;
    public GameObject playerList;
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
    PlayerController winner;

    public SyncList<GameObject> cardList = new SyncList<GameObject>();

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
    }

	private void FixedUpdate()
	{
        switch (state)
        {
            case GameState.demo:
                ShowDemo();
                break;
            case GameState.countingDown:
                Countdown();
                break;
			case GameState.playing:
				if (GameIsEnded())
					state = GameState.finished;
				break;
			case GameState.finished:
				ShowGameResult();
				break;
		}
    }

    /// <summary>
    /// For executing before the minigame starts
    /// </summary>
    public virtual void ShowDemo() 
    {
        Debug.Log("Demo");
        state = GameState.countingDown;
    }

    /// <summary>
    /// For executing the countdown to start the minigame.
    /// </summary>
    public virtual void Countdown() {
        if (finishedCountdown)
            return;

        Debug.Log(CustomNetworkManager.players.Count);
        foreach (PlayerController player in CustomNetworkManager.players)
        {
			GameObject playerCard = Instantiate(playerCardPrefab);
			PlayerCharacterMinigameCard newPlayerInfo = playerCard.GetComponent<PlayerCharacterMinigameCard>();

			newPlayerInfo.SetCharImage(player.charID, player.connectID);
			playerCard.transform.SetParent(playerList.transform);
            playerCard.transform.localScale = Vector3.one;

            cardList.Add(playerCard);
            Debug.Log("Add player card");
        }
        playerList.SetActive(true);
        countdown.SetActive(true);
        Debug.Log("Countdown");
        finishedCountdown = true;
    }

    /// <summary>
    /// For executing the minigame's core gameplay loop.
    /// </summary>
    public virtual bool GameIsEnded()
    {
        return true;
    }

    internal bool LastManStandingMinigameTypeWinCondition() 
    {
        foreach (PlayerController player in CustomNetworkManager.players)
        {
            if (!player.gameObject.GetComponent<PlayerDataForMap>().isLose)
            {
                if (winner != null)
                {
                    winner = null;
                    return false;
                }
                else
                    winner = player;
            }
        }
        return true;
    }

    /// <summary>
    /// For executing after the minigame finishes
    /// </summary>
    public virtual void ShowGameResult() 
    {
        Debug.Log("End");
        finish.SetActive(true);
    }
}
