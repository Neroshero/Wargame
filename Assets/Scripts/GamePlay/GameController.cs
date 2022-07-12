using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameController : MonoBehaviourPunCallbacks
{
    //this class handles all the game information that needs to be synced between players during gameplay

    private int gamePhase; // will move from 0-6, then back to 1, to represent the current phase of gameplay
    private int lastPhase; // used to remember what the previous phase before damage was
    private int speedPhase; // will move from 1-7, then back to 1, to represent the current speedclass of active units
    private int playerTurn; //will alternate between 1 and 2 to represent the current player's turn
    private bool doDamage;

    //public Material toUse;
    private Dictionary<int, GameObject> ownership;
    private int initalPlayer; //keeps track of who was initially selected to go first for use in later phases
    private List<bool> deploymentReady; // should keep track of each player finishing deployment
    private List<bool> damageDone; //keeps track of each player finishing assigning damage

    [SerializeField]
    private List<GameObject> deploymentA;
    [SerializeField]
    private List<GameObject> deploymentB;
    private List<List<GameObject>> deploymentZones;
    private bool hasSetup;

    [SerializeField]
    private GameObject speedPhaseText;
    private bool speedPhaseChecked;
    [SerializeField]
    private GameObject playerTurnText;
    [SerializeField]
    private GameObject gamePhaseText;
    private List<string> gamePhases;

    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject menu;

    private PlayerControls player1;
    private PlayerControls player2;

    // Start is called before the first frame update
    void Start()
    {
        gamePhases = new List<string>()
        {
            "movement",
            "special",
            "shooting",
            "charge",
            "melee",
            "morale",
            "damage"
        };
        hasSetup = false;
        speedPhaseChecked = false;
        ownership = new Dictionary<int, GameObject>();
        gamePhase = 0;
        speedPhase = 1;
        deploymentReady = new List<bool>() { false, false };
        deploymentZones = new List<List<GameObject>>
        {
            deploymentA,
            deploymentB
        };
        doDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasSetup && PhotonView.Find(1001) != null && PhotonView.Find(2001) != null && PhotonView.Find(1002) != null && PhotonView.Find(2002) != null)
        {
            hasSetup = true;
            //playerTurn = Random.Range(1, 3);
            playerTurn = 1; //Just this way for now so I can have easy access to the console;
            int depZone = Random.Range(0, 2);
            int temp = Random.Range(0, 2);
            if (PhotonNetwork.LocalPlayer.IsMasterClient) photonView.RPC("SetupDeployment", RpcTarget.All, playerTurn, depZone, temp);
        }
    }

    /*
    [PunRPC]
    public void AddArmy(int toSet, Army toArmy)
    {
        if (ownership == null)
        {
            ownership = new Dictionary<int, GameObject>();
        }
        ownership.Add(toSet, toArmy.gameObject);
        Debug.Log("Curr Ownership: " + ownership);
    }
    */


    [PunRPC]
    public void UpdateSight()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ownership[1].GetPhotonView().RPC("CheckSight", RpcTarget.All, ownership[2].GetPhotonView().ViewID);
            ownership[2].GetPhotonView().RPC("CheckSight", RpcTarget.All, ownership[1].GetPhotonView().ViewID);
        }
    }

    [PunRPC]
    public void UpdateSight(int toCheck, int controller)
    {
        if(controller == 1)
        {
            ownership[1].GetComponent<Army>().CheckSight(ownership[2].GetPhotonView().ViewID, toCheck);
            ownership[2].GetComponent<Army>().CheckFor(ownership[1].GetPhotonView().ViewID, toCheck);
        }
        else
        {
            ownership[2].GetComponent<Army>().CheckSight(ownership[1].GetPhotonView().ViewID, toCheck);
            ownership[1].GetComponent<Army>().CheckFor(ownership[2].GetPhotonView().ViewID, toCheck);
        }
    }

    [PunRPC]
    public void Iterate()
    {
        //handle the transition between each gamephase

        Debug.Log("Before Iterating");
        if (gamePhase == 0)
        {
            player1.RemoveDeploymentZone();
            player2.RemoveDeploymentZone();
            UpdateSight();
            gamePhase++;
            CheckSpeedPhase();
            /*
            int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
            if(localPlayer == playerTurn) playerTurnText.GetComponent<Text>().text = "It is your turn";
            else playerTurnText.GetComponent<Text>().text = "It not is your turn";
            playerTurnText.SetActive(true);
            speedPhaseText.GetComponent<Text>().text = "It is currently speedphase 1";
            ownership[1].GetComponent<Army>().Iterate();
            ownership[2].GetComponent<Army>().Iterate();
            gamePhase++;
            */
        }
        else if(gamePhase == 5)
        {
            ownership[1].GetComponent<Army>().Restart();
            ownership[2].GetComponent<Army>().Restart();
            gamePhase = 1;
            speedPhase = 1; //The solution to several hours of debugging an infinite loop... Definitely my least favorite thing to debug...
            playerTurn = initalPlayer;
            Debug.Log(playerTurn);
            CheckSpeedPhase();
            gamePhaseText.GetComponent<Text>().text = "It is currently the " + gamePhases[gamePhase - 1] + " phase";
            return;
            /*
            int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
            if (localPlayer == playerTurn) playerTurnText.GetComponent<Text>().text = "It is your turn";
            else playerTurnText.GetComponent<Text>().text = "It is not your turn";
            playerTurnText.SetActive(true);
            speedPhaseText.GetComponent<Text>().text = "It is currently speedphase 1";
            */
        }
        else
        {
            ownership[1].GetComponent<Army>().Iterate();
            ownership[2].GetComponent<Army>().Iterate();
            speedPhase = 1;
            gamePhase++;
            playerTurn = initalPlayer;
            if (gamePhase == 2) gamePhase+=1; //Remember to remove when 'special' phase is implemented
            CheckSpeedPhase();
            Debug.Log(playerTurn);
            /*
            int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
            if (localPlayer == playerTurn) playerTurnText.GetComponent<Text>().text = "It is your turn";
            else playerTurnText.GetComponent<Text>().text = "It is not your turn";
            playerTurnText.SetActive(true);
            speedPhaseText.GetComponent<Text>().text = "It is currently speedphase 1";
            */
        }
        gamePhaseText.GetComponent<Text>().text = "It is currently the " + gamePhases[gamePhase - 1] + " phase";
    }

    public bool CheckGameOver()
    {
        return false; //Muwahahaaaa, the game never ends! (Not true, this isn't implemented yet, I just know it'll need to be a boolean. Actually, maybe not... I might just do things here if true)
    }
    /*
    public void Test()
    {
        ProBuilderMesh temp = ShapeGenerator.GenerateCylinder(0, 40, 10, 4, 0);
        temp.transform.position = new Vector3(21.33f, 0.505f, -5.1f);
        temp.SetMaterial(temp.faces, toUse);
    }
    */
    public int GetGamePhase()
    {
        return gamePhase;
    }

    public int GetSpeedPhase()
    {
        return speedPhase;
    }

    public int GetPlayerTurn()
    {
        return playerTurn;
    }

    public void Confirm()
    {

        //Make sure only the active player called confirm before attempting to actually confirm their orders

        if (gamePhase == 0 && !deploymentReady[PhotonNetwork.LocalPlayer.ActorNumber -1]
            || gamePhase == 7 && !damageDone[PhotonNetwork.LocalPlayer.ActorNumber -1] 
            || gamePhase != 0 && gamePhase != 7 && PhotonNetwork.LocalPlayer.ActorNumber == playerTurn) ActualConfirm(PhotonNetwork.LocalPlayer.ActorNumber);
        else Debug.Log("Inactive player called confirm");
    }

    //[PunRPC]
    public void ActualConfirm(int toUse)
    {
        //Carry out the active player's orders, and change the active player

        Army army = ownership[toUse].GetComponent<Army>();
        Debug.Log(army);
        if (army.ConfirmOrders()) //ensure the active player's orders can be carried out
        {
            if (gamePhase == 0)
            {
                /*
                deploymentReady[toUse - 1] = true;
                foreach (bool check in deploymentReady)
                {
                    if (!check)
                    {
                        Debug.Log("Player " + deploymentReady.IndexOf(check) + 1 + " not ready");
                        return;
                    }
                    
                }
                */
                army.EndDeploy();
                photonView.RPC("UpdateDeployment", RpcTarget.All, toUse - 1);
                foreach (bool check in deploymentReady)
                {
                    if (!check)
                    {
                        Debug.Log("Player " + (deploymentReady.IndexOf(check) + 1) + " not ready");
                        return;
                    }
                }
                photonView.RPC("Iterate", RpcTarget.All);
                return;
            }
            else if(gamePhase == 7)
            {
                photonView.RPC("UpdateDamage", RpcTarget.All, toUse - 1);
                foreach (bool check in damageDone)
                {
                    if (!check)
                    {
                        Debug.Log("Player " + (damageDone.IndexOf(check) + 1) + " not ready");
                        return;
                    }
                }
                //return to the previous phase
                photonView.RPC("CheckSpeedPhase", RpcTarget.All);
            }
            else
            {
                Debug.Log("Before Updating SpeedPhase");
                photonView.RPC("UpdatePlayers", RpcTarget.All);
                photonView.RPC("CheckSpeedPhase", RpcTarget.All);
                return;
            }
        }
        Debug.Log("Orders could not be verified");
    }

    [PunRPC]
    private void UpdatePlayers()
    {
        if (playerTurn == 1) playerTurn = 2;
        else playerTurn = 1;
    }

    [PunRPC]
    private void UpdateDeployment(int toUse)
    {
        deploymentReady[toUse] = true;
    }

    [PunRPC]
    private void UpdateDamage(int toUse)
    {
        damageDone[toUse] = true;
    }

    /*
    [PunRPC]
    public void UpdateTurn()
    {
        if(gamePhase == 0)
        {
            Iterate();
            return;
        }
        Army army1 = ownership[1].GetComponent<Army>();
        Debug.Log("Army 1 Established: " + army1);
        Army army2 = ownership[2].GetComponent<Army>();
        Debug.Log("Army 2 Established: " + army2);
        if (gamePhase == 1)
        {
            if (playerTurn == 0)
            {
                foreach (Unit unit in army1.GetUnits())
                {
                    if (unit.GetHasMoved())
                    {
                        playerTurn = 1;
                        return;
                    }
                }
                foreach (Unit unit in army2.GetUnits())
                {
                    if (unit.GetHasMoved())
                    {
                        return;
                    }
                }
                speedPhase++;
            }
            else
            {
                foreach (Unit unit in army2.GetUnits())
                {
                    if (unit.GetHasMoved())
                    {
                        playerTurn = 0;
                        return;
                    }
                }
                foreach (Unit unit in army1.GetUnits())
                {
                    if (unit.GetHasMoved())
                    {
                        return;
                    }
                }
                speedPhase++;
            }
            if (speedPhase == 7)
            {
                Iterate();
                return;
            }
        }
    }
    */
    [PunRPC]
    private void SetupDeployment(int turn, int depZone, int temp)
    {
        //assign player deployment zones

        playerTurn = turn;
        initalPlayer = playerTurn;
        Debug.Log(playerTurn);
        List<GameObject> toUse = deploymentZones[depZone];
        player1 = PhotonView.Find(1001).gameObject.GetComponent<PlayerControls>();
        Debug.Log("Player 1: "+ player1);
        player2 = PhotonView.Find(2001).gameObject.GetComponent<PlayerControls>();
        Debug.Log("Player 2: " + player2);
        if (playerTurn == 1)
        {
            if (PhotonNetwork.CurrentRoom.GetPlayer(1).IsLocal) player1.SetDeploymentZone(toUse[temp]);
            else
            {
                if (temp == 1) player2.SetDeploymentZone(toUse[0]);
                else player2.SetDeploymentZone(toUse[1]);
            }
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.GetPlayer(2).IsLocal) player2.SetDeploymentZone(toUse[temp]);
            else
            {
                if (temp == 1) player1.SetDeploymentZone(toUse[0]);
                else player1.SetDeploymentZone(toUse[1]);
            }
        }
        Debug.Log(player1.GetArmy());
        Debug.Log(player2.GetArmy());
        ownership.Add(1, player1.GetArmy().gameObject);
        ownership.Add(2, player2.GetArmy().gameObject);
        Debug.Log("Ownership: " + ownership);
        Debug.Log("Player 1's Army: " + ownership[1]);
        Debug.Log("Player 2's Army: " + ownership[2]);
    }

    [PunRPC]
    public void CheckSpeedPhase()
    {
        //check to see if the speedphase, and consequently, the gamephase, need to change

        Debug.Log("Checking speedPhase for phase " + gamePhases[gamePhase - 1]);
        Army toCheck = ownership[playerTurn].GetComponent<Army>();
        List<Unit> toUnit = toCheck.GetUnits();
        List<Unit> toConfirm = new List<Unit>();
        if(gamePhase == 7)//as this marks the end of the damage phase, check if either army has been wiped out
        {
            int first = ownership[1].GetComponent<Army>().CheckGameOver();
            int second = ownership[2].GetComponent<Army>().CheckGameOver();
            if(first != -1 && second != -1)
            {
                GameOver();
                return;
            }
            if(first != -1)
            {
                GameOver(first);
                return;
            }
            if(second != -1)
            {
                GameOver(second);
                return;
            }
            gamePhase = lastPhase;
            speedPhase++;
            playerTurn = initalPlayer;
            playerTurnText.SetActive(true);
        }
        foreach (Unit unit in toUnit)
        {
            if (unit.GetSpeedClass() == speedPhase) toConfirm.Add(unit); //get the list of units that can activate in this speedphase
        }
        if (toConfirm.Count == 0)
        {
            Debug.Log("None can activate this phase");
            if (!speedPhaseChecked) //if none of the active player's units can activate in this speedphase, move to the other player
            {
                speedPhaseChecked = true;
                if (playerTurn == 1) playerTurn = 2;
                else playerTurn = 1;
                CheckSpeedPhase();
            }
            else
            {
                speedPhaseChecked = false;
                if (doDamage)
                {
                    //move to the damage phase
                    Debug.Log("Moving to Damage Phase");
                    lastPhase = gamePhase;
                    gamePhase = 7;
                    gamePhaseText.GetComponent<Text>().text = "It is currently the " + gamePhases[gamePhase - 1] + " phase";
                    doDamage = false;
                    damageDone = new List<bool>() { false, false };
                    playerTurnText.SetActive(false);
                    return;
                }
                if (speedPhase == 7)
                {
                    //move to the next gamephase
                    Iterate();
                    return;
                }
                else
                {
                    //move to the next speedphase and see if there are units that can be activated
                    speedPhase++;
                    CheckSpeedPhase();
                }
            }
        }
        else
        {
            Debug.Log(toConfirm.Count);
            int[] temp = new int[toConfirm.Count];
            foreach (Unit unit in toConfirm)// check if the units that can activate in this speedphase have already activated
            {
                if (unit.CheckActivated())
                {
                    Debug.Log("This unit activated");
                    temp[toConfirm.IndexOf(unit)] = 1;
                }
            }
            Debug.Log(temp.Length);
            for (int i = temp.Length - 1; i >= 0; i--)
            {
                Debug.Log(i);
                Debug.Log(toConfirm[i]);
                if (temp[i] == 1) toConfirm.RemoveAt(i);
            }
            Debug.Log(toConfirm.Count);
            if (toConfirm.Count == 0)
            {
                Debug.Log("None left who can activate");
                if (!speedPhaseChecked)
                {
                    speedPhaseChecked = true;
                    if (playerTurn == 1) playerTurn = 2;
                    else playerTurn = 1;
                    CheckSpeedPhase();
                }
                else
                {
                    speedPhaseChecked = false;
                    if (doDamage)
                    {
                        Debug.Log("Moving to Damage Phase");
                        lastPhase = gamePhase;
                        gamePhase = 7;
                        gamePhaseText.GetComponent<Text>().text = "It is currently the " + gamePhases[gamePhase - 1] + " phase";
                        doDamage = false;
                        damageDone = new List<bool>() { false, false };
                        playerTurnText.SetActive(false);
                        return;
                    }
                    if (speedPhase == 7)
                    {

                        Iterate();
                        return;
                    }
                    else
                    {
                        speedPhase++;
                        CheckSpeedPhase();
                    }
                }
            }
        }
        int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
        if (localPlayer == playerTurn) playerTurnText.GetComponent<Text>().text = "It is your turn";
        else playerTurnText.GetComponent<Text>().text = "It is not your turn";
        playerTurnText.SetActive(true);
        speedPhaseText.GetComponent<Text>().text = "It is currently speedphase " + speedPhase;
    }

    public bool GetDeployed(int toCheck)
    {
        return deploymentReady[toCheck];
    }
    /*
    public void SetTarget()
    {
        ownership[PhotonNetwork.LocalPlayer.ActorNumber].GetComponent<Army>().SetTarget();
    }
    */
    [PunRPC]
    public void DoDamage(bool val)
    {
        doDamage = val;
    }

    [PunRPC]
    public void GameOver(int loser)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == loser) losePanel.SetActive(true);
        else winPanel.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Panel").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Button").gameObject.SetActive(false);
        playerTurnText.SetActive(false);
        gamePhaseText.SetActive(false);
        speedPhaseText.SetActive(false);
    }

    public void GameOver()
    {
        losePanel.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Panel").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Button").gameObject.SetActive(false);
        playerTurnText.SetActive(false);
        gamePhaseText.SetActive(false);
        speedPhaseText.SetActive(false);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void ChangeMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    public bool GetMenuState()
    {
        return menu.activeSelf;
    }
}
