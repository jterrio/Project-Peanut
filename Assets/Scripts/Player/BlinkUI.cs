using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkUI : MonoBehaviour
{
    public GameObject blinking;
    public bool isBlinking = false;
    public float blinkTime = 0.15f;
    private CanvasGroup canvasGroup;

    void Start() {
        canvasGroup = blinking.GetComponent<CanvasGroup>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Blinking(canvasGroup));
        }
    }

    /// <summary>
    /// Disables Vision for Player as well as ToDo: blocks raycast so Peanut can move while blinked
    /// </summary>
    /// <param name="canvGroup"></param>
    /// <returns></returns>
    public IEnumerator Blinking(CanvasGroup canvGroup)
    {
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
    }
}
