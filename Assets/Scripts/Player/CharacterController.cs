/* 
 * author : jiankaiwang
 * description : The script provides you with basic operations of first personal control.
 * platform : Unity
 * date : 2017/12
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    [Header("Movement and Speeds")]
    public float speed = 10.0f;
    public float runSpeedIncrease = 5.0f;
    public bool toggleSprint = false;
    public Sprint Sprint;
    private float mainSpeed;
    private float translation;
    private float straffe;

    // Use this for initialization
    void Start() {
        mainSpeed = speed;
        //log(isRunning);
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        // Input.GetAxis() is used to get the user's input
        // You can furthor set it on Unity. (Edit, Project Settings, Input)
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape")) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
        //Methods from Sprint Class.
        Sprint.UpdateStamina(mainSpeed);
        Sprint.Sprinting(speed, runSpeedIncrease, toggleSprint, mainSpeed);
    }
}