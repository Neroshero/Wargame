using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour
{

    public GameObject unitInput;
    private BattleGroupsCreator BGS;
    private InputField toName;
    private GameObject input;

    // Start is called before the first frame update
    void Start()
    {
        BGS = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateInput()
    {
        if (toName == null)
        {
            input = Instantiate(unitInput);
            input.transform.SetParent(GameObject.Find("Canvas").transform, false);
            toName = input.GetComponent<InputField>();
        }
    }

    public void RenameUnit()
    {
        if (toName.text != "")
        {
            BGS.GetActiveUnit().GetComponent<UnitCreator>().Rename(toName.text);
        }
        toName = null;
    }

    public GameObject GetInput()
    {
        return input;
    }

    public void Close()
    {
        GameObject active = BGS.GetActiveUnit();
        active.GetComponent<UnitCreator>().GetPanel().SetActive(false);
        active.GetComponent<UnitCreator>().GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
        BGS.RemoveActive();
    }

    public void Add()
    {
        //toDo
    }
}
