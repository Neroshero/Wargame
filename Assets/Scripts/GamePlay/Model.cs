using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Model : MonoBehaviourPun
{
    //This class contains the game information and functions for the individual models

    private int playerID;

    private HashSet<GameObject> visible; //These should be Units;
    private HashSet<GameObject> inMeleeWith;
    private bool isMoving;
    private bool hasMoved;
    private bool hasShot;
    private bool hasCharged;
    private bool inMelee;
    private bool hasPunched;
    private bool hasDeployed;

    private Unit unit;
    private GameController GC;

    private ProBuilderMesh moveRadius;
    private ProBuilderMesh cohesionRadius;
    private ProBuilderMesh chargeRadius;
    private Vector3 destination;

    private Unit chargeTarget;
    private Army army;
    private Dictionary<Unit, List<Weapon>> hits;

    [SerializeField]
    private GameObject button;
    private GameObject relbutton;

    /*[SerializeField]
    private GameObject panel;
    private GameObject targetPanel;
    */
    [SerializeField]
    private GameObject panel;
    private GameObject wpnPanel;
    private GameObject melPanel;

    [SerializeField]
    private Material moveMaterial;
    [SerializeField]
    private Material cohesionMaterial;
    [SerializeField]
    private Material chargeMaterial;
    [SerializeField]
    GameObject movePlanner;
    GameObject planMove;

    [SerializeField]
    private string origName;
    new private string name;

    [SerializeField]
    private int origHealth;
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
    private int move;
    private int charge;
    private int health;
    private bool wounded;

    [SerializeField]
    private List<GameObject> toInstantiate;
    private List<Weapon> weapons;

    //private List<GameObject> visibleButtons;


    // Start is called before the first frame update
    void Start()
    {
        //do the initial setup for the model

        //PhotonPeer.RegisterType(typeof(Model), (byte)'u', Model.SerializeModel, Model.DeSerializeModel);
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        hasMoved = false;
        hasShot = false;
        hasPunched = false;
        charge = 0;
        destination = transform.position;
        visible = new HashSet<GameObject>();
        inMeleeWith = new HashSet<GameObject>();
        health = origHealth;
        wounded = false;
        if (GetComponent<PhotonView>().IsMine)
        {
            relbutton = Instantiate(button);
            /*
            targetPanel = Instantiate(panel);
            targetPanel.SetActive(false);
            targetPanel.transform.SetParent(GameObject.Find("Canvas").transform, true);
            */
            weapons = new List<Weapon>();
            wpnPanel = Instantiate(panel);
            melPanel = Instantiate(panel);
            wpnPanel.SetActive(false);
            melPanel.SetActive(false);
            wpnPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
            melPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
            foreach (GameObject toDo in toInstantiate)
            {
                GameObject temp = PhotonNetwork.Instantiate("Weapons\\" + toDo.name, new Vector3(), new Quaternion());
                Weapon weapon = temp.GetComponent<Weapon>();
                Debug.Log(weapon);
                temp = weapon.GetNameBox();
                if (weapon.IsMelee())
                {
                    temp.transform.SetParent(melPanel.transform.Find("Viewport").transform.Find("Content"), false);
                    temp = weapon.GetDropdown();
                    temp.transform.SetParent(melPanel.transform.Find("Viewport").transform.Find("Content"), false);
                }
                else
                {
                    temp.transform.SetParent(wpnPanel.transform.Find("Viewport").transform.Find("Content"), false);
                    temp = weapon.GetDropdown();
                    temp.transform.SetParent(wpnPanel.transform.Find("Viewport").transform.Find("Content"), false);
                }
                weapons.Add(weapon);
            }
        }

        //playerID = what it needs to be;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) //I'm starting to wonder if this'll cause issues between machines with significantly different hardware; 
                      //keep it the same for now, but maybe change isMoving to an RPC later
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 2 * Time.deltaTime);
            if (Vector3.Distance(transform.position, destination) == 0)
            {
                //GC.photonView.RPC("UpdateSight", RpcTarget.All, unit.photonView.ViewID, playerID);
                isMoving = false;
                unit.CheckMoved();
            }
        }
    }

    public HashSet<GameObject> CheckVisible(int other)
    {
        //After each move, look at every unit in the opposing army to see what is and isn't visible and report back to the unit.

        Debug.Log(other);
        HashSet<GameObject> temp = new HashSet<GameObject>();
        Army toFind = PhotonView.Find(other).GetComponent<Army>();
        foreach(Unit unit in toFind.GetUnits())
        {
            foreach(Model model in unit.GetModels())
            {
                Ray ray = new Ray
                {
                    origin = transform.position + Vector3.up,
                    direction = model.transform.position - transform.position //* 110 + Vector3.down * 50
                };
                Debug.DrawRay(ray.origin, ray.direction * 130, Color.red, 10);
                RaycastHit hit = new RaycastHit();
                if(Physics.Raycast(ray, out hit, 130, ~10) && hit.collider.tag == "Model")
                {
                    Debug.Log("Hit model");
                    if (hit.collider.gameObject == model.gameObject)
                    {
                        Debug.Log("Definitely hit it");
                        temp.Add(unit.gameObject);

                        /*
                        GameObject otherTemp = Instantiate(unit.GetTargetButton());
                        //setup temp to point to unit. Still need to write the method.
                        visibleButtons.Add(otherTemp);
                        */

                        break;
                    }
                }
            }
        }
        visible.ExceptWith(temp);
        Debug.Log(visible.Count);
        if (visible.Count != 0 && photonView.IsMine)
        {
            foreach (GameObject unit in visible) foreach (Weapon weapon in weapons) weapon.RemoveTarget(unit);
        }
        visible = temp;
        if (photonView.IsMine)
        {
            Debug.Log("Model visible " + visible.Count);
            foreach (Weapon weapon in weapons)
            {
                if (!weapon.IsMelee()) weapon.AddTargets(visible);
                else
                {
                    Debug.Log(inMeleeWith);
                    weapon.AddTargets(inMeleeWith);
                }
            }
        }
        return visible;
    }

    public bool CheckFor(Unit toCheck)
    {
        //After a unit in the opposing army moves, check to see if it is visible.

        bool foundYou = false;
        foreach (Model model in toCheck.GetModels())
        {
            Ray ray = new Ray
            {
                origin = transform.position + Vector3.up,
                direction = model.transform.position - transform.position
            };
            Debug.DrawRay(ray.origin, ray.direction * 130, Color.red, 10);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 130, ~10) && hit.collider.tag == "Model")
            {
                Debug.Log("Hit model");
                if (hit.collider.gameObject == model.gameObject)
                {
                    Debug.Log("Definitely hit it");
                    foundYou = true;
                    break;
                }
            }
        }
        if (foundYou)
        {
            if(!visible.Contains(toCheck.gameObject))
            visible.Add(toCheck.gameObject);
            if (photonView.IsMine)
            {
                //foreach (Weapon weapon in weapons) weapon.AddTarget(toCheck.gameObject);
                foreach (Weapon weapon in weapons)
            {
                if (!weapon.IsMelee()) weapon.AddTargets(visible);
                else
                {
                    Debug.Log(inMeleeWith);
                    weapon.AddTargets(inMeleeWith);
                }
            }
            }
            /*
            if (photonView.Controller.IsLocal)
            {
                GameObject temp = toCheck.GetTargetButton();
                temp.transform.SetParent(targetPanel.transform.Find("Viewport").transform.Find("Content"));
                //setup temp to point to toCheck. Still need to write the method.
                visibleButtons.Add(temp);
            }
            */
        }
        else if (visible.Contains(toCheck.gameObject))
        {
            visible.Remove(toCheck.gameObject);
            if (photonView.IsMine) foreach (Weapon weapon in weapons) weapon.RemoveTarget(toCheck.gameObject);
        }
        return foundYou;
    }

    public bool CanSee(GameObject toCheck)
    {
        return visible.Contains(toCheck);
    }

    public HashSet<GameObject> GetVisible()
    {
        return visible;
    }

    public void Move()
    {
        //handle the models movements, both in the movement phase and in the charge phase.

        isMoving = true;
        if (GC.GetGamePhase() == 1)
        {
            unit.SetHasMoved(true);
            inMelee = false;
            unit.SetInMelee(false);
        }
        else
        {
            hasCharged = true;
            charge = 0;
            bool added = false;
            foreach (Model model in chargeTarget.GetModels())
            {
                if (Vector3.Distance(model.transform.position, destination) <= 3)
                {
                    if (!added)
                    {
                        inMelee = true;
                        inMeleeWith.Add(chargeTarget.gameObject);
                        unit.photonView.RPC("SetInMelee", RpcTarget.All, true);
                    }
                    model.photonView.RPC("AddInMelee", RpcTarget.All, photonView.ViewID);
                    added = true;
                }
            }
            chargeTarget.EndChargeTarget();
            chargeTarget = null;
        }
        Destroy(planMove);
        if(cohesionRadius != null) Destroy(cohesionRadius.gameObject);
        if(moveRadius != null) Destroy(moveRadius.gameObject);
        relbutton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }

    /*
     * Because InMelee now tracks units instead of models, this may no longer be necessary here;
    public void ClearInMelee()
    {
        //remove all opposing units in this models inMeleeWith, and remove this model from those unit's models inMeleeWith

        foreach(GameObject unit in inMeleeWith)
        {
            Unit temp = unit.GetComponent<Unit>();
            foreach(Model model in temp.GetModels())
            model.photonView.RPC("RemoveInMelee", RpcTarget.All, photonView.ViewID, temp.photonView.ViewID);
        }
        inMeleeWith = new HashSet<GameObject>();
        inMelee = false;
        unit.CheckMelee();
    }
    */

    [PunRPC]
    public void AddInMelee(int toAdd)
    {
        //Add a new model to this model's InMeleeWith

        Model model = PhotonView.Find(toAdd).GetComponent<Model>();
        if (!inMeleeWith.Contains(model.unit.gameObject)) inMeleeWith.Add(model.unit.gameObject);
        model.inMelee = true;
        unit.SetInMelee(true);
    }
    
    [PunRPC]
    public void RemoveInMelee(int toRemove, int toUnit)
    {
        //Remove existing models from this model's InMeleeWith

        Model model = PhotonView.Find(toRemove).GetComponent<Model>();
        Unit unit = PhotonView.Find(toUnit).GetComponent<Unit>();
        foreach(Model other in unit.GetModels())
        {
            if (other != model && Vector3.Distance(model.transform.position, destination) <= 3) return;
        }
        inMeleeWith.Remove(unit.gameObject);
        if (inMeleeWith.Count == 0)
        {
            inMelee = false;
            unit.CheckMelee();
        }
    }

    public bool GetInMeleeWith()
    {
        return inMelee;
    }

    public void Shoot()
    {
        //Handle ranged weapon attacks with this model

        bool oneOrMoreHit = false;
        foreach (Weapon weapon in weapons)
        {
            if(!weapon.IsMelee()) if (weapon.Attack(ranAcc, "medium")) oneOrMoreHit = true;
        }
        if (oneOrMoreHit) GC.photonView.RPC("DoDamage", RpcTarget.All, true);
        hasShot = true;
        relbutton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        wpnPanel.SetActive(false);
    }

    public int GetMelDef()
    {
        return melDef;
    }

    public void Punch()
    {
        //Handle melee weapon attacks with this model

        bool oneOrMoreHit = false;
        foreach (Weapon weapon in weapons) if(weapon.IsMelee()) if (weapon.Attack(melAcc, "melee")) oneOrMoreHit = true;
        if (oneOrMoreHit) GC.photonView.RPC("DoDamage", RpcTarget.All, true);
        hasPunched = true;
        relbutton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        melPanel.SetActive(false);
    }

    public void AddHit(Unit unit, Weapon weapon)
    {
        if (!hits.ContainsKey(unit)) hits.Add(unit, new List<Weapon>());
        hits[unit].Add(weapon);
    }

    public void PlanMove(Vector3 toMove)
    {
        //Visualize the planned move so the player can see without committing

        bool oneOrMoreMoved = false;
        if(Vector3.Distance(toMove, transform.position) > move)
        {
            Debug.Log("Trying to move beyond radius");
            return;
        }
        destination = toMove;
        if (CheckCohesion())
        {
            Debug.Log("Cohesion passed: moving");
            if (hasMoved)
            {
                Debug.Log("Previously moved, destroying cohesion circle: " + moveRadius);
                if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
                {
                    Destroy(cohesionRadius.gameObject);
                    Destroy(planMove);
                }
            }
            hasMoved = true;
            cohesionRadius = ShapeGenerator.GenerateCylinder(0, 40, 5, 1, 0);

            cohesionRadius.SetMaterial(cohesionRadius.faces, cohesionMaterial);
            cohesionRadius.transform.position = destination;
            planMove = Instantiate(movePlanner, destination, new Quaternion());
        }
        else
        {
            foreach (Model model in unit.GetModels())
            {
                if (model.CheckMoved() && model != this)
                {
                    Debug.Log("Another model has moved");
                    oneOrMoreMoved = true;
                    break;
                }
            }
        }
        if (!oneOrMoreMoved)
        {
            Debug.Log("Cohesion failed, but first moved: moving");
            if (hasMoved)
            {
                Debug.Log("Previously moved, destroying cohesion circle: " + moveRadius);
                if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
                {
                    Destroy(cohesionRadius.gameObject);
                    Destroy(planMove);
                }
            }
            hasMoved = true;
            Debug.Log(hasMoved);
            cohesionRadius = ShapeGenerator.GenerateCylinder(0, 40, 5, 1, 0);
            cohesionRadius.gameObject.GetComponent<MeshRenderer>().material = cohesionMaterial;
            //cohesionRadius.SetMaterial(cohesionRadius.faces, cohesionMaterial);
            cohesionRadius.transform.position = destination;
            planMove = Instantiate(movePlanner, destination, new Quaternion());
        }
        else
        {
            destination = transform.position;
            Debug.Log("Failed due to cohesion");
        }
    }

    public void PlanCharge(Vector3 toMove)
    {
        //like PlanMove above, only for charges

        bool oneOrMoreCharged = false;
        if (Vector3.Distance(toMove, transform.position) > charge)
        {
            Debug.Log("Trying to move beyond radius");
            return;
        }
        if (Vector3.Distance(toMove, chargeTarget.transform.position) > Vector3.Distance(transform.position, chargeTarget.transform.position))
        {
            Debug.Log("Trying to move away from charge target");
            return;
        }
        destination = toMove;
        if (CheckCohesion())
        {
            Debug.Log("Cohesion passed: moving");
            if (hasCharged)
            {
                Debug.Log("Previously moved, destroying cohesion circle: " + moveRadius);
                if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
                {
                    Destroy(cohesionRadius.gameObject);
                    Destroy(planMove);
                }
            }
            hasCharged = true;
            cohesionRadius = ShapeGenerator.GenerateCylinder(0, 40, 5, 1, 0);

            cohesionRadius.SetMaterial(cohesionRadius.faces, cohesionMaterial);
            cohesionRadius.transform.position = destination;
            planMove = Instantiate(movePlanner, destination, new Quaternion());
        }
        else
        {
            foreach (Model model in unit.GetModels())
            {
                if (model.CheckCharged() && model != this)
                {
                    Debug.Log("Another model has moved");
                    oneOrMoreCharged = true;
                    break;
                }
            }
        }
        if (!oneOrMoreCharged)
        {
            Debug.Log("Cohesion failed, but first moved: moving");
            if (hasCharged)
            {
                Debug.Log("Previously moved, destroying cohesion circle: " + moveRadius);
                if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
                {
                    Destroy(cohesionRadius.gameObject);
                    Destroy(planMove);
                }
            }
            hasCharged = true;
            cohesionRadius = ShapeGenerator.GenerateCylinder(0, 40, 5, 1, 0);
            cohesionRadius.gameObject.GetComponent<MeshRenderer>().material = cohesionMaterial;
            //cohesionRadius.SetMaterial(cohesionRadius.faces, cohesionMaterial);
            cohesionRadius.transform.position = destination;
            planMove = Instantiate(movePlanner, destination, new Quaternion());
        }
        else
        {
            destination = transform.position;
            Debug.Log("Failed due to cohesion");
        }
    }

    [PunRPC]
    public bool Deploy(Vector3 toDeploy)
    {
        //Deploy the model during the deployment phase

        Debug.Log(playerID);
        bool oneOrMoreDeployed = false;
        destination = toDeploy;
        if (CheckCohesion())
        {
            Debug.Log("Cohesion passed: Deploying");
            if (hasDeployed)
            {
                Debug.Log("Previously moved, destroying cohesion circle: " + moveRadius);
                if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal) Destroy(cohesionRadius.gameObject);
            }
            hasDeployed = true;
            transform.position = new Vector3(toDeploy.x, toDeploy.y - 0.1f, toDeploy.z);
            if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
            {
                cohesionRadius = ShapeGenerator.GenerateCylinder(0, 40, 5, 1, 0);
                cohesionRadius.gameObject.GetComponent<MeshRenderer>().material = cohesionMaterial;
                //cohesionRadius.SetMaterial(cohesionRadius.faces, cohesionMaterial);
                cohesionRadius.transform.position = transform.position;
            }
            return true;
        }
        else
        {
            foreach (Model model in unit.GetModels())
            {
                if (model.CheckDeployed() && model != this)
                {
                    Debug.Log("Another Model has deployed");
                    oneOrMoreDeployed = true;
                    break;
                }
            }
        }
        if (!oneOrMoreDeployed)
        {
            Debug.Log("Cohesion failed, but first moved: Deploying");
            if (hasDeployed)
            {
                Debug.Log("previously moved, destroying cohesion circle: " + moveRadius);
                if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)  Destroy(cohesionRadius.gameObject);
            }
            hasDeployed = true;
            transform.position = new Vector3(toDeploy.x, toDeploy.y - 0.1f, toDeploy.z);
            if (PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
            {
                cohesionRadius = ShapeGenerator.GenerateCylinder(0, 40, 5, 1, 0);
                cohesionRadius.gameObject.GetComponent<MeshRenderer>().material = cohesionMaterial;
                //cohesionRadius.SetMaterial(cohesionRadius.faces, cohesionMaterial);
                cohesionRadius.transform.position = transform.position;
            }
            return true;
        }
        else
        {
            Debug.Log("Failed due to cohesion");
            return false;
        }
    }

    [PunRPC]
    public void SetUnit(int toSet)
    {
        Unit toUnit = PhotonView.Find(toSet).GetComponent<Unit>();
        army = toUnit.GetArmy();
        Debug.Log("Model army: " + army);
        unit = toUnit;
        if (photonView.IsMine)
        {
            relbutton.transform.SetParent(unit.GetComponent<Unit>().GetModelScroll().transform.Find("Viewport").transform.Find("Content"), true);
            relbutton.GetComponent<MSelectButton>().SetModel(this, army);
        }
    }

    public int GetPlayer()
    {
        return playerID;
    }

    public Unit GetUnit()
    {
        return unit;
    }

    /*
    public void RanAttack(GameObject toAttack, Weapon toUse)
    {
        //toAttack is a unit picked from visible;
        //use a rand and add ranAttack, (later on will be adding additional modifiers, such as toUse's range values) if final value > 100, hit, else, nothing;
        //if hit, get str of toUse and compare with toAttack, use a rand to generate a number, and if it rolls in a specific range based on the earlier comparision (tbd) damage;
        //if damage, get damage value from weapon (which is either another roll or a set number), and pass it on to toAttack;
        //all else aside, set hasShot to true;
    }

    public void MelAttack(GameObject toAttack, Weapon toUse)
    {
        //as above, subtituting ranAttack w/ melAttack;
        //all else aside, set hasPunched to true;
    }

    */
    public void Restart()
    {
        //return the model's values to default at the start of a new turn

        hasMoved = false;
        hasShot = false;
        hasCharged = false;
        hasPunched = false;
        destination = transform.position;
    }

    public void BeginChargeTarget()
    {
        chargeRadius = ShapeGenerator.GenerateCylinder(0, 40, 3, 0.4f, 0);
        chargeRadius.transform.position = gameObject.transform.position;
        chargeRadius.gameObject.GetComponent<MeshRenderer>().material = chargeMaterial;
        chargeRadius.SetMaterial(chargeRadius.faces, chargeMaterial);
    }

    public void EndChargeTarget()
    {
        Debug.Log("Destroying Charge Radius");
        Destroy(chargeRadius.gameObject);
    }

    public void Select()
    {
        //When this model is selected, make important information visible according to the phase

        /*
        if (GC.GetGamePhase() == 0)
        {
            if (hasDeployed)
            {
                moveRadius.gameObject.SetActive(true);
            }
        }
        */
        if (GC.GetGamePhase() == 1)
        {
            moveRadius = ShapeGenerator.GenerateCylinder(0, 40, move, 0.4f, 0);
            moveRadius.transform.position = gameObject.transform.position;
            moveRadius.gameObject.GetComponent<MeshRenderer>().material = moveMaterial;
            moveRadius.SetMaterial(moveRadius.faces, moveMaterial);
        }

        if (GC.GetGamePhase() == 3)
        {
            wpnPanel.SetActive(true);
        }

        if (GC.GetGamePhase() == 4 && charge != 0)
        {
            moveRadius = ShapeGenerator.GenerateCylinder(0, 40, charge, 0.4f, 0);
            moveRadius.transform.position = gameObject.transform.position;
            moveRadius.gameObject.GetComponent<MeshRenderer>().material = moveMaterial;
            moveRadius.SetMaterial(moveRadius.faces, moveMaterial);
        }

        if (GC.GetGamePhase() == 5) melPanel.SetActive(true);
    }

    public void Deselect()
    {
        //Remove the things unique to this model according to the phase

        //Debug.Log("Model Deselecting");
        if (GC.GetGamePhase() == 0 && cohesionRadius != null)
        {
            cohesionRadius.gameObject.SetActive(false);
        }

        if (GC.GetGamePhase() == 1 && hasMoved)
        {
            hasMoved = false;
            Destroy(cohesionRadius.gameObject);
            destination = transform.position;
            Destroy(planMove);
        }

        if (GC.GetGamePhase() == 3)
        {
            wpnPanel.SetActive(false);
        }

        if (GC.GetGamePhase() == 5) melPanel.SetActive(false);
    }

    public void ActiveDeselect()
    {
        //If this is the active model being deselected, remove things unique to this model according to the phase

        if(GC.GetGamePhase() == 1 && moveRadius != null || GC.GetGamePhase() == 4 && moveRadius != null)
        {
            Destroy(moveRadius.gameObject);
        }
        if (GC.GetGamePhase() == 3)
        {
            wpnPanel.SetActive(false);
        }
        if (GC.GetGamePhase() == 5) melPanel.SetActive(false);
        relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        relbutton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }

    public void ChangeModel()
    {
        //If a new model in this unit is being picked as the active model, remove the visuals for this model

        if (GC.GetGamePhase() == 1 || GC.GetGamePhase() == 4 && moveRadius != null) Destroy(moveRadius.gameObject);
        if (GC.GetGamePhase() == 3) wpnPanel.SetActive(false);
        if (GC.GetGamePhase() == 5) melPanel.SetActive(false);
    }

    public bool CheckMoved()
    {
        return hasMoved;
    }

    public bool CheckCharged()
    {
        return hasCharged;
    }

    public bool CheckCohesion()
    {
        Debug.Log("CheckingCohesion");
        if (GC.GetGamePhase() == 0)
        {
            foreach (Model model in unit.GetModels())
            {
                if (Vector3.Distance(destination, model.GetDestination()) <= 5 && model != this) return true;
            }
        }else if(GC.GetGamePhase() == 1)
        {
            foreach (Model model in unit.GetModels())
            {
                if (model.CheckMoved() && Vector3.Distance(destination, model.GetDestination()) <= 5 && model != this) return true;
            }
        }
        else
        {
            foreach (Model model in unit.GetModels())
            {
                if (model.CheckCharged() && Vector3.Distance(destination, model.GetDestination()) <= 5 && model != this) return true;
            }
        }
        Debug.Log("Cohesion Failed");
        return false;
    }

    public void Rename(string toName)
    {
        name = toName;
        if (GetComponent<PhotonView>().IsMine)
        {
            relbutton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = name;
        }
    }

    public void Rename()
    {
        name = origName;
        if (GetComponent<PhotonView>().IsMine)
        {
            relbutton.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = name;
        }
    }

    /*
    public void SetTarget()
    {
        if(currTarget != null)
        {
            //Find their button, and make it interactable. I'm actually thining of changing this to a dropdown for each weapon, so this might be removed
        }
        currTarget = toSet;
        foreach (Weapon weapon in weapons) weapon.SetTarget();
    }
    */
    public Vector3 GetDestination()
    {
        return destination;
    }

    public int GetMove()
    {
        return move;
    }

    public bool CheckDeployed()
    {
        return hasDeployed;
    }

    public void EndDeploy()
    {
        Destroy(cohesionRadius.gameObject);
    }

    public void EndActive()
    {
        relbutton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        relbutton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }

    [PunRPC]
    public void SetPlayer(int toSet)
    {
        playerID = toSet;
        Debug.Log("Set PlayerID as: " + playerID);
        if (PhotonNetwork.LocalPlayer.ActorNumber != toSet) foreach (Transform child in transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 8;
        else foreach (Transform child in transform.GetComponentInChildren<Transform>()) child.gameObject.layer = 10;
    }

    public GameObject GetRelButton()
    {
        return relbutton;
    }

    public void CheckState()
    {
        if(GC.GetGamePhase() == 0)
        {
            //Debug.Log("Gamephase 0, showing cohesionradius");
            if (hasDeployed) cohesionRadius.gameObject.SetActive(true);
        }
    }

    public void Iterate()
    {
        if((GC.GetGamePhase() == 0 || GC.GetGamePhase() == 1) && PhotonNetwork.CurrentRoom.Players[playerID].IsLocal && moveMaterial != null)
        {
            Destroy(moveRadius);
        }
    }

    public void ConfirmHit(Weapon toConfirm)
    {
        int temp = toConfirm.GetStrength();
        if(temp <= armor / 2)
        {
            temp = 50;
        }
        else if(temp < armor)
        {
            temp = 25;
        }
        else if (temp == armor)
        {
            temp = 0;
        }else if (temp >= armor * 2)
        {
            temp = -50;
        }
        else
        {
            temp = -25;
        }
        if(Random.Range(0, 201) + temp >= 100)
        {
            List<int> damage = toConfirm.GetDamage();
            if (damage.Count == 1) photonView.RPC("Damage", RpcTarget.All, damage[0]);
            else photonView.RPC("Damage", RpcTarget.All, Random.Range(damage[0], damage[1] + 1));
        }
    }

    [PunRPC]
    public void Damage(int toDamage)
    {
        health -= toDamage;
        if (health <= 0) Die();
        else if (health < origHealth) wounded = true;
        else wounded = false;
    }

    public void Die()
    {
        //ClearInMelee();
        unit.RemoveModel(this);
        unit.CheckMelee();
        if (photonView.IsMine) //this happens off an RPC, so only do this part for the player that controls this model;
        {
            Destroy(relbutton);
            PhotonNetwork.Destroy(photonView);
        }
    }

    public void PrepareCharge(Unit toCharge, int distance)
    {
        chargeTarget = toCharge;
        charge = distance;
    }

    /*
    public bool CheckActivated()
    {
        if(GC.GetGamePhase() == 1)
        {
            return hasMoved;
        }
        if(GC.GetGamePhase() == 2){
            return hasDoneSpecial;
        }
         
        if (GC.GetGamePhase() == 3)
        {
            return hasShot;
        }
        if(GC.GetGamePhase() == 4)
        {
            return hasCharged;
        }
        if(GC.GetGamePhase() == 5 && inMelee)
        {
            return hasPunched;
        }
        Debug.Log("hmmmmmmmm... check activated messed up");
        return true;
    }
    */

    public bool GetIsMoving()
    {
        return isMoving;
    }
}
