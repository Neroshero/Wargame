using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmLoad : MonoBehaviour
{
    public Dropdown currFac;
    public Dropdown currArmy;
    public GameObject menu;
    Button btn;

    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("LoadMenu(Clone)");
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Confrim);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Confrim()
    {
        GameObject.Find("ScriptHolder").GetComponent<LoadArmy>().Load(currFac.options[currFac.value].text + "/" + currArmy.options[currArmy.value].text);
        GameObject.Find("LoadButton").GetComponent<Button>().interactable = true;
        Destroy(menu);
    }
}