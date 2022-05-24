using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectButton : MonoBehaviour
{

    private BattleGroupsCreator BGS;
    private GameObject unit;

    // Start is called before the first frame update
    void Start()
    {
        BGS = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>();
        unit = BGS.GetRecent();
        gameObject.GetComponent<Button>().onClick.AddListener(SetActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetActive()
    {
        BGS.SetActiveUnit(unit);
    }
}
