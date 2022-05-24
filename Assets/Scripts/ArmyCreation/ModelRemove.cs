using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelRemove : MonoBehaviour
{

    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Remove);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Remove()
    {
        GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>().RemoveModel();
    }
}
