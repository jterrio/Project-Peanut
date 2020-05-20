using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCP : MonoBehaviour {
    [HideInInspector]
    public string scpID;
    [HideInInspector]
    public Classification c;

    public enum Classification {
        Safe,
        Euclid,
        Keter
    }


    public string GetSCPName() {
        return "SCP-" + scpID;
    }

}
