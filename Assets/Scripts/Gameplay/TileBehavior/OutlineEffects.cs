using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffects : MonoBehaviour
{
    private float prevYOffset = 0;
    private float startOpacity;

    private void Start()
    {
        startOpacity = GetComponent<SpriteRenderer>().color.a;
    }

    void Update()
    {
        float offsetRate = 1.15f;
        float yOffset = Mathf.Sin(Time.time * offsetRate) * 0.085f;
        Vector3 position = transform.position;
        transform.Translate(new Vector3(0, yOffset - prevYOffset, 0));
        prevYOffset = yOffset;

        float opacityRate = 1.3f;
        float opacityOffset = Mathf.Sin(Time.time * opacityRate) * 0.2f;
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(
            color.r,
            color.g,
            color.b,
            startOpacity + opacityOffset
        );
    }
}
