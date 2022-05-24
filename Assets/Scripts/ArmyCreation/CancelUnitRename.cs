using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelUnitRename : MonoBehaviour
{
    private GameObject input;
    Button btn;

    // Start is called before the first frame update
    void Start()
    {
        input = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>().GetPanel().transform.Find("ScriptHolder").
            GetComponent<UnitPanel>().GetInput();
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Cancel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Cancel()
    {
        Destroy(input);
    }
}
