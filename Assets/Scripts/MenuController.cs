using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MenuController : MonoBehaviourPunCallbacks
{

    private bool isConnecting;
    private string gameVersion = "1";

    public List<GameObject> UI;
    public GameObject connectingStatus;

    private void Awake()
    {
        isConnecting = false;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        connectingStatus.SetActive(false);
        foreach (GameObject toDo in UI) toDo.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        if (!string.IsNullOrEmpty((string)PhotonNetwork.LocalPlayer.CustomProperties["Army"]))
        {
            Debug.Log("PlayerNumber at Connect: " + PhotonNetwork.LocalPlayer.ActorNumber);
            connectingStatus.SetActive(true);
            foreach (GameObject toDo in UI) toDo.SetActive(false);
            GameObject.Find("FindGame").SetActive(false);
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        else Debug.Log("Connect failed due to no army");
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnect due to {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadBattleSelect()
    {
        SceneManager.LoadScene("BattleSelect");
    }

    public void LoadArmyMaker()
    {
        SceneManager.LoadScene("ArmyCreator");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnJoinedRoom()
    {
        /*
        Hashtable toSet = new Hashtable()
            {
                {PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.ActorNumber}
            };
        PhotonNetwork.CurrentRoom.SetCustomProperties(toSet);
        Debug.Log(toSet);
        */
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            SceneManager.LoadScene("BattleSelect");
        }
    }
}
