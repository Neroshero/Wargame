using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseUnitPanel : MonoBehaviour
{
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
        btn.onClick.AddListener(ClosePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClosePanel()
    {
        BG.GetActive().SetActive(false);
    }
}
