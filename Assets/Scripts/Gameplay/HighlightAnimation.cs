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

    private void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float sinVal1 = Mathf.Sin(Time.time * scaleChangeRate);
        float scaleMultiplier = sinVal1 * scaleRange + 1;
        transform.localScale = startScale * scaleMultiplier;

        if(transformRange != 0)
        {
            float sinVal2 = Mathf.Sin(Time.time * transformChangeRate + 45);
            float transformMultiplier = sinVal2 * transformRange;
            transform.localPosition += new Vector3(0, transformMultiplier, 0);
        }
    }
}
