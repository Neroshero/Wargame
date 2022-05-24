using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchSave : MonoBehaviour
{
    public GameObject savePanel;
    Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(launch);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void launch()
    {
        Instantiate(savePanel).transform.SetParent(GameObject.Find("Canvas").transform, false);
        btn.interactable = false;
    }
}
