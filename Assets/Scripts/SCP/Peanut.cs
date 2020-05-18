using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Peanut : SCP {

    public bool IsSeen { get; set; } = false;
    public float speed;
    public CapsuleCollider col;
    private Grid grid;
    public Vector3 tryingToMoveTo;



    public Action action;

    void Start(){
        scpID = "173";
        c = Classification.Euclid;
        grid = GameManager.gm.peanutManager.GetComponent<Grid>();
    }

    void Update(){
        CanBeSeen();
        Move();
    }

    void CanBeSeen() {
        
        bool inCamera = false;
        bool LOS = false;

        List<Vector3> pointsToCheck = new List<Vector3> {
            //CENTER
            transform.position,
            //TOP
            new Vector3(transform.position.x, transform.position.y + (col.height / 2), transform.position.z),
            //LEFT
            new Vector3(transform.position.x - col.radius, transform.position.y, transform.position.z),
            //RIGHT
            new Vector3(transform.position.x + col.radius, transform.position.y, transform.position.z),
            //LEFT-KNEE
            new Vector3(transform.position.x - col.radius, transform.position.y - (col.height / 4), transform.position.z),
            //RIGHT-KNEE
            new Vector3(transform.position.x + col.radius, transform.position.y - (col.height / 4), transform.position.z),
            //LEFT-SHOULDER
            new Vector3(transform.position.x - col.radius, transform.position.y + (col.height / 4), transform.position.z),
            //RIGHT-SHOULDER
            new Vector3(transform.position.x + col.radius, transform.position.y + (col.height / 4), transform.position.z),
            //LOWER-CHEST
            new Vector3(transform.position.x, transform.position.y + (col.height / 6), transform.position.z),
            //BELLY
            new Vector3(transform.position.x, transform.position.y - (col.height / 6), transform.position.z),
            //UPPER-CHEST
            new Vector3(transform.position.x, transform.position.y + (col.height / 3), transform.position.z),
            //PP
            new Vector3(transform.position.x, transform.position.y - (col.height / 3), transform.position.z)
        };


        /*
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, col.bounds)) {
            inCamera = true;
        }*/

        foreach (Vector3 v in pointsToCheck) {
            if (IsPointOnScreen(v)) {
                inCamera = true;
            }
            RaycastHit hit;
            Vector3 direction = v - GameManager.gm.player.transform.position;
            if (Physics.Raycast(GameManager.gm.player.transform.position, direction, out hit, Mathf.Infinity, GameManager.gm.peanut)) {
                //print(hit.collider.gameObject.name);
                if (hit.collider.gameObject.layer == 9) {
                    Debug.DrawRay(GameManager.gm.player.transform.position, direction, Color.green);
                    LOS = true;
                } else {
                    Debug.DrawRay(GameManager.gm.player.transform.position, direction, Color.red);
                }
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

    bool IsPointOnScreen(Vector3 v) {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(v);
        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
            return true;
        }
        return false;
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

    void Move() {
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
