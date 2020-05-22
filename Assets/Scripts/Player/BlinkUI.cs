using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkUI : MonoBehaviour
{
    public GameObject blinking;
    public bool isBlinking = false;
    public float blinkTime = 0.15f;
    public Image blinkBar; 
    private CanvasGroup canvasGroup;
    public float timeToBlink = 3f;
    private float maxTimeToBlink;
    public GameObject Peanut;
    private Peanut peanut;
    private Coroutine blinkCo = null;

    void Start() {
        canvasGroup = blinking.GetComponent<CanvasGroup>();
        peanut = Peanut.GetComponent<Peanut>();
        maxTimeToBlink = timeToBlink;
    }
    // Update is called once per frame
    void Update(){
        BlinkTimer();
    }

    /// <summary>
    /// Blocks vision for Player as well as lets Peanut move while they are Blinking
    /// </summary>
    /// <param name="canvGroup"></param>
    /// <returns></returns>
    public IEnumerator Blinking(CanvasGroup canvGroup){
        float counter = 0f;
        float oneThirdBlink = blinkTime/3;
        isBlinking = true;
        //Closing Eyes
        while (counter < oneThirdBlink) {
            counter += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(0, 1f, counter / oneThirdBlink);
            yield return null;
        }
        //Time with eyes closed
        counter = 0f;
        while (counter < (oneThirdBlink)) {
            counter += Time.deltaTime;
            yield return null;
        }
        //Opening Eyes
        isBlinking = false;
        counter = 0f;
        while (counter < oneThirdBlink) {
            counter += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(1f, 0, counter / oneThirdBlink);
            yield return null;
        }
        blinkCo = null;
        timeToBlink = maxTimeToBlink;
    }

    public void BlinkTimer() {
        //ToDo: Disable/Enable based on if the blinkBar is full or being used
        if (blinkCo == null) {
            blinkBar.fillAmount = (timeToBlink / maxTimeToBlink);
        } else {
            blinkBar.fillAmount = 0;
        }// ToDo: have the blinkBar reset to Max if you look away... Something like that
        if (timeToBlink > 0 && peanut.IsSeen) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                timeToBlink = 0;
                if (blinkCo != null) {
                    StopCoroutine(blinkCo);
                }
                blinkCo = StartCoroutine(Blinking(canvasGroup));
            }
            timeToBlink -= Time.deltaTime;
        } else if (timeToBlink <= 0 && blinkCo == null) {
            blinkCo = StartCoroutine(Blinking(canvasGroup));
        }
    }
}
