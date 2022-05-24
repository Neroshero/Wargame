using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitsButton : MonoBehaviour
{
    private Army army;
    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Confirm);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Confirm()
    {
        army.ConfirmHit();
    }

    public void SetArmy(Army toSet)
    {
        army = toSet;
    }
}
