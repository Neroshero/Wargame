using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSave : MonoBehaviour
{
    private GameObject input;
    Button btn;

    // Start is called before the first frame update
    void Start()
    {
        input = GameObject.Find("SavePanel(Clone)");
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(Confrim);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Confrim()
    {
        if (input.GetComponent<InputField>().text != "")
        {
            GameObject.Find("ScriptHolder").GetComponent<SaveArmy>().Save(input.GetComponent<InputField>().text);
        }
        GameObject.Find("SaveButton").GetComponent<Button>().interactable = true;
        Destroy(input);
    }
}
