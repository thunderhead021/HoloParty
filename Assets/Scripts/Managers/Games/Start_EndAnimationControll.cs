using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_EndAnimationControll : MonoBehaviour
{
    public GameObject placementCardPrefab;
    public GameObject playerList;

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

    public void CreatePlacementCards() 
    {
        foreach (GameObject card in gameManager.cardList) 
        {
            GameObject playerplacementCard = Instantiate(placementCardPrefab);
            MinigameFinishPlacemtnCard playerplacementCardInfo = playerplacementCard.GetComponent<MinigameFinishPlacemtnCard>();
            string placement = card.GetComponent<PlayerCharacterMinigameCard>().placement.text;
            playerplacementCardInfo.SetPlacement(placement);

            playerplacementCard.transform.SetParent(playerList.transform);
            playerplacementCard.transform.localScale = Vector3.one;
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
        gameManager.state = BaseGameManager.GameState.playing;
        gameObject.SetActive(false);
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
