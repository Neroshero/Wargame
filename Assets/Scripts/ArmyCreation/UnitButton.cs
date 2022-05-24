using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{

    public GameObject unit;
    private BattleGroupsCreator BG;
    private GameObject AC;
    private Button btn;
    private ArmyCreator ACS;

    // Start is called before the first frame update
    private void Awake()
    {
        AC = GameObject.Find("ScriptHolder");
        ACS = AC.GetComponent<ArmyCreator>();
        BG = ACS.GetActive().GetComponent<BattleGroupsCreator>();
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(AddUnit);
        btn.transform.Find("Text").gameObject.GetComponent<Text>().text = unit.GetComponent<UnitCreator>().GetName();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void AddUnit()
    {
        BG.AddUnit(unit);
    }
}
