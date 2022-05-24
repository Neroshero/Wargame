using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsDropdown : MonoBehaviour
{

    private Weapon weapon;
    private Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate
        {
            SetTarget();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeapon(Weapon toSet)
    {
        weapon = toSet;
    }

    private void SetTarget()
    {
        weapon.SetTarget();
    }
}
