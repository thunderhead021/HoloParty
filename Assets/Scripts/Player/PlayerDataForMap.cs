using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Experimental;

public class PlayerDataForMap : NetworkBehaviour
{

    public GameObject playerModel;
    [SerializeField]
    private string curMapName = "";
    private bool haveMapData = false;
    [SyncVar]
    public Vector3 playerBoardPos = Vector3.zero;
    [SyncVar]
    public bool notUpdate = false;

    [SyncVar]
    public bool isLose = false;

    [SyncVar]
    public bool canMove = true;

    [SyncVar]
    private BaseGameManager gameManager;

    private CustomNetworkManager networkManager;
    private CustomNetworkManager CustomNetworkManager
    {
        get
        {
            if (networkManager != null)
                return networkManager;
            return networkManager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerModel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (notUpdate && transform != null)
        {
            transform.position = playerBoardPos;
            notUpdate = false;
        }
        if (SceneManager.GetActiveScene().name.Contains("Map") && GetComponent<PlayerController>().ready) 
        {  
            if (!curMapName.Equals(SceneManager.GetActiveScene().name)) 
            {
                if (transform.childCount > 1)
                {
                    Destroy(transform.GetChild(1).gameObject);
                    RemoveRB();
                }

                GameObject mapData = GameObject.FindGameObjectWithTag("MapData");
                if (mapData != null && mapData.transform.root == mapData.transform && GetComponent<PlayerController>().charID >= 0)
                {
                    gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<BaseGameManager>();
                    GameObject duplicate = Instantiate(mapData);
                    duplicate.transform.SetParent(transform);
                    UpdatePlayerModel();
                    if (playerBoardPos != Vector3.zero && SceneManager.GetActiveScene().name.Contains("Board"))
                    {
                        notUpdate = true;
                    }
                    else 
                    {
                        transform.GetComponentInChildren<BaseMapData>().SetPostion(GetComponent<PlayerController>().connectID);
                        Transform model = FindModel();
                        model.localPosition = Vector3.zero;
                    }
                }
                else 
                {
                    haveMapData = false;
                    playerModel.SetActive(false);
                }
                
            }
            if (isOwned && haveMapData)
            {
                if (!isLose && canMove)
                {
                    if (gameManager?.gameStart ?? true)
                    {
                        transform.GetComponentInChildren<BaseMapData>()?.Movement();
                    }
                }
                    
            }
            if (isLose && FindModel() != null)
            {
                Transform model = FindModel();
                model.gameObject.SetActive(false);
                SetPlacement();
                RemoveRB();
            }
        }
    }

    public void SetPlacement() 
    {
        foreach (GameObject card in gameManager.cardList)
        {
            Debug.Log(card.GetComponent<PlayerCharacterMinigameCard>().playerId);
            if (card.GetComponent<PlayerCharacterMinigameCard>().playerId == GetComponent<PlayerController>().connectID)
            {
                card.GetComponent<PlayerCharacterMinigameCard>().SetPlactment(CustomNetworkManager.PlayerStanding(GetComponent<PlayerController>()));
                Debug.Log("placement:" + card.GetComponent<PlayerCharacterMinigameCard>().placement.text);
            }
        }
    }

    Transform FindModel()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Player")) return t;
        }
        return null;
    }

    public void UpdatePlayerModel() 
    {
        transform.GetComponentInChildren<BaseMapData>().SetMapModel(GetComponent<PlayerController>().charID, transform.GetChild(0).gameObject);
        curMapName = SceneManager.GetActiveScene().name;
        haveMapData = true;
        isLose = false;
        playerModel.SetActive(true);
        
    }

    public BaseMapData GetMapData() 
    {
        return transform.GetComponentInChildren<BaseMapData>();
    }

    public void AddRB(bool is2D) 
    {
		if (is2D)
		{
			gameObject.AddComponent<Rigidbody2D>();
			gameObject.GetComponent<Rigidbody2D>().angularDrag = 0;
			gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.GetComponent<NetworkRigidbody2D>().enabled = true;
            gameObject.GetComponent<NetworkRigidbody2D>().target = gameObject.GetComponent<Rigidbody2D>();
        }
		else
		{
			gameObject.AddComponent<Rigidbody>();
			gameObject.GetComponent<Rigidbody>().angularDrag = 0;
			gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<NetworkRigidbody>().enabled = true;
            gameObject.GetComponent<NetworkRigidbody>().target = gameObject.GetComponent<Rigidbody>();
        }
	}

    public void RemoveRB() 
    {
        Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Destroy(rb);
            gameObject.GetComponent<NetworkRigidbody>().enabled = false;
        }
        else if (rb2d != null) 
        {
            Destroy(rb2d);
            gameObject.GetComponent<NetworkRigidbody2D>().enabled = false;
        }

    }

}
