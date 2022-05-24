using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelButton : MonoBehaviour
{

    private GameObject model;
    private UnitCreator uc;
    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        uc = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>();
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(AddModel);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetModel(GameObject c)
    {
        model = c;
    }

    private void AddModel()
    {
        uc.AddModel(model);
    }
}
