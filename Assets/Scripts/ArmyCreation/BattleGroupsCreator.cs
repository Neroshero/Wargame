using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGroupsCreator : MonoBehaviour
{

    private int cost;
    private List<string> roles;
    private bool legal;
    public new string name;
    private string origName;

    private GameObject panel;
    private GameObject button;
    private GameObject active;
    private GameObject recent;
    private GameObject activeUnit;

    private GameObject infPanel;
    private GameObject cmdPanel;
    private GameObject hvyPanel;
    private GameObject supPanel;
    private GameObject qukPanel;
    private GameObject astPanel;

    [SerializeField]
    private GameObject unitPanel;

    private List<GameObject> units;

    [SerializeField]
    private List<int> required;
    [SerializeField]
    private List<int> possible;

    // Start is called before the first frame update
    void Awake()
    {
        legal = false;
        roles = new List<string> {"Command", "Infantry", "Heavy", "Support", "Assault", "Quick"};
        origName = name;
        units = new List<GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddUnit(GameObject toAdd)
    {
        GameObject unit = Instantiate(toAdd);
        units.Add(unit);
        UnitCreator uc = unit.GetComponent<UnitCreator>();
        int role = roles.IndexOf(uc.GetRole());
        recent = unit;
        GameObject button = Instantiate(uc.GetButton());
        uc.SetRelButton(button);
        if (role == 0)
        {
            button.transform.SetParent(cmdPanel.transform, false);
        }
        else if (role == 1)
        {
            button.transform.SetParent(infPanel.transform, false);
        }

        else if (role == 2)
        {
            button.transform.SetParent(hvyPanel.transform, false);
        }
        else if (role == 3)
        {
            button.transform.SetParent(supPanel.transform, false);
        }
        else if (role == 4)
        {
            button.transform.SetParent(astPanel.transform, false);
        }
        else
        {
            button.transform.SetParent(qukPanel.transform, false);
        }
        GameObject temp = Instantiate(unitPanel);
        temp.transform.SetParent(GameObject.Find("Canvas").transform, false);
        temp.transform.Find("Viewport").Find("Unit Name").GetComponent<UnityEngine.UI.Text>().text = uc.GetName();
        uc.SetPanel(temp);
        SetActiveUnit(unit);
        required[role] = required [role]-1;
        possible[role] = possible[role] -1;
        if (required[role] <= 0 && possible[role]>=0) legal = true;
    }

    public List<GameObject> GetUnits()
    {
        return units;
    }

    public string GetName()
    {
        return name;
    }

    public void ChangeName(string toChange)
    {
        name = toChange;
    }

    public void SetPanel(GameObject toSet)
    {
        panel = toSet;
        infPanel = panel.transform.Find("GreaterInf").Find("Viewport").Find("InfPanel").gameObject;
        cmdPanel = panel.transform.Find("GreaterCmd").Find("Viewport").Find("CmdPanel").gameObject;
        hvyPanel = panel.transform.Find("GreaterHvy").Find("Viewport").Find("HvyPanel").gameObject;
        supPanel = panel.transform.Find("GreaterSup").Find("Viewport").Find("SupPanel").gameObject;
        qukPanel = panel.transform.Find("GreaterQuk").Find("Viewport").Find("QukPanel").gameObject;
        astPanel = panel.transform.Find("GreaterAst").Find("Viewport").Find("AstPanel").gameObject;

    }

    public GameObject GetPanel()
    {
        return panel;
    }

    public void SetButton(GameObject toSet)
    {
        button = toSet;
    }

    public GameObject GetButton()
    {
        return button;
    }

    public void SetActive(GameObject toSet)
    {
        active = toSet;
    }
    
    public void SetActiveUnit(GameObject toSet)
    {
        if(activeUnit != null)
        {
            activeUnit.GetComponent<UnitCreator>().GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeUnit.GetComponent<UnitCreator>().GetPanel().SetActive(false);
        }
        activeUnit = toSet;
        if (activeUnit != null)
        {
            activeUnit.GetComponent<UnitCreator>().GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = false;
            activeUnit.GetComponent<UnitCreator>().GetPanel().SetActive(true);
        }
    }
   
    public GameObject GetActiveUnit()
    {
        return activeUnit;
    }
    
    public GameObject GetActive()
    {
        return active;
    }

    public GameObject GetRecent()
    {
        return recent;
    }

    public void RemoveActive()
    {
        if (activeUnit != null){
            activeUnit.GetComponent<UnitCreator>().GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeUnit.GetComponent<UnitCreator>().GetPanel().SetActive(false);
        }
        activeUnit = null;
    }

    public void UpdateCost(int cost)
    {
        this.cost += cost;
        GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().UpdateCost(cost);
    }

    public string GetOrigName()
    {
        return origName;
    }
}
