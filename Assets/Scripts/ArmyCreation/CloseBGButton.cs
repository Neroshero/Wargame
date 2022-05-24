using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseBGButton : MonoBehaviour
{
    Button btn;
    ArmyCreator AC;

    // Start is called before the first frame update
    void Start()
    {
        AC = GameObject.Find("ScriptHolder").GetComponent<ArmyCreator>();
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Close()
    {
        AC.Deactivate();
    }
}
