using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Peanut : AggroSCP {

    public bool IsSeen { get; set; } = false;

    [HideInInspector]
    public CapsuleCollider col;

    void Start(){
        scpID = "173";
        c = Classification.Euclid;
        col = GetComponent<CapsuleCollider>();
    }

    void Update(){
        CanBeSeen();
        Move();
        AttemptDamage();
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


    protected override IEnumerator MoveLerp() {
        yield return new WaitForSeconds(0.1f);
        List<Vector3> finalPath = GameManager.gm.AI.FindPath(transform.position, target.transform.position, 3);  
        while (finalPath.Count > 0) {
            float distance = 0f;
            Vector3 current = transform.position;
            List<Vector3> temp = new List<Vector3>(finalPath);
            for(int i = 0; i < finalPath.Count; i++) {
                float d = Vector3.Distance(current, finalPath[i]);
                if(distance + d < speed) {
                    distance += d;
                    current = finalPath[i];
                    temp.RemoveAt(0);
                } else if(distance + d == speed) {
                    current = finalPath[i];
                    temp.RemoveAt(0);
                    break;
                } else {
                    float percentage = (d - ((distance + d) - speed)) / d;
                    current = Vector3.Lerp(current, finalPath[i], ((percentage - distance) / (distance - d)));
                    temp.RemoveAt(0);
                    break;
                }
            }
            transform.LookAt(current);
            transform.position = current;
            break;
        }
        moveLerpCoroutine = null;
    }

    protected override void Move() {
        if (!IsSeen && moveLerpCoroutine == null && target != null) {
            moveLerpCoroutine = StartCoroutine(MoveLerp());
        }
    }

    public override void AttemptDamage() {
        if (!IsSeen) {
            base.AttemptDamage();
        }
    }


}
