using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightAnimation : MonoBehaviour
{
    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float scaleChangeRate = 1.5f;
        float scaleRange = 0.1f;
        float scaleMultiplier = Mathf.Sin(Time.time * scaleChangeRate) * scaleRange + 1;
        transform.localScale = startScale * scaleMultiplier;
    }
}
