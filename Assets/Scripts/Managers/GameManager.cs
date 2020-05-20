using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{

    public static GameManager gm;
    public GameObject player;
    public LayerMask peanut;
    public BlinkUI blink;

    public GameObject peanutManager;
    public Pathfinding AI;

    // Start is called before the first frame update
    void Start(){
        blink = player.GetComponent<BlinkUI>();
        if (gm == null) {
            gm = this;
        }else if(gm != this) {
            Destroy(this);
        }
        AI = GetComponent<Pathfinding>();
    }
}
