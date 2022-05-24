using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun; //this is likely a temporary solution; I imagine that having this many PhotonViews in the furture might risk being a bit much

public class Weapon : MonoBehaviourPun
{
    [SerializeField]
    private GameObject dropdown;
    private Dropdown relDropdown;

    [SerializeField]
    private GameObject textBox;

    [SerializeField]
    private GameObject btn;

    [SerializeField]
    new private string name;
    [SerializeField]
    private int attacks;
    [SerializeField]
    private int str;
    [SerializeField]
    private bool isMelee;

    private List<string> rangesA;
    [SerializeField]
    private List<int> rangesB;
    private Dictionary<string, int> rangeValues;

    [SerializeField]
    private int minDamage;
    [SerializeField]
    private int maxDamage;

    private int pointer;
    private int lastPointer;
    private Unit target;
    private List<GameObject> units;
    private Dictionary<int, GameObject> possTargets;

    // Start is called before the first frame update
    void Start()
    {
        rangesA = new List<string> { "melee", "point blank", "short", "medium", "long", "extreme" };
        rangeValues = new Dictionary<string, int>();
        for (int i = 0; i < rangesA.Count; i++) rangeValues.Add(rangesA[i], rangesB[i]);
        possTargets = new Dictionary<int, GameObject>
        {
            {0, null}
        };
        units = new List<GameObject>() { null };
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool IsMelee()
    {
        return isMelee;
    }

    public GameObject GetDropdown()
    {
        GameObject temp = Instantiate(dropdown);
        relDropdown = temp.GetComponent<Dropdown>();
        temp.GetComponent<WeaponsDropdown>().SetWeapon(this);
        relDropdown.AddOptions(new List<string>() {"No Target Selected"});
        return temp;
    }

    public GameObject GetNameBox()
    {
        GameObject temp = Instantiate(textBox);
        temp.GetComponent<Text>().text = "Target for " + name + ":";
        return temp;
    }

    public string GetName()
    {
        return name;
    }

    public void AddTargets(HashSet<GameObject> toAdd)
    {
        int temp = possTargets.Count;
        List<string> names = new List<string>();
        foreach (GameObject toDo in toAdd)
        {
            if (!units.Contains(toDo))
            {
                units.Add(toDo);
                names.Add(toDo.GetComponent<Unit>().GetName());
                possTargets.Add(temp, toDo);
                temp++;
            }
        }
        relDropdown.AddOptions(names);
    }

    public void AddTarget(GameObject toAdd)
    {
        if (!units.Contains(toAdd))
        {
            List<string> name = new List<string>
            {
                toAdd.GetComponent<Unit>().GetName()
            };
            units.Add(toAdd);
            possTargets.Add(possTargets.Count, toAdd);
            relDropdown.AddOptions(name);
        }
    }

    public void RemoveTarget(GameObject toRemove)
    {
        if (!units.Contains(toRemove)) return;
        for(int temp = units.IndexOf(toRemove); temp<possTargets.Count-1; temp++)
        {
            possTargets[temp] = possTargets[temp + 1];
        }
        possTargets.Remove(possTargets.Count-1);
        relDropdown.ClearOptions();
        List<string> toKeep = new List<string>()
        {
            "No Target Selected"
        };
        for (int i = 1; i < possTargets.Count - 1; i++) toKeep.Add(possTargets[i].GetComponent<Unit>().GetName());
        relDropdown.AddOptions(toKeep);
    }

    public bool Attack(int accValue, string range)
    {
        bool oneOrMoreHit = false;
        if (target != null)
        {
            //do what I said in RanAtt in model, I'll handle both types of attack here by sending the correct accuracy value here from the model
            for(int i = 0; i<attacks; i++) if (Random.Range(0, 201) + accValue >= 100) //maybe I'll do some percentage based math here later to try and make this more interesting
            {
                Debug.Log("Hit Target");
                target.photonView.RPC("AddHit", RpcTarget.All, photonView.ViewID);
                oneOrMoreHit = true;
            }
        }
        return oneOrMoreHit;
    }

    public GameObject GetButton()
    {
        return btn;
    }

    public void SetTarget()
    {
        target = possTargets[relDropdown.value].GetComponent<Unit>();
    }

    public Unit GetTarget()
    {
        return target;
    }

    public int GetStrength()
    {
        return str;
    }

    public List<int> GetDamage()
    {
        if (minDamage == maxDamage) return new List<int>() { minDamage };
        return new List<int>() { minDamage, maxDamage };
    }
}
