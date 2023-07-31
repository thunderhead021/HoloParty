using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyInfo : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong steamID;
    private bool avatarRecived;

    public Text playerNameTxt;
    public RawImage avatarIcon;
    public Image charSelected;

    public GameObject playerReadyBox;
    public bool playerReady;

    protected Callback<AvatarImageLoaded_t> imageLoaded;


    // Start is called before the first frame update
    void Start()
    {
        imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        transform.localPosition = new Vector2(0, -315);
    }

    public void ChangeReadyStatus() 
    {
        playerReadyBox.SetActive(playerReady);
    }

	private void OnImageLoaded(AvatarImageLoaded_t avatarCall)
	{
        if (avatarCall.m_steamID.m_SteamID == steamID)
        {
            avatarIcon.texture = GetSteamAvatar(avatarCall.m_iImage);
        }
        else
            return;
	}

    private void GetPlayerIcon() 
    {
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)steamID);
        if (imageID == -1)
            return;
        avatarIcon.texture = GetSteamAvatar(imageID);
    }

    public void SetPlayerInfo() 
    {
        playerNameTxt.text = playerName;
        ChangeReadyStatus();
        if (!avatarRecived) 
        {
            GetPlayerIcon();
        }
    }

	private Texture2D GetSteamAvatar(int m_iImage)
	{
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(m_iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(m_iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarRecived = true;
        return texture;
    }
}
