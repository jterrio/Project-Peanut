using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Image sprintBar;

    //COROUTINES FOR SPRINTING
    private Coroutine sprintRegenCoroutine;
    private Coroutine sprintDrainCoroutine;

    public PlayerCharacter playerController;

    public enum SprintType {
        REGULAR,
        ALWAYS,
        TOGGLE
    }

    void Start() {
        maxStamina = stamina;
    }

    private void Update() {
        Sprinting(playerController.speed, playerController.runSpeedIncrease, playerController.mainSpeed);
        UpdateStamina(playerController.mainSpeed);
        sprintBar.fillAmount = stamina / maxStamina;
        //ToDo: finish, May not be the right checks and may need to have the parent object checked instead of sprintBar.
        if(stamina == maxStamina && sprintBar.isActiveAndEnabled) {
            //ToDo: Disable sprintBar since it is full and not needed
        }else if (stamina != maxStamina && !sprintBar.isActiveAndEnabled) {
            //ToDo: Enable sprintBar since it is being used and the user needs to see it.
        }
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
                if (Input.GetKeyDown(KeyCode.LeftShift) && playerController.IsMoving()) {
                    if (!(playerController.moveState == PlayerCharacter.MovementState.RUN) && stamina > staminaRequireToRun) {
                        playerController.speed += runSpeedIncrease;
                        playerController.moveState = PlayerCharacter.MovementState.RUN;
                    } else if (playerController.moveState == PlayerCharacter.MovementState.RUN) {
                        playerController.speed = mainSpeed;
                        playerController.moveState = PlayerCharacter.MovementState.WALK;
                    }
                } else if (!playerController.IsMoving() && (playerController.moveState == PlayerCharacter.MovementState.RUN)) {
                    playerController.speed = mainSpeed;
                    playerController.moveState = PlayerCharacter.MovementState.WALK;
                }
                break;
            case SprintType.REGULAR:
                if ((Input.GetKey(KeyCode.LeftShift) && playerController.moveState != PlayerCharacter.MovementState.RUN)) {
                    if (playerController.IsMoving() && stamina > staminaRequireToRun) {
                        playerController.speed += runSpeedIncrease;
                        playerController.moveState = PlayerCharacter.MovementState.RUN;
                    }
                } else if (playerController.moveState == PlayerCharacter.MovementState.RUN && !playerController.IsMoving()) {
                    playerController.speed = mainSpeed;
                    playerController.moveState = PlayerCharacter.MovementState.WALK;
                }
                if (Input.GetKeyUp(KeyCode.LeftShift)) {
                    playerController.speed = mainSpeed;
                    playerController.moveState = PlayerCharacter.MovementState.WALK;
                }
                break;
            case SprintType.ALWAYS:
                if (Input.GetKeyDown(KeyCode.LeftShift)) {
                    toggleRun = !toggleRun;
                }

                // Change Speed
                if (playerController.IsMoving() && toggleRun && playerController.moveState != PlayerCharacter.MovementState.RUN && stamina > staminaRequireToRun) {
                    playerController.speed += runSpeedIncrease;
                    playerController.moveState = PlayerCharacter.MovementState.RUN;
                } else if ((!playerController.IsMoving() || !toggleRun) && playerController.moveState != PlayerCharacter.MovementState.WALK) {
                    playerController.speed = mainSpeed;
                    playerController.moveState = PlayerCharacter.MovementState.WALK;
                }
                break;
        }
    }

    /// <summary>
    /// The goal is for it to update the stamina every tick and equalize and bad values.
    /// </summary>
    public void UpdateStamina(float mainSpeed) {

        //REGEN
        if (playerController.moveState != PlayerCharacter.MovementState.RUN && (sprintRegenCoroutine == null) && stamina < maxStamina) {
            StopDrainCoroutine();
            sprintRegenCoroutine = StartCoroutine(RegenStamina(mainSpeed));
        }

        //DRAIN
        if(playerController.moveState == PlayerCharacter.MovementState.RUN && stamina > 0 && sprintDrainCoroutine == null) {
            StopRegenCoroutine();
            sprintDrainCoroutine = StartCoroutine(DrainStamina(mainSpeed));
        }
        // Todo: make ui showing stamina
        //Debug.Log(stamina);
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
        while(stamina > 0 && playerController.moveState == PlayerCharacter.MovementState.RUN && playerController.IsMoving()) {
            yield return new WaitForSeconds(0.1f);
            stamina -= staminaDrainRate;
        }
        if(stamina < 0) {
            stamina = 0;
            playerController.speed = mainSpeed;
            playerController.moveState = PlayerCharacter.MovementState.WALK;
        }
        sprintBar.fillAmount = stamina / maxStamina;
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
