using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SelectButton : MonoBehaviour
{

    private GameObject panel;
    private Button btn;
    private Army army;
    private GameObject unit;
    private int playerID;

    // Start is called before the first frame update
    void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Active);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Active()
    {
        if(army.GetPanel() != null){
            List<GameObject> temp = army.GetPanel();
            //if (temp.Count == 0) return;
            if (temp[2].GetComponent<Unit>().GetPlanningCharge()) return;
            temp[0].SetActive(false);
            temp[1].GetComponent<Button>().interactable = true;
            temp[2].GetComponent<Unit>().SetWoundsPanel(false);
            temp[2].GetComponent<Unit>().SetChargePanel(false);
        }
        panel.SetActive(true);
        btn.interactable = false;
        army.SetPanel(new List<GameObject> { panel, btn.gameObject, unit});
    }

    public void SetPanel(GameObject toSet, int toID, Unit toUnit)
    {
        panel = toSet;
        playerID = toID;
        unit = toUnit.gameObject;
        Debug.Log("I am a button for player: " + toID);
        if (PhotonView.Find(playerID*1000+1).gameObject != null) Debug.Log("I found my player");
        if (PhotonView.Find(playerID*1000+1).gameObject.GetComponent<PlayerControls>() != null) Debug.Log("I found my controls");
        if (PhotonView.Find(playerID*1000+1).gameObject.GetComponent<PlayerControls>().GetArmy() != null) Debug.Log("I found my army");
        army = PhotonView.Find(playerID*1000+1).gameObject.GetComponent<PlayerControls>().GetArmy();
        Debug.Log("Panel setup: " + army);
    }

}
