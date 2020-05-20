using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AggroSCP : SCP {

    public float speed;
    protected Coroutine moveLerpCoroutine;
    public GameObject target;
    protected Rigidbody rb;

    protected virtual IEnumerator MoveLerp() {
        yield return new WaitForSeconds(0.1f);
        List<Vector3> finalPath = GameManager.gm.AI.FindPath(transform.position, target.transform.position);
        if (finalPath.Count > 0) {
            Vector3 move = (finalPath[0] - transform.position).normalized * speed;
            rb.velocity = move;
            transform.LookAt(target.transform.position);
        } else {
            rb.velocity = Vector3.zero;
        }
        moveLerpCoroutine = null;
    }

    protected virtual void Move() {
        if(moveLerpCoroutine == null && target != null) {
            moveLerpCoroutine = StartCoroutine(MoveLerp());
        }
    }


    public bool HasReachedPosition(Vector3 pos) {
        return Vector3.Distance(transform.position, pos) <= 0.1f;
    }

}
