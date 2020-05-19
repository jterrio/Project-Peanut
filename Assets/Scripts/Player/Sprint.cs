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
    [Tooltip("Regular is regular, Always is a toggle where if you start moving you run if you can, Toggle is if you are moving and press sprint you run till you stop moving and have to press it again.")]
    public SprintType sprintType;
    private bool toggleRun = false;

    //COROUTINES FOR SPRINTING
    private Coroutine sprintRegenCoroutine;
    private Coroutine sprintDrainCoroutine;

    public CharacterController characterController;

    public enum SprintType {
        REGULAR,
        ALWAYS,
        TOGGLE
    }

    void Start() {
        maxStamina = stamina;
    }

    private void Update() {
        Sprinting(characterController.speed, characterController.runSpeedIncrease, characterController.mainSpeed);
        UpdateStamina(characterController.mainSpeed);
    }

    /// <summary>
    /// To handle all sprinting types for the character based on what they type they have selected in a neat one line function
    /// </summary>
    /// <param name="speed">base speed of the character</param>
    /// <param name="runSpeedIncrease">have much more speed the user wants to increase the base walk speed by</param>
    /// <param name="mainSpeed">The base speed of the Character</param>
    public void Sprinting(float speed, float runSpeedIncrease, float mainSpeed) {

        switch (sprintType) {
            case SprintType.TOGGLE:

                if (Input.GetKeyDown(KeyCode.LeftShift) && characterController.IsMoving()) {
                    if (!(characterController.moveState == CharacterController.MovementState.RUN)) {
                        characterController.speed += runSpeedIncrease;
                        characterController.moveState = CharacterController.MovementState.RUN;
                    } else if (characterController.moveState == CharacterController.MovementState.RUN) {
                        characterController.speed = mainSpeed;
                        characterController.moveState = CharacterController.MovementState.WALK;
                    }
                } else if (!characterController.IsMoving() && (characterController.moveState == CharacterController.MovementState.RUN)) {
                    characterController.speed = mainSpeed;
                    characterController.moveState = CharacterController.MovementState.WALK;
                }
                break;
            case SprintType.REGULAR:
                if ((Input.GetKey(KeyCode.LeftShift) && characterController.moveState != CharacterController.MovementState.RUN)) {
                    if (characterController.IsMoving() && stamina > staminaRequireToRun) {
                        characterController.speed += runSpeedIncrease;
                        characterController.moveState = CharacterController.MovementState.RUN;
                    }
                } else if (characterController.moveState == CharacterController.MovementState.RUN && !characterController.IsMoving()) {
                    characterController.speed = mainSpeed;
                    characterController.moveState = CharacterController.MovementState.WALK;
                }
                if (Input.GetKeyUp(KeyCode.LeftShift)) {
                    characterController.speed = mainSpeed;
                    characterController.moveState = CharacterController.MovementState.WALK;
                }
                break;
            case SprintType.ALWAYS:
                if (Input.GetKeyDown(KeyCode.LeftShift)) {
                    toggleRun = !toggleRun;
                }

                // Change Speed
                if (characterController.IsMoving() && toggleRun && characterController.moveState != CharacterController.MovementState.RUN && stamina > staminaRequireToRun) {
                    characterController.speed += runSpeedIncrease;
                    characterController.moveState = CharacterController.MovementState.RUN;
                } else if ((!characterController.IsMoving() || !toggleRun) && characterController.moveState != CharacterController.MovementState.WALK) {
                    characterController.speed = mainSpeed;
                    characterController.moveState = CharacterController.MovementState.WALK;
                }
                break;
        }
    }

    /// <summary>
    /// The goal is for it to update the stamina every tick and equalize and bad values.
    /// </summary>
    public void UpdateStamina(float mainSpeed) {

        //REGEN
        if (characterController.moveState != CharacterController.MovementState.RUN && (sprintRegenCoroutine == null) && stamina < maxStamina) {
            StopDrainCoroutine();
            sprintRegenCoroutine = StartCoroutine(RegenStamina(mainSpeed));
        }

        //DRAIN
        if(characterController.moveState == CharacterController.MovementState.RUN && stamina > 0 && sprintDrainCoroutine == null) {
            StopRegenCoroutine();
            sprintDrainCoroutine = StartCoroutine(DrainStamina(mainSpeed));
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


    IEnumerator DrainStamina(float mainSpeed) {
        while(stamina > 0 && characterController.moveState == CharacterController.MovementState.RUN && characterController.IsMoving()) {
            yield return new WaitForSeconds(0.1f);
            stamina -= staminaDrainRate;
        }
        if(stamina < 0) {
            stamina = 0;
            characterController.speed = mainSpeed;
            characterController.moveState = CharacterController.MovementState.WALK;
        }
        sprintDrainCoroutine = null;
    }

    IEnumerator RegenStamina(float mainSpeed) {
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
