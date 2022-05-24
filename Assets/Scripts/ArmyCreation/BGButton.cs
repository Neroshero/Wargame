using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGButton : MonoBehaviour
{

    private GameObject AC;
    private Button btn;
    private ArmyCreator ACS;
    private GameObject BG;

    // Start is called before the first frame update
    private void Awake()
    {
        AC = GameObject.Find("ScriptHolder");
        ACS = AC.GetComponent<ArmyCreator>();
        BG = ACS.GetRecent();
        btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(SetActive);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void SetActive()
    {
        ACS.SetActive(BG);
    }
}
