using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelCreator : MonoBehaviour
{
    [SerializeField]
    new private string name;
    [SerializeField]
    private int cost;

    [SerializeField]
    private int health;
    [SerializeField]
    private int str;
    [SerializeField]
    private int ranAcc;
    [SerializeField]
    private int melAcc;
    [SerializeField]
    private int melDef;
    [SerializeField]
    private int armor;

    [SerializeField]
    private List<Weapon> weapons;

    [SerializeField]
    private Button addButton;
    [SerializeField]
    private Button selectButton;
    private Button relButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCost()
    {
        return cost;
    }

    public void Rename(string toName)
    {
        name = toName;
    }

    public Button GetAddButton()
    {
        return addButton;
    }

    public string GetName()
    {
        return name;
    }

    public Button GetSelButton()
    {
        return selectButton;
    }

    public void SetButton(Button btn)
    {
        relButton = btn;
    }

    public Button GetRelButton()
    {
        return relButton;
    }
}
