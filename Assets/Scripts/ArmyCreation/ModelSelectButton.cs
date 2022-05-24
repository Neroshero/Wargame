using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSelectButton : MonoBehaviour
{

    private GameObject toSelect;
    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(SetActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetActive()
    {
        GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>().SetActive(toSelect);
    }

    public void SetToSelect(GameObject toSet)
    {
        toSelect = toSet;
    }
}
