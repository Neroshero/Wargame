using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyCreator : MonoBehaviour
{

    private List<string> factions;
    private string currFaction;
    private int cost;
    private int numGroups;
    private int activeGroup;
    private bool changesMade;

    public GameObject FacTxt;
    public GameObject CosTxt;
    public GameObject EmpArmy;
    public GameObject ArmyPanel;
    public GameObject UnitsPanel;
    public GameObject BGButton;
    public GameObject BGPanel;
    public GameObject MainPanel;
    private GameObject recent;

    public List<UnitCreator> CoAUnits;
    public List<UnitCreator> ArmyUnits;
    public List<UnitCreator> RebUnits;
    public List<UnitCreator> CorpUnits;
    private List<GameObject> Battlegroups;

    // Start is called before the first frame update
    void Start()
    {
        cost = 0;
        factions = new List<string>() { "Army", "Corp", "CoA", "Resistance" };
        currFaction = "Army";
        FacTxt.GetComponent<Text>().text = currFaction;
        CosTxt.GetComponent<Text>().text = cost.ToString();
        Battlegroups = new List<GameObject>(8) {null, null, null, null, null, null, null, null};
        activeGroup = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceFaction()
    {
        int curr = factions.IndexOf(currFaction);
        if (curr + 1 == factions.Count){
            currFaction = factions[0];
        }
        else
        {
            currFaction = factions[curr + 1];
        }
        FacTxt.GetComponent<Text>().text = currFaction;
        ClearArmy();
    }

    public void DecreaseFaction()
    {
        int curr = factions.IndexOf(currFaction);
        if (curr == 0)
        {
            currFaction = factions[factions.Count-1];
        }
        else
        {
            currFaction = factions[curr - 1];
        }
        FacTxt.GetComponent<Text>().text = currFaction;
        ClearArmy();
    }

    public string GetFac()
    {
        return currFaction;
    }

    public void SetFac(string toSet)
    {
        currFaction = toSet;
        FacTxt.GetComponent<Text>().text = toSet;
    }

    public List<UnitCreator> GetPossibleUnits()
    {
        if (currFaction == "Army") return ArmyUnits;
        if (currFaction == "CoA") return CoAUnits;
        if (currFaction == "Corp") return CorpUnits;
        else return RebUnits;
    }

    public void CreateBattleGroup(GameObject toCreate)
    {
        if (numGroups < 8)
        {
            for (int i = 0; i < 8; i++)
            {
                if (Battlegroups[i] == null){
                    GameObject group = Instantiate(toCreate);
                    Battlegroups[i] = group;
                    numGroups++;
                    if (numGroups == 1)
                    {
                        EmpArmy.SetActive(false);
                    }
                    recent = group;
                    GameObject button = Instantiate(BGButton) as GameObject;
                    button.transform.SetParent(ArmyPanel.transform, false);
                    if (i <= 3)
                    {
                        button.transform.localPosition = new Vector3(button.transform.localPosition.x - 479,
                            button.transform.localPosition.y + 191 - 160 * (i));
                    }
                    else
                    {
                        button.transform.localPosition = new Vector3(button.transform.localPosition.x + 479,
                            button.transform.localPosition.y + 191 - 160 * (i - 4));
                    }
                    button.transform.Find("Text").GetComponent<Text>().text = group.GetComponent<BattleGroupsCreator>().GetName();
                    GameObject panel = Instantiate(BGPanel);
                    panel.transform.SetParent(MainPanel.transform, false);
                    group.GetComponent<BattleGroupsCreator>().SetPanel(panel);
                    group.GetComponent<BattleGroupsCreator>().SetButton(button);
                    SetActive(group);
                    break;
                }
            }
        }
    }

    public void SetActive(GameObject toSet)
    {
        if (activeGroup != -1)
        {
            GameObject curr = Battlegroups[activeGroup];
            curr.GetComponent<BattleGroupsCreator>().GetPanel().SetActive(false);
            curr.GetComponent<BattleGroupsCreator>().GetButton().GetComponent<Button>().interactable = true;
            GameObject temp = curr.GetComponent<BattleGroupsCreator>().GetActive();
            if (temp != null) temp.SetActive(false);
            curr.GetComponent<BattleGroupsCreator>().RemoveActive();
        }
        activeGroup = Battlegroups.IndexOf(toSet);
        toSet.GetComponent<BattleGroupsCreator>().GetPanel().SetActive(true);
        toSet.GetComponent<BattleGroupsCreator>().GetButton().GetComponent<Button>().interactable = false;
    }

    public void RemoveGroup()
    {
        if (activeGroup != -1)
        {
            GameObject curr = Battlegroups[activeGroup];
            Battlegroups[activeGroup] = null;
            Destroy(curr.GetComponent<BattleGroupsCreator>().GetPanel());
            Destroy(curr.GetComponent<BattleGroupsCreator>().GetButton());
            GameObject temp = curr.GetComponent<BattleGroupsCreator>().GetActive();
            if (temp != null) Destroy(temp);
            foreach (GameObject i in curr.GetComponent<BattleGroupsCreator>().GetUnits())
            {
                foreach (GameObject x in i.GetComponent<UnitCreator>().GetModels())
                {
                    Destroy(x);
                }
                Destroy(i.GetComponent<UnitCreator>().GetPanel());
                Destroy(i);
            }
            Destroy(curr);
            activeGroup = -1;
            numGroups--;
            if (numGroups == 0) EmpArmy.SetActive(true);
        }
    }

    public void RemoveGroup(GameObject toRemove)
    {
        if (toRemove != null)
        {
            Destroy(toRemove.GetComponent<BattleGroupsCreator>().GetPanel());
            Destroy(toRemove.GetComponent<BattleGroupsCreator>().GetButton());
            GameObject temp = toRemove.GetComponent<BattleGroupsCreator>().GetActive();
            if (temp != null) Destroy(temp);
            foreach (GameObject i in toRemove.GetComponent<BattleGroupsCreator>().GetUnits())
            {
                foreach (GameObject x in i.GetComponent<UnitCreator>().GetModels())
                {
                    cost -= x.GetComponent<ModelCreator>().GetCost();
                    Destroy(x);
                }
                Destroy(i.GetComponent<UnitCreator>().GetPanel());
                Destroy(i);
            }
            Destroy(toRemove);
            activeGroup = -1;
            numGroups--;
            if (numGroups == 0) EmpArmy.SetActive(true);
        }
    }

    public GameObject GetActive()
    {
        return Battlegroups[activeGroup];
    }

    public GameObject GetRecent()
    {
        return recent;
    }

    public void UpdateCost(int cost)
    {
        this.cost += cost;
        CosTxt.GetComponent<Text>().text = this.cost.ToString();
    }

    public List<GameObject> GetBattleGroups()
    {
        return Battlegroups;
    }

    public void Deactivate()
    {
        GameObject curr = Battlegroups[activeGroup];
        curr.GetComponent<BattleGroupsCreator>().GetPanel().SetActive(false);
        curr.GetComponent<BattleGroupsCreator>().GetButton().GetComponent<Button>().interactable = true;
        GameObject temp = curr.GetComponent<BattleGroupsCreator>().GetActive();
        if (temp != null) temp.SetActive(false);
        curr.GetComponent<BattleGroupsCreator>().RemoveActive();
        activeGroup = -1;
    }

    public void ClearArmy()
    {
        foreach(GameObject x in Battlegroups)
        {
            RemoveGroup(x);
        }
        Battlegroups = new List<GameObject>(8) { null, null, null, null, null, null, null, null };
    }
}
