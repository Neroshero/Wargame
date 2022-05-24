using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSelectButton : MonoBehaviour
{

    private Model model;
    private Army army;
    private Button btn;
    private GameController GC;
    [SerializeField]
    private Material selected;

    // Start is called before the first frame update
    void Start()
    {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Select);
        GC = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Select()
    {
        if (GC.GetGamePhase() == 0 && !GC.GetDeployed(army.GetPlayer() - 1) ||
            GC.GetGamePhase() == 7 && !model.GetUnit().CheckActivated() ||
            GC.GetGamePhase() == 5 && model.GetUnit().GetInMelee() && GC.GetPlayerTurn() == army.GetPlayer() && !model.GetUnit().CheckActivated() ||
            GC.GetGamePhase() != 0 && GC.GetPlayerTurn() == army.GetPlayer() && !model.GetUnit().CheckActivated())
        {
            if (army.GetActive() != null)
            {
                Model model = army.GetActive().GetActive();
                if (model != null)
                {
                    GameObject temp = army.GetActive().GetActive().GetRelButton();
                    temp.GetComponent<Button>().interactable = true;
                    temp.GetComponent<Image>().color = Color.white;
                }
            }
            army.SetActive(model.GetUnit(), model);
            btn.interactable = false;
            GetComponent<Image>().color = Color.yellow;
        }
    }

    public void SetModel(Model toSet, Army toArmy)
    {
        army = toArmy;
        Debug.Log(army);
        model = toSet;
    }
}
