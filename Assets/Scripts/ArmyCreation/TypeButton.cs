using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeButton : MonoBehaviour
{

    private Button btn;
    private GameObject AC;
    private ArmyCreator ACS;
    private GameObject BG;
    private BattleGroupsCreator BGS;
    private GameObject canvas;
    private GameObject panel;

    public GameObject CoA;
    public GameObject Reb;
    public GameObject Army;
    public GameObject Corp;
    public string Type;

    // Start is called before the first frame update
    void Awake()
    {
        AC = GameObject.Find("ScriptHolder");
        ACS = AC.GetComponent<ArmyCreator>();
        btn = this.gameObject.GetComponent<Button>();
        BG = ACS.GetRecent();
        BGS = BG.GetComponent<BattleGroupsCreator>();
        canvas = GameObject.Find("Canvas");
        btn.onClick.AddListener(OpenList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenList()
    {
        GameObject prev = BGS.GetActive();
        if(prev != null)
        {
            prev.SetActive(false);
        }
        if (panel == null)
        {
            string faction = ACS.GetFac();
            if (faction == "Army")
            {
                panel = Instantiate(Army);
            }
            else if (faction == "CoA")
            {
                panel = Instantiate(CoA);
            }
            else if (faction == "Corp")
            {
                panel = Instantiate(Corp);
            }
            else
            {
                panel = Instantiate(Reb);
            }
            panel.transform.SetParent(canvas.transform, false);
        }
        panel.SetActive(true);
        BGS.SetActive(panel);
    }
}
