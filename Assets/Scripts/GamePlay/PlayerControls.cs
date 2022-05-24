using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerControls : MonoBehaviourPun
{

    public GameObject emptyArmy;

    private Army army;

    private int playerID;

    private GameController GC;

    private GameObject deploymentZone;


    //Load information
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

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            Debug.Log("Online");
            playerID = gameObject.GetPhotonView().Owner.ActorNumber;
            Debug.Log("PlayerID: " + playerID);
            //photonView.RPC("SetViewID", RpcTarget.All, playerID);
            if (PhotonView.Find(playerID * 1000 + 2) != null) army = PhotonView.Find(playerID * 1000 + 2).gameObject.GetComponent<Army>();
            else
            {
                factions = new Dictionary<string, string>{
                    { "0", "Army" },
                    { "1", "Corp" },
                    { "2", "CoA" },
                    { "3", "Reb" }
                };

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
                army = PhotonNetwork.Instantiate(emptyArmy.name, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Army>();
                //army = Instantiate(emptyArmy).GetComponent<Army>();
                string[] input = null;
                if (photonView.IsMine && PhotonNetwork.CurrentRoom.Players[playerID].IsLocal)
                {
                    Debug.Log("Loading my Army");
                    input = File.ReadAllLines(Application.streamingAssetsPath + "\\Armies\\" + (string)PhotonNetwork.LocalPlayer.CustomProperties["Army"] + ".txt");
                    StartCoroutine(Load(input, playerID));

                    //army.gameObject.GetPhotonView().RPC("Load", RpcTarget.All, input, playerID);
                }
            }
            //StartCoroutine(army.Load(input, playerID));
            /*
            Hashtable toSet = new Hashtable()
            {
                {playerID.ToString(), army}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(toSet);
            */
            GC = GameObject.Find("GameController").GetComponent<GameController>();
        }
        //GC.photonView.RPC("AddPlayer", RpcTarget.All, playerID, army.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) {
            /*manage vertical, horizontal, and zoomular(?) movement
            float horizontal = Input.GetAxis("Vertical");
            float vertical = -1*Input.GetAxis("Horizontal");
            float zoom = Input.GetAxis("Mouse ScrollWheel") * -1;


            Vector3 position = transform.position;
            position.x = position.x + horizontal * speed * Time.deltaTime;
            position.z = position.z + vertical * speed * Time.deltaTime;
            if (zoom != 0)
            {
                if (!zooming && position.y < 60)
                {
                    zooming = true;
                }
                else
                {
                    zoomSpeed+= 2;
                }
                position.y = position.y + zoom * zoomSpeed * Time.deltaTime;
                if (position.y > 60)
                {
                    position.y = 60;
                }
                else if (position.y < 10)
                {
                    position.y = 10;
                }
            }
            else
            {
                zooming = false;
                zoomSpeed = startZoomSpeed;
            }

            transform.position = position;

            manage rotation

            if (Input.GetKey(KeyCode.Q))
            {
                //rotate right
                transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.E))
            {
                //rotate left
                transform.Rotate(0, rotateSpeed * Time.deltaTime * -1, 0);
            }

            if (Input.GetKey(KeyCode.Z))
            {
                //rotate down
                transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(KeyCode.X))
            {
                //rotate up
                transform.Rotate(rotateSpeed * Time.deltaTime * -1, 0, 0);
            }
            */

            /*Mouse controls (and a somewhat random tab)

            if (Input.GetMouseButtonDown(0)) //&&is this player's turn
            {
                //set active unit
                RaycastHit hit = new RaycastHit();
                //check if we click on a model controlled by this player
                if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.tag == "model"
                    && hit.collider.gameObject.GetComponent<Model>().GetPlayer() == playerID)
                {
                    army.SetActive(hit.collider.gameObject.GetComponent<Model>().GetUnit(), hit.collider.gameObject.GetComponent<Model>());
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Tab)) //&&is this player's turn
            {
                army.SetActive();
            }
            */


            if (Input.GetKeyDown(KeyCode.Escape)) GC.ChangeMenu();

            if (!GC.GetMenuState()) {
                if (Input.GetMouseButtonDown(1)) //&&is this player's turn 
                {
                    RaycastHit hit = new RaycastHit();
                    Debug.Log("GC: " + GC);
                    //set deployment spot
                    if (GC.GetGamePhase() == 0)
                    {
                        if (!GC.GetDeployed(playerID - 1) && Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) && hit.collider.tag == "Deployment")
                        {
                            if (army.GetActive() != null)
                            {
                                Model model = army.GetActive().GetComponent<Unit>().GetActive().GetComponent<Model>();
                                model.photonView.RPC("Deploy", RpcTarget.All, hit.point);
                            }
                        }
                    }
                    //set move destination
                    if (GC.GetGamePhase() == 1 || GC.GetGamePhase() == 4)
                    {
                        if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit))
                        {
                            if (army.GetActive() != null)
                            {
                                Model model = army.GetActive().GetComponent<Unit>().GetActive().GetComponent<Model>();
                                if (GC.GetGamePhase() == 1) model.PlanMove(hit.point);
                                else model.PlanCharge(hit.point);
                            }
                        }
                    }
                }
                /*set active target 
                else if (GC.GetGamePhase() == 3 || GC.GetGamePhase() == 4 || GC.GetGamePhase() == 5)
                    if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.tag == "model"
                        && hit.collider.gameObject.GetComponent<Model>().GetPlayer() != playerID)
                    {
                        army.SetTarget(hit.collider.gameObject.GetComponent<Model>().GetUnit());
                    }
                    */
            }
        }
    }

    public int GetID()
    {
        return playerID;
    }

    public Army GetArmy()
    {
        return army;
    }

    public void SetDeploymentZone(GameObject toSet)
    {
        //Move the player's camera to begin from behind their randomly assigned deployment zone.
        GameObject camera = GameObject.Find("Main Camera");
        if(toSet.transform.localPosition.x == 0)
        {
            if (toSet.transform.localPosition.z < 0)
                camera.transform.localPosition = new Vector3(toSet.transform.localPosition.x, toSet.transform.localPosition.y + 25, toSet.transform.localPosition.z - 60);
            else
                camera.transform.localPosition = new Vector3(toSet.transform.localPosition.x, toSet.transform.localPosition.y + 25, toSet.transform.localPosition.z + 60);
        }
        else
        {
            if (toSet.transform.localPosition.x < 0)
                camera.transform.localPosition = new Vector3(toSet.transform.localPosition.x - 60, toSet.transform.localPosition.y + 25, toSet.transform.localPosition.z);
            else
                camera.transform.localPosition = new Vector3(toSet.transform.localPosition.x + 60, toSet.transform.localPosition.y + 25, toSet.transform.localPosition.z);
        }
        camera.transform.LookAt(new Vector3(0, 10, 0));
        deploymentZone = Instantiate(toSet);
    }

    public void RemoveDeploymentZone()
    {
        Destroy(deploymentZone);
    }

    /*
    [PunRPC]
    private void SetViewID(int toSet)
    {
        Debug.Log("Setting viewID as: " + toSet);
        if (photonView.IsMine)
        {
            photonView.ViewID = playerID;
            Debug.Log("I am settimg myself");
        }
        else
        {
            photonView.ViewID = toSet;
            Debug.Log("I am setting the other");
        }
        Debug.Log("Set Player " + toSet + "as player " + toSet);
    }
    */

    public IEnumerator Load(string[] input, int toSet)
    {
        //Load the army when generated

        yield return new WaitForEndOfFrame();
        //toDo (pretty much just copy my LoadArmy script here, changed to be Units instead of UnitCreators and Models instead of ModelCreators)
        List<int> units = new List<int>();
        List<int> temp = new List<int>();
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
                    temp = new List<int>();
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    Debug.Log(input[pointer]);
                    string unitName = ArmyUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\Army\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit.photonView.ViewID);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.photonView.RPC("SetArmy", RpcTarget.All, army.photonView.ViewID);
                    yield return new WaitForEndOfFrame();
                    //I'll check for custom name here, but later
                    AU.Rename();
                    AU.photonView.RPC("SetPlayer", RpcTarget.All, toSet);
                    //AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForEndOfFrame();
                        //AU.AddModel(model);
                        //I'll check for custom name here, but later
                        model.Rename();
                        temp.Add(model.photonView.ViewID);
                        pointer++;
                    }
                    AU.photonView.RPC("EndEditing", RpcTarget.All, temp.ToArray());
                }
            }
            else if (faction == "Corp")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    temp = new List<int>();
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    string unitName = CorpUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\Corp\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit.photonView.ViewID);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.photonView.RPC("SetArmy", RpcTarget.All, army.photonView.ViewID);
                    yield return new WaitForEndOfFrame();
                    //I'll check for custom name here, but later
                    AU.Rename();
                    AU.photonView.RPC("SetPlayer", RpcTarget.All, toSet);
                    //AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForSeconds(4);;
                        //I'll check for custom name here, but later
                        model.Rename();
                        temp.Add(model.photonView.ViewID);
                        pointer++;
                    }
                    AU.photonView.RPC("EndEditing", RpcTarget.All, temp.ToArray());
                }
            }
            else if (faction == "CoA")
            {
                for (int x = 0; x < numUnits; x++)
                {
                    temp = new List<int>();
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    string unitName = CoAUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\CoA\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit.photonView.ViewID);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.photonView.RPC("SetArmy", RpcTarget.All, army.photonView.ViewID);
                    yield return new WaitForEndOfFrame();
                    //I'll check for custom name here, but later
                    AU.Rename();
                    AU.photonView.RPC("SetPlayer", RpcTarget.All, toSet);
                    //AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForSeconds(4);
                        //Guess what I'll do here later, that's right, check for custom name
                        model.Rename();
                        temp.Add(model.photonView.ViewID);
                        pointer++;
                    }
                    AU.photonView.RPC("EndEditing", RpcTarget.All, temp.ToArray());
                }
            }
            else
            {
                for (int x = 0; x < numUnits; x++)
                {
                    temp = new List<int>();
                    //Units need to be photon instantiated because they'll have photon views for rpc functions
                    string unitName = RebUnits[input[pointer]].name;
                    pointer++;
                    Debug.Log("Attempting to instantiate unit");
                    Unit unit = PhotonNetwork.Instantiate("Units\\Reb\\" + unitName, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Unit>();
                    Debug.Log("Successfuly instantiated unit");
                    units.Add(unit.photonView.ViewID);
                    int numModels = int.Parse(input[pointer]);
                    pointer++;
                    Unit AU = unit.GetComponent<Unit>();
                    AU.photonView.RPC("SetArmy", RpcTarget.All, army.photonView.ViewID);
                    yield return new WaitForEndOfFrame();
                    //sorry about that last one, I needed a break for typing the same thing, anyway, custom name check goes here later
                    AU.Rename();
                    AU.photonView.RPC("SetPlayer", RpcTarget.All, toSet);
                    //AU.SetPlayer(toSet);
                    List<Model> modelChecker = AU.GetLegalModels();
                    for (int c = 0; c < numModels; c++)
                    {
                        Debug.Log("Attempting to instantiate model");
                        Model model = PhotonNetwork.Instantiate("Models\\" + unitName + "\\" + modelChecker[int.Parse(input[pointer])].name,
                            new Vector3(0, -200, 0), new Quaternion(0, 0, 0, 0)).GetComponent<Model>();
                        Debug.Log("Successfuly instantiated model");
                        yield return new WaitForEndOfFrame();
                        //I'll check for custom name here, but later
                        model.Rename();
                        temp.Add(model.photonView.ViewID);
                        pointer++;
                    }
                    AU.photonView.RPC("EndEditing", RpcTarget.All, temp.ToArray());
                }
            }
        }
        army.photonView.RPC("SetUnits", RpcTarget.All, units.ToArray(), playerID);
    }

}
