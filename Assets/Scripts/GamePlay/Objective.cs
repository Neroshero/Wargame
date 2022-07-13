using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Objective : MonoBehaviour
{

    Army player1Army;
    Army player2Army;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupArmies(Army player1, Army player2)
    {
        player1Army = player1;
        player2Army = player2;
    }

    public void UpdatePoints()
    {
        //update the current points values of the players depending on who controls this objective
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5);
        int player1ObSec = 0;
        int player2ObSec = 0;
        int player1Models = 0;
        int player2Models = 0;
        foreach (Collider collider in colliders)
        {
            Model model = collider.gameObject.GetComponent<Model>();
            if (model != null) //make sure we actually hit a model
            {
                int temp = model.GetPlayer();
                if (temp == 1) {
                    if (model.HasTrait("obSec"))
                    {
                        player1ObSec++;
                    }
                    player1Models++;
                }
                else
                {
                    if (model.HasTrait("obSec"))
                    {
                        player2ObSec++;
                    }
                    player2Models++;
                }
            }
        }
        Debug.Log("Player1ObSec: " +player1ObSec);
        Debug.Log("Player1Models: " +player1Models);
        Debug.Log("Player2ObSec: " +player2ObSec);
        Debug.Log("Player2Models: " +player2Models);
        if (player1ObSec != player2ObSec)
        {
            if (player1ObSec > player2ObSec)
            {
                player1Army.AddPoints(1);
            }
            else player2Army.AddPoints(1);
        }
        else if (player1Models != player2Models)
        {
            if (player1Models > player2Models)
            {
                player1Army.AddPoints(1);
            }
            else player2Army.AddPoints(1);
        }
        else Debug.Log("obSec and model numbers the same, no points change");
    }
}
