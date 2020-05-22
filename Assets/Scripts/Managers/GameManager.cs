using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{

    public static GameManager gm;
    public GameObject player;
    public PlayerCharacter pc;
    public LayerMask peanut;
    public BlinkUI blink;
    public Pathfinding AI;

    // Start is called before the first frame update
    void Start(){
        if (gm == null) {
            gm = this;
        }else if(gm != this) {
            Destroy(this);
        }
        AI = GetComponent<Pathfinding>();
        pc = player.GetComponent<PlayerCharacter>();
        blink = player.GetComponent<BlinkUI>();
    }
}
