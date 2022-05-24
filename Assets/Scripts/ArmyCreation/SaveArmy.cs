using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveArmy : MonoBehaviour
{
    [SerializeField]
    private List<string> FactionsA;
    private Dictionary<string, string> Factions;
    [SerializeField]
    private List<string> BattlegroupsA;
    private Dictionary<string, string> Battlegroups;
    [SerializeField]
    private List<string> ArmyUnitsA;
    [SerializeField]
    private List<string> CorpUnitsB;
    [SerializeField]
    private List<string> CoAUnitsB;
    [SerializeField]
    private List<string> RebUnitsB;
    private Dictionary<string, string> ArmyUnits;

    private float displayTime;
    
    //toDo: Dictionaries for each model of each faction leading to each unit :( (maybe find something easier, maybe piggyback off the existing available models list in each unit)
    private Dictionary<string, string> CorpUnits;
    private Dictionary<string, string> RebUnits;
    private Dictionary<string, string> CoAUnits;
    

    // Start is called before the first frame update
    void Start()
    {
        DirectoryInfo armiesPath;
        try
        {
            armiesPath = new DirectoryInfo(Application.dataPath + "\\Armies");
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(Application.dataPath + "\\Armies");
        }
        Factions = new Dictionary<string, string>();
        foreach (string i in FactionsA)
        {
            Factions.Add(i, FactionsA.IndexOf(i).ToString());
        }

        Battlegroups = new Dictionary<string, string>();
        foreach (string i in BattlegroupsA)
        {
            Battlegroups.Add(i, BattlegroupsA.IndexOf(i).ToString());
        }

        ArmyUnits = new Dictionary<string, string>();
        foreach (string i in ArmyUnitsA)
        {
            ArmyUnits.Add(i, ArmyUnitsA.IndexOf(i).ToString());
        }
        CorpUnits = new Dictionary<string, string>();
        foreach (string i in CorpUnitsB)
        {
            CorpUnits.Add(i, CorpUnitsB.IndexOf(i).ToString());
        }
        CoAUnits = new Dictionary<string, string>();
        foreach (string i in CoAUnitsB)
        {
            CoAUnits.Add(i, CoAUnitsB.IndexOf(i).ToString());
        }
        RebUnits = new Dictionary<string, string>();
        foreach (string i in RebUnitsB)
        {
            RebUnits.Add(i, RebUnitsB.IndexOf(i).ToString());
        }
        displayTime = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (displayTime != -1) {
            if (displayTime > 0)
            {
                displayTime -= Time.deltaTime;
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("SaveUnderway").transform.Find("SaveComplete").gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.Find("SaveUnderway").gameObject.SetActive(false);
                displayTime = -1;
            } 
        }
    }

    public void Save(string input)
    {
        GameObject SU = GameObject.Find("Canvas").transform.Find("SaveUnderway").gameObject;
        SU.SetActive(true);
        SU.transform.Find("SaveInProgress").gameObject.SetActive(true);
        ArmyCreator AC = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>();
        string Faction = AC.GetFac();
        string path = Application.streamingAssetsPath + "/Armies/" + Faction + "/" + input + ".txt";
        string toSave = Factions[Faction] + "\n";
        List<GameObject> allGroups = AC.GetBattleGroups();
        allGroups.RemoveAll(item => item == null);
        toSave = toSave + allGroups.Count.ToString() + "\n";
        foreach (GameObject g in allGroups)
        {
            BattleGroupsCreator currGroup = g.GetComponent<BattleGroupsCreator>();
            toSave = toSave + Battlegroups[currGroup.GetOrigName()] + "\n";
            List<GameObject> currUnits = currGroup.GetUnits();
            toSave = toSave + currUnits.Count.ToString() + "\n";
            foreach (GameObject u in currUnits)
            {
                UnitCreator currUnit = u.GetComponent<UnitCreator>();
                if (Faction == "Army")
                {
                    toSave = toSave + ArmyUnits[currUnit.GetOrigName()] + "\n";
                    List<GameObject> currModels = currUnit.SaveUnit();
                    toSave = toSave + currModels.Count.ToString() + "\n";
                    List<string> modelChecker = new List<string>(currUnit.GetLegalModels().Count);
                    foreach(ModelCreator c in currUnit.GetLegalModels())
                    {
                        modelChecker.Add(c.GetName());
                    }
                    foreach (GameObject m in currModels)
                    {
                        //do the things I need to do later
                        toSave = toSave + modelChecker.IndexOf(m.GetComponent<ModelCreator>().GetName()) + "\n";
                    }
                }
                else if (Faction == "Corp")
                {
                    toSave = toSave + CorpUnits[currUnit.GetOrigName()] + "\n";
                    List<GameObject> currModels = currUnit.SaveUnit();
                    toSave = toSave + currModels.Count.ToString() + "\n";
                    List<string> modelChecker = new List<string>(currUnit.GetLegalModels().Count);
                    foreach (ModelCreator c in currUnit.GetLegalModels())
                    {
                        modelChecker.Add(c.GetName());
                    }
                    foreach (GameObject m in currModels)
                    {
                        //do the things I need to do later
                        toSave = toSave + modelChecker.IndexOf(m.GetComponent<ModelCreator>().GetName()) + "\n";
                    }
                }
                else if (Faction == "CoA")
                {
                    toSave = toSave + CoAUnits[currUnit.GetOrigName()] + "\n";
                    List<GameObject> currModels = currUnit.SaveUnit();
                    toSave = toSave + currModels.Count.ToString() + "\n";
                    List<string> modelChecker = new List<string>(currUnit.GetLegalModels().Count);
                    foreach (ModelCreator c in currUnit.GetLegalModels())
                    {
                        modelChecker.Add(c.GetName());
                    }
                    foreach (GameObject m in currModels)
                    {
                        //do the things I need to do later
                        toSave = toSave + modelChecker.IndexOf(m.GetComponent<ModelCreator>().GetName()) + "\n";
                    }
                }
                else
                {
                    toSave = toSave + RebUnits[currUnit.GetOrigName()] + "\n";
                    List<GameObject> currModels = currUnit.SaveUnit();
                    toSave = toSave + currModels.Count.ToString() + "\n";
                    List<string> modelChecker = new List<string>(currUnit.GetLegalModels().Count);
                    foreach (ModelCreator c in currUnit.GetLegalModels())
                    {
                        modelChecker.Add(c.GetName());
                    }
                    foreach (GameObject m in currModels)
                    {
                        //do the things I need to do later
                        toSave = toSave + modelChecker.IndexOf(m.GetComponent<ModelCreator>().GetName()) + "\n";
                    }
                }
            }
        }
        try
        {
            DirectoryInfo test = new DirectoryInfo(Application.dataPath + "\\Armies\\" + Faction);
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(Application.dataPath + "\\Armies\\" + Faction);
        }
        File.WriteAllText(path, toSave);
        SU.transform.Find("SaveInProgress").gameObject.SetActive(false);
        SU.transform.Find("SaveComplete").gameObject.SetActive(true);
        displayTime = 2;
    }
}
