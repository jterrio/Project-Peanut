using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : AggroSCP {

    void Start() {
        scpID = "049";
        c = Classification.Euclid;
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        Move();
    }

}
