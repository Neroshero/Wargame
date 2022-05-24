using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField input = this.GetComponent<InputField>();
        if(input != null)
        {
            if (PlayerPrefs.HasKey("PlayerName"))
            {
                defaultName = PlayerPrefs.GetString("PlayerName");
                input.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetName(string toSet)
    {
        if (string.IsNullOrEmpty(toSet))
        {
            Debug.Log("Player name was null or Empty");
            return;
        }

        PhotonNetwork.NickName = toSet;
        PlayerPrefs.SetString("PlayerName", toSet);
    }

}
