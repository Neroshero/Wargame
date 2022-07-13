using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using Photon.Realtime;


public class Army : MonoBehaviourPun
{
    //This class contains all the game information and functions for each player's army. For the time being, there should be only two of these per game, one per player.

    //Game information
    private List<Unit> units;
    private int points;
    private HashSet<GameObject> visible; //List of visible enemy units;
    private int playerID; //keep track of which player controls this army;
    private Unit active;
    private int activePointer;
    private GameController GC;
    private List<GameObject> panel;
    private Dictionary<Unit, Weapon> hits;

    /*Load information
    private string faction;
    private Dictionary<string, string> factions;
    [SerializeField]
    private List<GameObject> BattleGroupsB;
    private Dictionary<string, GameObject> Battlegroups;
    [SerializeField]
    private List<GameObject> ArmyUnitsB; //remember, each of these should have a unit script attatched
    [SerializeField]
    private List<GameObject> CorpUnitsB; //remember, each of these should have a unit script attatched
    [SerializeField]
    private List<GameObject> CoAUnitsB; //remember, each of these should have a unit script attatched
    [SerializeField]
    private List<GameObject> RebUnitsB; //remember, each of these should have a unit script attatched

    private Dictionary<string, GameObject> ArmyUnits;
    private Dictionary<string, GameObject> CorpUnits;
    private Dictionary<string, GameObject> RebUnits;
    private Dictionary<string, GameObject> CoAUnits;
    */

    // Start is called before the first frame update
    void Start()
    {
        /*
        factions = new Dictionary<string, string>{
            { "0", "Army" },
            { "1", "Corp" },
            { "2", "CoA" },
            { "3", "Reb" }
        };
        GC = GameObject.Find("GameController").GetComponent<GameController>();

        ArmyUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in ArmyUnitsB)
        {
            ArmyUnits.Add(ArmyUnitsB.IndexOf(i).ToString(), i);
        }
        CorpUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in CorpUnitsB)
        {
            CorpUnits.Add(CorpUnitsB.IndexOf(i).ToString(), i);
        }
        CoAUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in CoAUnitsB)
        {
            CoAUnits.Add(CoAUnitsB.IndexOf(i).ToString(), i);
        }
        RebUnits = new Dictionary<string, GameObject>();
        foreach (GameObject i in RebUnitsB)
        {
            RebUnits.Add(RebUnitsB.IndexOf(i).ToString(), i);
        }
        */
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        visible = new HashSet<GameObject>();
        hits = new Dictionary<Unit, Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* No Longer Needed! This is all handeled elsewhere now in order to properly sync the armies between players.
    //Load the army when generated
    public IEnumerator Load(string[] input, int toSet)
    {
        yield return new WaitForEndOfFrame();
        //toDo (pretty much just copy my LoadArmy script here, changed to be Units instead of UnitCreators and Models instead of ModelCreators)
        units = new List<Unit>();
        playerID = toSet;
        //Debug.Log("Player: " + toSet.ToString() + " loaded army: " + path);
        int pointer = 0;
        faction = factions[input[pointer].ToString()];
        pointer++;
        int numGroups = int.Parse(input[pointer]);
        pointer++;
        for (int i = 0; i < numGroups; i++)
        {
            //Battlegroups don't matter at this stage in the coding, so just skip over them
            pointer++;
            //Find how many units we're adding
            int numUnits = int.Parse(input[pointer]);
            pointer++;
            if (faction == "Army")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    Debug.Log(input[pointer]);
                    string unitName = ArmyUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\Army\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.SetArmy(this);
                    yield return new WaitForEndOfFrame();
                    //I'll check for custom name here, but later
                    AU.Rename();
                    AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForEndOfFrame();
                        AU.AddModel(model);
                        //I'll check for custom name here, but later
                        model.GetComponent<Model>().Rename();
                        pointer++;
                    }
                    AU.EndEditing();
                }
            }
            else if (faction == "Corp")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    string unitName = CorpUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\Corp\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.SetArmy(this);
                    yield return new WaitForEndOfFrame();
                    //I'll check for custom name here, but later
                    AU.Rename();
                    AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForSeconds(4);
                        AU.AddModel(model);
                        //I'll check for custom name here, but later
                        model.GetComponent<Model>().Rename();
                        pointer++;
                    }
                    AU.EndEditing();
                }
            }
            else if (faction == "CoA")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    string unitName = CoAUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\CoA\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.SetArmy(this);
                    yield return new WaitForEndOfFrame();
                    //I'll check for custom name here, but later
                    AU.Rename();
                    AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForSeconds(4);
                        AU.AddModel(model);
                        //Guess what I'll do here later, that's right, check for custom name
                        model.GetComponent<Model>().Rename();
                        pointer++;
                    }
                    AU.EndEditing();
                }
            }
            else
            {
                for (int x = 0; x < numUnits; x++)
                {
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    string unitName = RebUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\Reb\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.SetArmy(this);
                    yield return new WaitForEndOfFrame();
                    //sorry about that last one, I needed a break for typing the same thing, anyway, custom name check goes here later
                    AU.Rename();
                    AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForEndOfFrame();
                        AU.AddModel(model);
                        //I'll check for custom name here, but later
                        model.GetComponent<Model>().Rename();
                        pointer++;
                    }
                    AU.EndEditing();
                }
            }
        }
        //photonView.RPC("SetUnits", RpcTarget.All, units.ToArray());
    }
    */

    public void SetActive(Unit toSet, Model toModel)
    {
        //sets the unit currently controlled by the player

        active = toSet;
        toSet.GetComponent<Unit>().Select(toModel);
    }

    public Unit GetActive()
    {
        //returns the unit currently controlled by the player

        return active;
    }

    /* Was originally going to be used to scroll between units without pressing a button, since active now also selects the model, this may no longer be needed/wanted/possible
    public void SetActive()
    {
        Unit temp = null;
        if (active != null)
        {
            temp = active;
            if (activePointer == units.Count)
            {
                activePointer = 0;
                active = units[0];
            }
            else
            {
                activePointer++;
                active = units[activePointer];
            }
            temp.GetComponent<Unit>().Deselect();
        }
        else
        {
            active = units[0];
        }
        active.GetComponent<Unit>().Select();

    }
    
    public void SetTarget()
    {
        active.SetTarget();
    }
    */
    public bool ConfirmOrders()
    {
        //most important script in the army class, delegates any orders issued by the player to the assigned unit, and returns if they were able to be carried out.

        int phase = GC.GetGamePhase();
        Debug.Log("Confirming Orders for GamePhase " + phase);
        if (phase == 0)
        {
            foreach (Unit unit in units)
            {
                if (!unit.CheckDeployed()) return false;
            }
            return true;
        }
        if(phase == 7)
        {
            foreach (Unit unit in units)
            {
                if (!unit.CheckActivated()) return false;
            }
            return true;
        }
        if (active != null)
        {
            Debug.Log("Attempting to confirm");
            if (active.GetComponent<Unit>().Confirm())
            {
                Debug.Log("Confirmed, setting toCheck");
                if (phase == 1) active.GetComponent<Unit>().photonView.RPC("SetHasMoved", RpcTarget.All, true);
                else if (phase == 3) active.GetComponent<Unit>().photonView.RPC("SetHasShot", RpcTarget.All, true);
                else if (phase == 4) active.GetComponent<Unit>().photonView.RPC("SetHasCharged", RpcTarget.All, true);
                else if (phase == 5) active.GetComponent<Unit>().photonView.RPC("SetHasPunched", RpcTarget.All, true);
                active = null;
                panel = null;
                return true;
            }
            Debug.Log("Failed to confirm");
            return false;
        }
        else if (phase == 1)
        {
            return NoMove();
        }
        else if (phase == 3)
        {
            return NoShoot();
        }
        else if (phase == 4)
        {
            return NoCharge();
        }
        else
        {
            Debug.Log("Hmmmmmm, this is strange");
            return false;
        }
    }

    public bool NoMove()
    {
        //if the active player chooses not to move any units, prevents them from moving for the rest of the phase

        int sp = GC.GetSpeedPhase();
        Debug.Log("NoMove speedphase: " + sp);
        foreach (Unit unit in units)
        {
            if(unit.GetSpeedClass() == sp)
            {
                unit.photonView.RPC("SetHasMoved", RpcTarget.All, true);
            }
        }
        return true;
    }

    private bool NoShoot()
    {
        //Same as NoMove(), only for the shooting phase
        int sp = GC.GetSpeedPhase();
        Debug.Log("NoShoot speedPhase: " + sp);
        foreach (Unit unit in units)
        {
            if (unit.GetSpeedClass() == sp)
            {
                unit.photonView.RPC("SetHasShot", RpcTarget.All, true);
            }
        }
        return true;
    }

    private bool NoCharge()
    {
        //same as NoMove(), only for the charge phase

        int sp = GC.GetSpeedPhase();
        Debug.Log("NoCharge speedPhase: " + sp);
        foreach (Unit unit in units)
        {
            if(unit.GetSpeedClass() == sp)
            {
                unit.photonView.RPC("SetHasCharged", RpcTarget.All, true);
            }
        }
        return true;
    }

    public List<Unit> GetUnits()
    {
        return units;
    }

    public void SetPanel(List<GameObject> toSet)
    {
        panel = toSet;
        Unit temp = toSet[2].GetComponent<Unit>();
        if (GC.GetGamePhase() == 7 && !temp.CheckActivated()) temp.SetWoundsPanel(true);
        if (GC.GetGamePhase() == 4) temp.SetChargePanel(true);
    }

    public List<GameObject> GetPanel()
    {
        if(active != null)
        {
            if (GC.GetGamePhase() == 4 && active.GetPlanningCharge()) return new List<GameObject>();
            active.Deselect();
            active = null;
        }
        return panel;
    }

    public int GetPlayer()
    {
        return playerID;
    }

    [PunRPC]
    public void SetUnits(int[] toSet, int ID)
    {
        playerID = ID;
        if (units == null)
        {
            List<Unit> temp = new List<Unit>();
            foreach (int toFind in toSet)
            {
                temp.Add(PhotonView.Find(toFind).gameObject.GetComponent<Unit>());
            }
            units = temp;
        }
    }

    public void Iterate()
    {
        Debug.Log("Army Iterating");
        foreach (Unit unit in units)
        {
            unit.Iterate();
        }
    }

    public void Restart()
    {
        Debug.Log("Army Restarting");
        foreach (Unit unit in units)
        {
            unit.Restart();
        }
    }

    [PunRPC]
    public void CheckSight(int other)
    {
        //after deployment, check what unit's can be seen by this player's army

        HashSet<GameObject> temp = new HashSet<GameObject>();
        foreach(Unit unit in units)
        {
            temp.UnionWith(unit.CheckVisible(other));
        }
        visible.ExceptWith(temp);
        foreach(GameObject unit in visible)
        {
            if (unit != null) //Ignore if it's dead
            {
                //make it invisible
                if (PhotonNetwork.LocalPlayer.ActorNumber == playerID) foreach (Model model in unit.GetComponent<Unit>().GetModels())
                    {
                        foreach (Transform child in model.transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 8;
                    }
            }
        }
        visible = temp;
        foreach (GameObject unit in visible)
        {
            //make it visible
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID) foreach (Model model in unit.GetComponent<Unit>().GetModels())
                {
                    foreach (Transform child in model.transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 9;
                }
        }
    }

    public void CheckSight(int other, int toCheck)
    {
        //After each move, update what units can and can't be seen by this army's player

        Unit unit = PhotonView.Find(toCheck).GetComponent<Unit>();
        HashSet<GameObject> toUse = unit.CheckVisible(other);
        HashSet<GameObject> temp = visible;
        visible.ExceptWith(toUse);
        if (!temp.Equals(visible))
        {
            HashSet<GameObject> toRemove = visible;
            foreach (GameObject u in visible)
            {
                foreach(Unit i in units)
                {
                    if (i == unit) continue;
                    if (i.CanSee(u))
                    {
                        toUse.Add(u);
                        toRemove.Remove(u);
                        continue;
                    }
                }
            }
            if(toRemove.Count != 0)
            {
                foreach (GameObject u in toRemove)
                {
                    //make it invisible
                    if (PhotonNetwork.LocalPlayer.ActorNumber == playerID) foreach (Model model in u.GetComponent<Unit>().GetModels())
                        {
                            foreach (Transform child in model.transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 8;
                        }
                }
            }
        }
        visible = toUse;
        foreach (GameObject u in visible)
        {
            //make it visible
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID) foreach (Model model in u.GetComponent<Unit>().GetModels())
                {
                    foreach (Transform child in model.transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 9;
                }
        }
    }

    public void CheckFor(int other, int toCheck)
    {
        //after a unit in the other player's army moves, check if it can be seen

        Unit unit = PhotonView.Find(toCheck).GetComponent<Unit>();
        bool foundYou = false;
        foreach(Unit u in units)
        {
            if (u.CheckFor(unit)) foundYou = true;
        }
        if (foundYou)
        {
            visible.Add(unit.gameObject);
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID) foreach (Model model in unit.GetComponent<Unit>().GetModels())
                {
                    foreach (Transform child in model.transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 9;
                }
        }
        else if (visible.Contains(unit.gameObject))
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerID) foreach (Model model in unit.GetComponent<Unit>().GetModels())
                {
                    foreach (Transform child in model.transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 8;
                }
        }
    }

    [PunRPC]
    public void AddHit(int target, int weapon)
    {
        hits[PhotonView.Find(target).GetComponent<Unit>()] = PhotonView.Find(weapon).GetComponent<Weapon>();
    }

    public void EndDeploy()
    {
        foreach (Unit unit in units)
        {
            unit.EndDeploy();
            if(unit == active)
            {
                unit.EndActive();
                active = null;
            }
        }
    }

    public void AddPoints(int toAdd)
    {
        points += toAdd;
        GC.UpdateScore(playerID, points);
    }

    public void SetIncoming(Weapon toSet, GameObject btn)
    {
        if(active != null) active.SetIncoming(toSet, btn);
    }

    public void ConfirmHit()
    {
        if (active != null) active.ConfirmHit();
    }

    public void RemoveUnit(Unit toRemove)
    {
        if (active == toRemove)
        {
            active = null;
            panel = null;
        }
        units.Remove(toRemove);
        GC.photonView.RPC("UpdateSight", RpcTarget.All);
    }

    public int CheckGameOver()
    {
        if (units.Count != 0) return -1;
        else return playerID;
    }
}
