using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitRename : MonoBehaviour
{

    private UnitPanel panel;
    private Button btn;
    private GameObject input;

    // Start is called before the first frame update
    void Start()
    {
        panel = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>().GetPanel().transform.Find("ScriptHolder").
            GetComponent<UnitPanel>();
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Rename);
        input = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>().GetPanel().transform.Find("ScriptHolder").
            GetComponent<UnitPanel>().GetInput();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Rename()
    {
        panel.RenameUnit();
        Destroy(input);
    }

}
