using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ArmySelect : MonoBehaviour
{
    public Dropdown currFac;
    public Dropdown currArmy;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Confrim()
    {
        if (currArmy.options[currArmy.value].text != "please select an army")
        {
            string army = currFac.options[currFac.value].text + "\\" + currArmy.options[currArmy.value].text;
            Hashtable toSet = new Hashtable()
            {
                { "Army", army }
            };
            Debug.Log(army);
            PhotonNetwork.LocalPlayer.SetCustomProperties(toSet);
        }
        else{
            Hashtable toSet = new Hashtable
            {
                { "Army", "" }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(toSet);
        }
    }
}