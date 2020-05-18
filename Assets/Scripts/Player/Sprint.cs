using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint : MonoBehaviour
{
    [Header("Stamina")]
    public double stamina = 100;
    [Tooltip("Per update call")]
    public double staminaDrainRate = .1;
    [Tooltip("Per update call")]
    public double staminaRefillRate = .03;
    public double staminaRequireToRun = 20;
    [Tooltip("Seconds")]
    public float timeTillStaminaRegen = 3f;
    private double maxStamina;
    private bool isRunning = false;
    private bool justStopped = false;
    private Coroutine timer;
    public CharacterController characterController;

    void Start() {
        maxStamina = stamina;
    }

    /// <summary>
    /// To handle all sprinting for the character in a neat one line function
    /// </summary>
    /// <param name="speed">base speed of the character</param>
    /// <param name="runSpeedIncrease">have much more speed the user wants to increase the base walk speed by</param>
    /// <param name="toggleSprint">Determins what type of sprinting the character wants to use</param>
    public void Sprinting(float speed, float runSpeedIncrease, bool toggleSprint, float mainSpeed) {
        //Determining if they are standing still
        if (false) {
            // todo: determine if they are standing still
        } else {
            //If they want toggleSprint or holdSprint.
            if (toggleSprint) {
                if (stamina < staminaRequireToRun && Input.GetKeyDown(KeyCode.LeftShift) && !isRunning)  {
                    // do nothing because you can't start running right now
                } else if (Input.GetKeyDown(KeyCode.LeftShift) && !isRunning) {
                    characterController.speed += runSpeedIncrease;
                    isRunning = true;
                } else if(Input.GetKeyDown(KeyCode.LeftShift)) {
                    // You are currently running. So you need to stop running.
                    characterController.speed = mainSpeed;
                    isRunning = false;
                    justStopped = true;
                    if (!(timer == null)) {
                        RestartStamTimer();
                    }
                }
            } else {
                //Set to sprint speed
                if (Input.GetKeyDown(KeyCode.LeftShift)) {
                    if (stamina > staminaRequireToRun) {
                        characterController.speed += runSpeedIncrease;
                        isRunning = true;
                    } else {
                        // do nothing because you can't start running right now
                    }
                }
                //Reset speed to original
                if (Input.GetKeyUp(KeyCode.LeftShift)) {
                    characterController.speed = mainSpeed;
                    isRunning = false;
                    justStopped = true;
                    if (!(timer == null)) {
                        RestartStamTimer();
                    }
                }
            }
        }
    }

    /// <summary>
    /// The goal is for it to update the stamina every tick and equalize and bad values.
    /// </summary>
    public void UpdateStamina(float mainSpeed) {
        if (justStopped && (timer == null)) {
            timer = StartCoroutine(WaitASec(timeTillStaminaRegen));
        } else if (justStopped) {
            //Started Timer just waiting
            if (isRunning && (stamina <= 0)) {
                stamina = 0;
                characterController.speed = mainSpeed;
                isRunning = false;
                RestartStamTimer();
            } else if (isRunning) {
                stamina -= staminaDrainRate;
            } else {
                if (stamina > maxStamina) {
                    stamina = maxStamina;
                } else if (stamina == maxStamina) {
                    // do nothing, stamina fine
                } else {
                    // do nothing still waiting for cooldown on stamina regen
                }
            }
        }
        else {
            //Can Regen Stamina
            if (isRunning && (stamina <= 0)) {
                stamina = 0;
                characterController.speed = mainSpeed;
                isRunning = false;
                justStopped = true;
            } else if (isRunning) {
                stamina -= staminaDrainRate;
            } else {
                if (stamina > maxStamina) {
                    stamina = maxStamina;
                } else if (stamina == maxStamina) {
                    // do nothing, stamina fine
                } else {
                    stamina += staminaRefillRate;
                }
            }
        }
        // Todo: make ui showing stamina
        Debug.Log(stamina);

        //if (justStopped) {
        //    timer = StartCoroutine(WaitASec(timeTillStaminaRegen));
        //} else if (isRunning && (stamina <= 0)) {
        //    stamina = 0;
        //    characterController.speed = mainSpeed;
        //    isRunning = false;
        //    justStopped = true;
        //} else if(isRunning){
        //    stamina -= staminaDrainRate;
        //} else {
        //    if (stamina > maxStamina) {
        //        stamina = maxStamina;
        //    } else if (stamina == maxStamina) {
        //        // do nothing, stamina fine
        //    } else {
        //        stamina += staminaRefillRate;
        //    }
        //}
    }
    private IEnumerator WaitASec(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        justStopped = false;
        timer = null;
    }
    private void RestartStamTimer() {
        StopCoroutine(timer);
        timer = null;
        timer = StartCoroutine(WaitASec(timeTillStaminaRegen));
    }
}
