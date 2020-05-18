using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint : MonoBehaviour
{
    [Header("Stamina")]
    public float stamina = 100;
    [Tooltip("Per update call")]
    public float staminaDrainRate = .08f;
    [Tooltip("Per update call")]
    public float staminaRefillRate = 1f;
    public float staminaRequireToRun = 20;
    [Tooltip("Seconds")]
    public float timeTillStaminaRegen = 3f;
    private float maxStamina;
    public bool toggleSprint = false;

    //COROUTINES FOR SPRINTING
    private Coroutine sprintRegenCoroutine;
    private Coroutine sprintDrainCoroutine;

    public CharacterController characterController;

    void Start() {
        maxStamina = stamina;
    }

    private void Update() {
        Sprinting(characterController.speed, characterController.runSpeedIncrease, characterController.mainSpeed);
        UpdateStamina(characterController.mainSpeed);
    }

    /// <summary>
    /// To handle all sprinting for the character in a neat one line function
    /// </summary>
    /// <param name="speed">base speed of the character</param>
    /// <param name="runSpeedIncrease">have much more speed the user wants to increase the base walk speed by</param>
    /// <param name="toggleSprint">Determins what type of sprinting the character wants to use</param>
    public void Sprinting(float speed, float runSpeedIncrease, float mainSpeed) {
        if (toggleSprint) {
            if (stamina < staminaRequireToRun && Input.GetKeyDown(KeyCode.LeftShift) && characterController.moveState == CharacterController.MovementState.WALK) {
                // do nothing because you can't start running right now
            } else if (Input.GetKeyDown(KeyCode.LeftShift) && characterController.moveState == CharacterController.MovementState.WALK) {
                characterController.speed += runSpeedIncrease;
                characterController.moveState = CharacterController.MovementState.RUN;
            } else if (Input.GetKeyDown(KeyCode.LeftShift)) {
                // You are currently running. So you need to stop running.
                characterController.speed = mainSpeed;
                characterController.moveState = CharacterController.MovementState.WALK;
            }
        } else {
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                if (stamina > staminaRequireToRun) {
                    characterController.speed += runSpeedIncrease;
                    characterController.moveState = CharacterController.MovementState.RUN;
                }
                if (Input.GetKeyUp(KeyCode.LeftShift)) {
                    characterController.speed = mainSpeed;
                    characterController.moveState = CharacterController.MovementState.WALK;
                }
            }
        }
    }

    /// <summary>
    /// The goal is for it to update the stamina every tick and equalize and bad values.
    /// </summary>
    public void UpdateStamina(float mainSpeed) {

        //REGEN
        if (characterController.moveState != CharacterController.MovementState.RUN && (sprintRegenCoroutine == null) && stamina < maxStamina) {
            StopDrainCoroutine();
            sprintRegenCoroutine = StartCoroutine(RegenStamina());
        }

        //DRAIN
        if(characterController.moveState == CharacterController.MovementState.RUN && stamina > 0 && sprintDrainCoroutine == null) {
            StopRegenCoroutine();
            sprintDrainCoroutine = StartCoroutine(DrainStamina());
        }
        // Todo: make ui showing stamina
        Debug.Log(stamina);
    }

    void StopSprintCoroutines() {
        StopRegenCoroutine();
        StopDrainCoroutine();
    }

    void StopRegenCoroutine() {
        if (sprintRegenCoroutine != null) {
            StopCoroutine(sprintRegenCoroutine);
            sprintRegenCoroutine = null;
        }
    }

    void StopDrainCoroutine() {
        if (sprintDrainCoroutine != null) {
            StopCoroutine(sprintDrainCoroutine);
            sprintDrainCoroutine = null;
        }
    }


    IEnumerator DrainStamina() {
        while(stamina > 0 && characterController.moveState == CharacterController.MovementState.RUN && characterController.IsMoving()) {
            yield return new WaitForSeconds(0.1f);
            stamina -= staminaDrainRate;
        }
        if(stamina < 0) {
            stamina = 0;
        }
        sprintDrainCoroutine = null;
    }

    IEnumerator RegenStamina() {
        yield return new WaitForSeconds(timeTillStaminaRegen);
        while(stamina < maxStamina) {
            yield return new WaitForSeconds(0.1f);
            stamina += staminaRefillRate;
        }
        if(stamina > maxStamina) {
            stamina = maxStamina;
        }
        sprintRegenCoroutine = null;
    }
}
