using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    [Header("Movement and Speeds")]
    public float speed = 10.0f;
    public float runSpeedIncrease = 5.0f;
    public float mainSpeed;
    private float translation;
    private float straffe;
    public MovementState moveState;


    public enum MovementState {
        SLOW,
        WALK,
        RUN
    }


    // Use this for initialization
    void Start() {
        moveState = MovementState.WALK;
        mainSpeed = speed;
        //log(isRunning);
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape")) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
        //Methods from Sprint Class.
        //Sprint.UpdateStamina(mainSpeed);

        //Sprint.Sprinting(speed, runSpeedIncrease, toggleSprint, mainSpeed);
    }

    public bool IsMoving() {
        return (translation != 0 || straffe != 0) ;
    }
}