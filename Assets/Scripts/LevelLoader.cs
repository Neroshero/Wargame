using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelLoader : MonoBehaviourPunCallbacks
{

    public GameObject connected;

    public GameObject loading;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player: " + newPlayer.NickName + " has joined as player: " +newPlayer.ActorNumber);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            connected.SetActive(false);
            loading.SetActive(true);
            PhotonNetwork.LoadLevel(3);
        }
    }
}
