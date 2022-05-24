using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCreator : MonoBehaviour
{

    private List<GameObject> models;

    [SerializeField]
    private List<ModelCreator> legalModels;
    [SerializeField]
    private List<ModelCreator> requiredModels;

    [SerializeField]
    private GameObject button;
    private GameObject relButton;

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    new private string name;
    private string origName;
    [SerializeField]
    private string role;
    [SerializeField]
    private int maxSize;

    private int cost;
    private bool legal;
    private bool character;

    private GameObject active;

    // Start is called before the first frame update
    void Start()
    {
        models = new List<GameObject>(maxSize);
        origName = name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddModel(GameObject model)
    {
        if (models.Count < maxSize)
        {
            GameObject rel = Instantiate(model);
            models.Add(rel);
            cost += model.GetComponent<ModelCreator>().GetCost();
            GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().UpdateCost(model.GetComponent<ModelCreator>().GetCost());
            Button btn = Instantiate(model.GetComponent<ModelCreator>().GetSelButton());
            btn.transform.SetParent(panel.transform.Find("Viewport").transform.Find("Content"), false);
            btn.transform.Find("Text").GetComponent<Text>().text = model.GetComponent<ModelCreator>().GetName();
            btn.GetComponent<ModelSelectButton>().SetToSelect(rel);
            rel.GetComponent<ModelCreator>().SetButton(btn);
        }
    }

    public void RemoveModel()
    {
        if (active != null)
        {
            models.Remove(active);
            cost -= active.GetComponent<ModelCreator>().GetCost();
            GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().UpdateCost(active.GetComponent<ModelCreator>().GetCost() * -1);
            Destroy(active.GetComponent<ModelCreator>().GetRelButton().gameObject);
            Destroy(active);
        }
    }

    public void SetActive(GameObject model)
    {
        if (active != null)
        {
            active.GetComponent<ModelCreator>().GetRelButton().interactable = true;
        }
        active = model;
        model.GetComponent<ModelCreator>().GetRelButton().interactable = false;
    }

    public GameObject GetActive()
    {
        return active;
    }

    public List<GameObject> GetModels()
    {
        return models;
    }


    public List<GameObject> SaveUnit()
    {
        models.TrimExcess();
        return models;
    }

    public bool GetLegal()
    {
        return legal;
    }

    public GameObject GetButton()
    {
        return button;
    }

    public string GetName()
    {
        return name;
    }

    public string GetRole()
    {
        return role;
    }

    public void Rename(string toName)
    {
        name = toName;
        panel.transform.Find("Viewport").Find("Unit Name").GetComponent<UnityEngine.UI.Text>().text = toName;
        relButton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = toName;
    }

    public void SetRelButton(GameObject toSet)
    {
        relButton = toSet;
        relButton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = name;
    }

    public GameObject GetRelButton()
    {
        return relButton;
    }

    public void SetPanel(GameObject toSet)
    {
        panel = toSet;
    }

    public GameObject GetPanel()
    {
        return panel;
    }

    public string GetOrigName()
    {
        return origName;
    }

    public List<ModelCreator> GetLegalModels()
    {
        return legalModels;
    }

}
