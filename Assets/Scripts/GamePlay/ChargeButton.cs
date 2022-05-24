using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeButton : MonoBehaviour
{

    private Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(StartCharge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartCharge()
    {
        unit.PrepareCharge();
    }

    public void SetUnit(Unit toSet)
    {
        unit = toSet;
    }
}
