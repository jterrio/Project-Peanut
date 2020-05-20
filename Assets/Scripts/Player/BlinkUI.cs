using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkUI : MonoBehaviour
{
    public GameObject blinking;
    public float blinkTime = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Blinking(blinking.GetComponent<CanvasGroup>()));
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
        float oneThirdBlink = blinkTime / 3;

        //Closing Eyes
        while (counter < oneThirdBlink) {
            counter += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(0, 1, counter / oneThirdBlink);

        }
        //Time with eyes closed
        //Todo: Turn off raycast for Player so Peanut can move
        counter = 0f;
        while (counter < oneThirdBlink) {
            counter += Time.deltaTime;
        }
        //Todo: Turn on raycast for Player so Peanut can not move
        //Opening Eyes
        counter = 0f;
        while (counter < oneThirdBlink) {
            counter += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(1, 0, counter / oneThirdBlink);
            yield return null; //Because we don't need a return value.
        }
    }
}
