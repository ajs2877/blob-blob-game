using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAnimation : MonoBehaviour
{
    private Vector3 startScale;
    public float scaleChangeRate = 1.5f;
    public float scaleRange = 0.1f;
    public float transformChangeRate = 1.5f;
    public float transformRange = 0.0f;
    private float time = 0;

    private void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        // Game is paused and so, pause animation
        time += Time.deltaTime * Time.timeScale;

        float sinVal1 = Mathf.Sin(time * scaleChangeRate);
        float scaleMultiplier = sinVal1 * scaleRange + 1;
        transform.localScale = startScale * scaleMultiplier;

        if(transformRange != 0)
        {
            float sinVal2 = Mathf.Sin(time * transformChangeRate + 45);
            float transformMultiplier = sinVal2 * transformRange * Time.timeScale;
            transform.localPosition += new Vector3(0, transformMultiplier, 0);
        }
    }
}
