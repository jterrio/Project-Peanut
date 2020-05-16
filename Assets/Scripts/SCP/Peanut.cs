using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Peanut : SCP {

    public bool IsSeen { get; set; } = false;
    public float speed;
    public Collider col;
    private Grid grid;
    public Vector3 tryingToMoveTo;



    public Action action;

    void Start(){
        scpID = "173";
        c = Classification.Euclid;
        grid = GameManager.gm.peanutManager.GetComponent<Grid>();
    }

    void Update(){
    }

    private void FixedUpdate() {
        CanBeSeen();
        Move();
    }


    private void CanBeSeen() {
        
        bool inCamera = false;
        bool LOS = false;
        

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, col.bounds)) {
            inCamera = true;
        }

        RaycastHit hit;
        Vector3 direction = transform.position - GameManager.gm.player.transform.position;
        if (Physics.Raycast(GameManager.gm.player.transform.position, direction, out hit, Mathf.Infinity, GameManager.gm.peanut)) {
            //print(hit.collider.gameObject.name);
            if (hit.collider.gameObject.layer == 9) {
                Debug.DrawRay(GameManager.gm.player.transform.position, direction, Color.green);
                LOS = true;
            } else {
                Debug.DrawRay(GameManager.gm.player.transform.position, direction, Color.red);
            }
        }
        //print("In camera: " + inCamera);
        //print("LOS: " + LOS);
        //print("");
        if(inCamera && LOS) {
            IsSeen = true;
        } else {
            IsSeen = false;
        }
    }


    void MoveLerp() {
        while (grid.FinalPath.Count > 0) {
            float distance = 0f;
            Vector3 current = transform.position;
            List<Node> temp = new List<Node>(grid.FinalPath);
            for(int i = 0; i < grid.FinalPath.Count; i++) {
                float d = Vector3.Distance(current, grid.FinalPath[i].vPosition);
                if(distance + d < speed) {
                    distance += d;
                    current = grid.FinalPath[i].vPosition;
                    temp.RemoveAt(0);
                } else if(distance + d == speed) {
                    current = grid.FinalPath[i].vPosition;
                    temp.RemoveAt(0);
                    break;
                } else {
                    float percentage = (d - ((distance + d) - speed)) / d;
                    current = Vector3.Lerp(current, grid.FinalPath[i].vPosition, ((percentage - distance) / (distance - d)));
                    temp.RemoveAt(0);
                    break;
                }
            }
            grid.FinalPath = null;
            //print(current);
            transform.LookAt(current);
            transform.position = current;
            return;
        }
    }

    public void Move() {
        if (!IsSeen && grid.FinalPath != null) {
            MoveLerp();
        }
        //rb.velocity = Vector3.zero;
        //transform.LookAt(GameManager.gm.player.tr)
    }

    public bool HasReachedPosition(Vector3 pos) {
        return Vector3.Distance(transform.position, pos) <= 0.1f;
    }

    
}
