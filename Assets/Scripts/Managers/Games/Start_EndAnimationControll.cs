using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_EndAnimationControll : NetworkBehaviour
{
    public GameObject placementCardPrefab;
    public GameObject playerList;
    public List<MinigameFinishPlacementCard> allCard = new List<MinigameFinishPlacementCard>();
    private CustomNetworkManager networkManager;
    private BaseGameManager gameManager;

    private CustomNetworkManager CustomNetworkManager
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
        PlayerCanMove(false);
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BaseGameManager>();
    }

    private void PlayerCanMove(bool canMove) 
    {
        foreach (PlayerController player in CustomNetworkManager.players)
        {
            player.gameObject.GetComponent<PlayerDataForMap>().canMove = canMove;
        }
    }

    [ClientRpc]
    public void CreatePlacementCards() 
    {
        for (int i = 0; i < gameManager.cardList.Count; i++)
        {
            GameObject card = gameManager.cardList[i];
            string placement = card.GetComponent<PlayerCharacterMinigameCard>().placement.text;

            allCard[i].SetPlacement(placement);
            allCard[i].gameObject.SetActive(true);

            gameManager.allPlayerReadyImg[i].gameObject.SetActive(true);
            gameManager.allPlayerReadyImg[i].color = Color.red;
        }
    }

    public void StartAniFinished() 
    {
        StartCoroutine(StartFinished());
    }

    IEnumerator StartFinished()
    {
        PlayerCanMove(true);
        yield return new WaitUntil(() => AllPlayerCanMove());
        gameManager.gameStart = true;

        StartAniHelper();
        gameObject.SetActive(false);
    }

    [ServerCallback]
    private void StartAniHelper() 
    {
        ChangeState msg = new ChangeState
        {
            gameState = GameState.playing
        };

        CustomNetworkManager.ChangeMiniGameState(msg);
    }

    private bool AllPlayerCanMove() 
    {
        foreach (PlayerController player in CustomNetworkManager.players)
        {
            if (!player.gameObject.GetComponent<PlayerDataForMap>().canMove)
                return false;
        } 
        return true;
    }
}
