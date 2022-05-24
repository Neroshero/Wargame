using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadArmy : MonoBehaviour
{
    [SerializeField]
    private List<string> FactionsB;
    private Dictionary<string, string> Factions;
    [SerializeField]
    private List<GameObject> BattleGroupsB;
    private Dictionary<string, GameObject> Battlegroups;
    [SerializeField]
    private List<GameObject> ArmyUnitsB;
    [SerializeField]
    private List<GameObject> CorpUnitsB;
    [SerializeField]
    private List<GameObject> CoAUnitsB;
    [SerializeField]
    private List<GameObject> RebUnitsB;
    private Dictionary<string, GameObject> ArmyUnits;
    //toDo: Dictionaries for each unit of each faction leading to each model :( (maybe find something easier, maybe piggyback off of the existing available models list in each?)
    private Dictionary<string, GameObject> CorpUnits;
    private Dictionary<string, GameObject> RebUnits;
    private Dictionary<string, GameObject> CoAUnits;

    // Start is called before the first frame update
    void Start()
    {
        Factions = new Dictionary<string, string>();
        foreach(string i in FactionsB)
        {
            Factions.Add(FactionsB.IndexOf(i).ToString(), i);
        }
        Battlegroups = new Dictionary<string, GameObject>();
        foreach(GameObject i in BattleGroupsB)
        {
            Battlegroups.Add(BattleGroupsB.IndexOf(i).ToString(), i);
        }
        ArmyUnits = new Dictionary<string, GameObject>();
        foreach(GameObject i in ArmyUnitsB)
        {
            ArmyUnits.Add(ArmyUnitsB.IndexOf(i).ToString(), i);
        }
        CorpUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in CorpUnitsB)
        {
            CorpUnits.Add(CorpUnitsB.IndexOf(i).ToString(), i);
        }
        CoAUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in CoAUnitsB)
        {
            CoAUnits.Add(CoAUnitsB.IndexOf(i).ToString(), i);
        }
        RebUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in RebUnitsB)
        {
            RebUnits.Add(RebUnitsB.IndexOf(i).ToString(), i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(string input)
    {
        StartCoroutine(ActualLoad(input));
    }
    private IEnumerator ActualLoad(string input)
    {
        string path = Application.streamingAssetsPath + "/Armies/" + input + ".txt";
        ArmyCreator AC = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>();
        AC.ClearArmy();
        string[] toLoad = File.ReadAllLines(path);
        int pointer = 0;
        string faction = Factions[toLoad[pointer]];
        AC.SetFac(faction);
        pointer++;
        int numGroups = int.Parse(toLoad[pointer]);
        pointer++;
        for(int i = 0; i<numGroups; i++)
        {
            AC.CreateBattleGroup(Battlegroups[toLoad[pointer]]);
            pointer++;
            int numUnits = int.Parse(toLoad[pointer]);
            pointer++;
            if (faction == "Army") {
                for (int x = 0; x < numUnits; x++)
                {
                    AC.GetActive().GetComponent<BattleGroupsCreator>().AddUnit(ArmyUnits[toLoad[pointer]]);
                    pointer++;
                    int numModels = int.Parse(toLoad[pointer]);
                    pointer++;
                    UnitCreator AU = AC.GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>();
                    List<ModelCreator> modelChecker = AU.GetLegalModels();
                    yield return new WaitForEndOfFrame();
                    for (int c = 0; c<numModels; c++)
                    {
                        AU.AddModel(modelChecker[int.Parse(toLoad[pointer])].gameObject);
                        pointer++;
                    }
                }
            }else if(faction == "Corp")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    AC.GetActive().GetComponent<BattleGroupsCreator>().AddUnit(CorpUnits[toLoad[pointer]]);
                    pointer++;
                    int numModels = int.Parse(toLoad[pointer]);
                    pointer++;
                    UnitCreator AU = AC.GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>();
                    List<ModelCreator> modelChecker = AU.GetLegalModels();
                    yield return new WaitForEndOfFrame();
                    for (int c = 0; c < numModels; c++)
                    {
                        AU.AddModel(modelChecker[int.Parse(toLoad[pointer])].gameObject);
                        pointer++;
                    }
                }
            }
            else if(faction == "CoA")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    AC.GetActive().GetComponent<BattleGroupsCreator>().AddUnit(CoAUnits[toLoad[pointer]]);
                    pointer++;
                    int numModels = int.Parse(toLoad[pointer]);
                    pointer++;
                    UnitCreator AU = AC.GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>();
                    List<ModelCreator> modelChecker = AU.GetLegalModels();
                    yield return new WaitForEndOfFrame();
                    for (int c = 0; c < numModels; c++)
                    {
                        AU.AddModel(modelChecker[int.Parse(toLoad[pointer])].gameObject);
                        pointer++;
                    }
                }
            }
            else
            {
                for (int x = 0; x < numUnits; x++)
                {
                    AC.GetActive().GetComponent<BattleGroupsCreator>().AddUnit(RebUnits[toLoad[pointer]]);
                    pointer++;
                    int numModels = int.Parse(toLoad[pointer]);
                    pointer++;
                    UnitCreator AU = AC.GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>();
                    List<ModelCreator> modelChecker = AU.GetLegalModels();
                    yield return new WaitForEndOfFrame();
                    for (int c = 0; c < numModels; c++)
                    {
                        AU.AddModel(modelChecker[int.Parse(toLoad[pointer])].gameObject);
                        pointer++;
                    }
                }
            }
        }
        AC.GetActive().GetComponent<BattleGroupsCreator>().RemoveActive();
        AC.Deactivate();
    }
}
