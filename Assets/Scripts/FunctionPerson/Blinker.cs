using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer eyeSprite;
    [SerializeField] private Sprite openEyes;
    [SerializeField] private Sprite closeEyes;
    [SerializeField] private float closedEyeTime;

    private void Start()
    {
        StartCoroutine(blinking(1f, 4f));
    }

    private IEnumerator blinking(float ranmin, float ranmax)
    {
        while (true)
        {
            eyeSprite.sprite = openEyes;
            float waiting = Random.Range(ranmin, ranmax);
            yield return new WaitForSeconds(waiting);
            eyeSprite.sprite = closeEyes;
            yield return new WaitForSeconds(closedEyeTime);
        }
    }

    private void OnDisable()
    {
        eyeSprite.sprite = openEyes;
    }
}
