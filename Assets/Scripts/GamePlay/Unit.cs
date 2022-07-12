using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Unit : MonoBehaviourPun
{
    //this class contains all the game information and functions for each unit in the game.

    [SerializeField]
    private List<Model> possModels; //A list of the models that can be added to this unit, used for creation at runtime
    [SerializeField]
    private List<Model> origModels;
    [SerializeField]
    private int origSpeedClass; //Where the unit falls in the turn order. (Maybe find a better name for this)
    private List<Model> models;
    private List<GameObject> visible;

    [SerializeField]
    private GameObject selButton;
    private GameObject relbutton;

    [SerializeField]
    private GameObject scroll;
    private GameObject modelScroll;
    private GameObject hitsScroll;

    [SerializeField]
    private GameObject chargeScrollPref;
    private GameObject chargeScroll;

    [SerializeField]
    private string origName;
    new private string name;

    private bool hasMoved;
    private bool hasShot;
    private bool planningCharge;
    private bool hasCharged;
    private bool hasPunched;
    private bool inMelee;

    private List<Weapon> hits;
    private Weapon incoming;
    private GameObject wpnbtn;

    private bool allDeployed;
    private int speedClass;
    private int playerID;

    private Model activeModel;

    private GameController GC;
    private Army army;

    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        hasMoved = false;
        hasShot = false;
        hasPunched = false;
        planningCharge = false;
        inMelee = false;
        speedClass = origSpeedClass;
        hits = new List<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public void AddModel(Model toAdd)
    {
        origModels.Add(toAdd);
        toAdd.SetPlayer(playerID);
        toAdd.SetUnit(this);
    }
    */

    public void CheckMoved()
    {//Once each model is done moving, update the list of visible units in the army, and update the average location of the unit, important for charging (for now).

        foreach (Model model in models)
        {
            if (model.GetIsMoving()) return;
        }
        GC.photonView.RPC("UpdateSight", RpcTarget.All, photonView.ViewID, playerID);
        photonView.RPC("UpdatePosition", RpcTarget.All);
    }

    [PunRPC]
    public void UpdatePosition()
    {//update the average location of the unit.

        Vector3 avg = Vector3.zero;
        foreach (Model model in models)
        {
            avg += model.transform.position;
        }
        transform.position = avg/models.Count;
    }

    [PunRPC]
    public void EndEditing(int[] toAdd)
    {
        Debug.Log(toAdd);
        List<Model> temp = new List<Model>();
        foreach (int toFind in toAdd)
        {
            Model model = PhotonView.Find(toFind).gameObject.GetComponent<Model>();
            model.photonView.RPC("SetPlayer", RpcTarget.All, playerID);
            model.photonView.RPC("SetUnit", RpcTarget.All, photonView.ViewID);
            temp.Add(model);
        }
        origModels = temp;
        models = origModels;
    }

    public List<Model> GetLegalModels()
    {
        return possModels;
    }

    public List<Model> GetModels()
    {
        return models;
    }

    public void Select(Model toSet)
    {//changes the currently active model in this unit

        if (activeModel != null)
        {
            activeModel.GetComponent<Model>().ChangeModel();
        }
        else
        {
            foreach (Model model in models)
            {
                model.CheckState();
            }
        }
        activeModel = toSet;
        activeModel.GetComponent<Model>().Select();
        if (GC.GetGamePhase() == 7) hitsScroll.SetActive(true);
    }

    public void Select()
    {
        if (activeModel == null)
        {
            activeModel = models[0];
            activeModel.GetComponent<Model>().Select();
        }
        else
        {
            int temp = models.IndexOf(activeModel)+1;
            if (temp == models.Count) temp = 0;
            activeModel = models[temp];
            activeModel.GetComponent<Model>().Select();
        }   
    }

    public Model GetActive()
    {
        return activeModel;
    }

    public void Deselect()
    {
        //tell each model and the UI that this unit is no longer the active unit

        Debug.Log("Unit Deselecting");
        relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        modelScroll.SetActive(false);
        foreach (Model model in models)
        {
            if(model == activeModel)
            {
                model.ActiveDeselect();
                activeModel = null;
            }
            model.Deselect();
        }
        if (GC.GetGamePhase() == 7) hitsScroll.SetActive(false);
    }

    /*
    public void SelectTarget(GameObject toSet)
    {
        //highlight all models
    }
    
    public void DeselectTarget()
    {
        //dehighlight all models
    }
    */

    public bool Confirm()
    {
        //The most important function in this class, confirms all orders issued by the player are valid, and if so, delegates them to the individual models.

        int phase = GC.GetGamePhase();
        if (phase == 1) //how to handle the movement phase
        {
            bool oneOrMoreMoved = false;
            foreach (Model model in models)
            {
                if (!model.CheckMoved())
                {
                    if (!model.CheckCohesion())
                    {
                        Debug.Log("Failed Due to Cohesion");
                        return false;
                    }
                }
                else oneOrMoreMoved = true;
            }
                if (!oneOrMoreMoved)
                {
                    return army.NoMove();
                }
            foreach(Model model in models)
            {
                model.Move();
            }
            activeModel.GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
            modelScroll.SetActive(false);
            relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeModel = null;
            return true;
        }
        else if(phase== 3)//how to handle the shooting phase
        {
            foreach (Model model in models) model.Shoot();
            activeModel.GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeModel.GetRelButton().GetComponent<UnityEngine.UI.Image>().color = Color.white;
            modelScroll.SetActive(false);
            relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeModel = null;
            return true;
        }
        else if (phase == 4)//handle the charge phase
        {
            bool oneOrMoreCharged = false;
            foreach (Model model in models)
            {
                if (!model.CheckCharged())
                {
                    if (!model.CheckCohesion())
                    {
                        Debug.Log("Failed Due to Cohesion");
                        return false;
                    }
                }
                else oneOrMoreCharged = true;
            }
            if (!oneOrMoreCharged)
            {
                return false;
            }
            foreach (Model model in models)
            {
                model.Move();
            }
            activeModel.GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
            modelScroll.SetActive(false);
            relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeModel = null;
            planningCharge = false;
            hasCharged = true;
            return true;
        }
        else if (phase == 5)//handle the melee phase
        {
            foreach (Model model in models) model.Punch();
            activeModel.GetRelButton().GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeModel.GetRelButton().GetComponent<UnityEngine.UI.Image>().color = Color.white;
            modelScroll.SetActive(false);
            relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            activeModel = null;
            hasPunched = true;
            return true;
        }
        return false;
    }

    public int GetSpeedClass()
    {
        return speedClass;
    }

    [PunRPC]
    public void SetHasMoved(bool toSet)
    {
        hasMoved = toSet;
    }

    [PunRPC]
    public void SetHasShot(bool toSet)
    {
        hasShot = toSet;
    }

    [PunRPC]
    public void SetHasCharged(bool toSet)
    {
        hasCharged = toSet;
    }

    [PunRPC]
    public void SetInMelee(bool toSet)
    {
        inMelee = toSet;
    }

    [PunRPC]
    public void SetHasPunched(bool toSet)
    {
        hasPunched = toSet;
    }

    public bool GetHasMoved()
    {
        return hasMoved;
    }

    public void Rename(string toName)
    {
        name = toName;
        if (GetComponent<PhotonView>().IsMine)
        {
            relbutton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = name;
        }
    }

    public void Rename() //this'll need to be an rpc, but I'm feeling lazy right now.
    {
        name = origName;
    }

    public string GetName()
    {
        return origName;
    }
    /*
    public void SetTarget()
    {
        activeModel.GetComponent<Model>().SetTarget();
    }
    */
    public bool CheckDeployed()
    {
        Debug.Log(models);
        foreach (Model model in models)
        {
            if (!model.CheckDeployed()) return allDeployed = false;
        }
        return allDeployed = true;
    }

    public GameObject GetModelScroll()
    {
        return modelScroll;
    }

    [PunRPC]
    public void SetPlayer(int toSet) //connect the unit to the player that controlls it, and setup the UI connected to the unit.

    {
        playerID = toSet;
        if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
        {
            Debug.Log("Adding unit UI " + origName + " to player " + playerID + "'s army");
            relbutton = Instantiate(selButton);
            relbutton.transform.SetParent(GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Units Scroll").transform.Find("Viewport").transform.Find("Content"), true);
            modelScroll = Instantiate(scroll);
            modelScroll.transform.SetParent(GameObject.Find("Canvas").transform.Find("Panel"), false);
            modelScroll.SetActive(false);
            Destroy(modelScroll.transform.Find("Button").gameObject);
            modelScroll.transform.localPosition = new Vector3((float)132.55, 0, 0);
            relbutton.GetComponent<SelectButton>().SetPanel(modelScroll, playerID, this);
            relbutton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = name;
            hitsScroll = Instantiate(scroll);
            hitsScroll.transform.SetParent(GameObject.Find("Canvas").transform, false);
            hitsScroll.SetActive(false);
            hitsScroll.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Hits";
            hitsScroll.transform.Find("Button").GetComponent<HitsButton>().SetArmy(army);
            chargeScroll = Instantiate(chargeScrollPref);
            chargeScroll.SetActive(false);
            chargeScroll.transform.Find("Button").GetComponent<ChargeButton>().SetUnit(this);
            chargeScroll.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
    }

    [PunRPC]
    public void SetArmy(int toSet)
    {
        army = PhotonView.Find(toSet).GetComponent<Army>();
    }

    public Army GetArmy()
    {
        return army;
    }

    public void Iterate()
    {
        foreach (Model model in models)
        {
            model.Iterate();
        }
    }

    public void Restart()
    {
        Debug.Log("Starting Over");
        hasMoved = false;
        hasShot = false;
        hasCharged = false;
        hasPunched = false;
        speedClass = origSpeedClass;
        foreach (Model model in models) model.Restart();
    }

    public bool CheckActivated()
    {
        Debug.Log("Unit Checking Activated for player: " + playerID);
        if (GC.GetGamePhase() == 1)
        {
            Debug.Log(hasMoved);
            return hasMoved;
        }
        /* if(GC.GetGamePhase() == 2){
            return hasDoneSpecial;
          }
         */
        if (GC.GetGamePhase() == 3)
        {
            if (inMelee) return true;
            if (hasShot) return true;
            foreach(Model model in models)
            {
                if (model.GetVisible().Count != 0) return false;
            }
            return true;
        }
        if (GC.GetGamePhase() == 4)
        {
            return hasCharged;
        }
        if (GC.GetGamePhase() == 5 && inMelee)
        {
            return hasPunched;
        }
        if (GC.GetGamePhase() == 7)
        {
            return hits.Count == 0;
        }
        Debug.Log("Must be punchy phase and not in melee... I think");
        return true;
    }

    public bool GetPlanningCharge()
    {
        return planningCharge && GC.GetGamePhase() == 4;
    }

    public void EndDeploy()
    {
        foreach (Model model in models)
        {
            model.EndDeploy();
        }
    }

    public void StartDamage()
    {
        if(hits.Count == 0)
        {
            relbutton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    public void EndDamage()
    {
        relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void EndActive()
    {
        modelScroll.SetActive(false);
        relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        activeModel.EndActive();
        activeModel = null;
    }

    public HashSet<GameObject> CheckVisible(int other)
    {
        HashSet<GameObject> temp = new HashSet<GameObject>();
        foreach (Model model in models)
        {
            temp.UnionWith(model.CheckVisible(other));
        }
        visible = new List<GameObject>(temp);
        return temp;
    }

    public bool CheckFor(Unit toCheck)
    {
        bool foundYou = false;
        foreach(Model model in models)
        {
            if (model.CheckFor(toCheck)) foundYou = true;
        }
        return foundYou;
    }

    public bool CanSee(GameObject toCheck)
    {
        foreach(Model model in models)
        {
            if (model.CanSee(toCheck)) return true;
        }
        return false;
    }

    [PunRPC]
    public void AddHit(int toAdd)
    {
        if (photonView.IsMine)
        {
            Weapon weapon = PhotonView.Find(toAdd).gameObject.GetComponent<Weapon>();
            hits.Add(weapon);
            GameObject temp = Instantiate(weapon.GetButton()); //DON'T FORGET TO ACTUALLY MAKE THE BUTTON! OTHERWISE WE'LL HAVE ISSUES! SPECIFICALLY WE'LL HAVE ISSUES HERE! Hey Brian, guess what you forgot...
            temp.transform.SetParent(hitsScroll.transform.Find("Viewport").Find("Content"));
            temp.GetComponent<WeaponButton>().SetWeapon(weapon);
            temp.GetComponent<WeaponButton>().SetArmy(army);
        }
    }

    public void SetIncoming(Weapon toSet, GameObject btn)
    {
        if (incoming != null)
        {
            wpnbtn.GetComponent<UnityEngine.UI.Button>().interactable = true;
            wpnbtn.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        incoming = toSet;
        wpnbtn = btn;
        wpnbtn.GetComponent<UnityEngine.UI.Button>().interactable = false;
        wpnbtn.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
    }

    public void ConfirmHit()
    {
        if (activeModel != null && incoming != null)
        {
            activeModel.ConfirmHit(incoming);
            hits.Remove(incoming);
            incoming = null;
            Destroy(wpnbtn);
            if (hits.Count == 0)
            {
                modelScroll.SetActive(false);
                hitsScroll.SetActive(false);
            }
        }
    }

    public void RemoveModel(Model toRemove)
    {
        if (toRemove == activeModel) activeModel = null;
        models.Remove(toRemove);
        if (models.Count == 0) Die();
    }

    public void Die()
    {
        army.RemoveUnit(this);
        if (photonView.IsMine)
        {
            Destroy(hitsScroll);
            Destroy(modelScroll);
            Destroy(relbutton);
            PhotonNetwork.Destroy(photonView);
        }
    }

    public void SetWoundsPanel(bool toSet)
    {
        hitsScroll.SetActive(toSet);
    }

    public void PrepareCharge()
    {
        //Confirm the Unit's charge target, and generate a random distance for each model in this unit to charge

        UnityEngine.UI.Dropdown dropdown = chargeScroll.transform.Find("Dropdown").GetComponent<UnityEngine.UI.Dropdown>();
        if (dropdown.value == 0) return;
        Unit toCharge = visible[dropdown.value - 1].GetComponent<Unit>();
        int charge = models[0].GetMove();
        charge = Random.Range( 1, charge * 2);
        foreach (Model model in models)
        {
            model.PrepareCharge(toCharge, charge);
        }
        toCharge.BeginChargeTarget();
        planningCharge = true;
        chargeScroll.SetActive(false);
    }

    public void BeginChargeTarget()
    {
        foreach (Model model in models)
        {
            model.BeginChargeTarget();
        }
    }

    public void EndChargeTarget()
    {
        Debug.Log("Ending Charge Target");
        foreach (Model model in models)
        {
            model.EndChargeTarget();
        }
    }

    public void CheckMelee()
    {
        foreach (Model model in models)
        {
            if (model.GetInMeleeWith())
            {
                inMelee = true;
                break;
            }
        }
        inMelee = false;
    }

    public void SetChargePanel(bool toSet)
    {
        if (toSet)
        {
            UnityEngine.UI.Dropdown dropdown = chargeScroll.transform.Find("Dropdown").GetComponent<UnityEngine.UI.Dropdown>();
            dropdown.ClearOptions();
            List<string> temp = new List<string>() { "No target selected" };
            foreach (GameObject unit in visible)
            {
                temp.Add(unit.GetComponent<Unit>().GetName());
            }
            dropdown.AddOptions(temp);
        }
        chargeScroll.SetActive(toSet);
    }

    public bool GetInMelee()
    {
        return inMelee;
    }
    
    /*
    public GameObject GetTargetButton()
    {
        return Instantiate(tarButton);
    }
    */
}
