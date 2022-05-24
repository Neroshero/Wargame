using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddModelButton : MonoBehaviour
{

    private UnitCreator uc;
    [SerializeField]
    private GameObject panel;
    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        uc = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>().GetActive().GetComponent<BattleGroupsCreator>().GetActiveUnit().GetComponent<UnitCreator>();
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OpenPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenPanel()
    {
        GameObject check = GameObject.Find("ModelPanel(Clone)");
        if (check == null)
        {
            List<ModelCreator> models = uc.GetLegalModels();
            GameObject toSet = Instantiate(panel);
            toSet.transform.SetParent(GameObject.Find("Canvas").transform, false);
            toSet = toSet.transform.Find("Viewport").transform.Find("Content").gameObject;
            foreach (ModelCreator c in models)
            {
                btn = Instantiate(c.GetAddButton());
                btn.transform.SetParent(toSet.transform, true);
                btn.GetComponent<ModelButton>().SetModel(c.gameObject);
                btn.transform.Find("Text").GetComponent<Text>().text = c.GetName();
            }
        }
    }
}
