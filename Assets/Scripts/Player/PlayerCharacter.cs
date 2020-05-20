using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    [Header("Movement and Speeds")]
    public float speed = 10.0f;
    public float runSpeedIncrease = 5.0f;
    public float mainSpeed;

    [Header("Health")]
    public float health = 100f;
    public float maxHealth = 100f;

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

    public void TakeDamage(float d) {
        health -= d;
        if(health <= 0) {
            health = 0;
            Dead();
        }
    }

    public void Heal(float d) {
        health += d;
        if(health > maxHealth) {
            health = maxHealth;
        }
    }

    public void Dead() {
        print("You have died!");
    }
}