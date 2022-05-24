using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButton : MonoBehaviour
{
    private Weapon weapon;
    private Army army;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SetActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeapon(Weapon toSet)
    {
        weapon = toSet;
        transform.Find("Text").GetComponent<Text>().text = toSet.GetName();
    }

    public void SetArmy(Army toSet)
    {
        army = toSet;
    }

    private void SetActive()
    {
        army.SetIncoming(weapon, gameObject);
    }
}
