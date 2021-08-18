using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBackground : MonoBehaviour
{
    public float opacityRate = 0.3f;
    public float maxOpacity = 0.7f;
    public float speed = 0.002f;
    private float time = 0;
    private float randomTimeOffset;
    private float waitTimer = 0;
    private bool teleported = false;
    private Vector3 directionToMove;

    // Start is called before the first frame update
    void Start()
    {
        waitTimer = Random.value;
        randomTimeOffset = Random.value * 90;
        MoveToRandomSpot();
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(
            color.r,
            color.g,
            color.b,
            0
        );
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * Time.timeScale;
        float opacityOffset = Mathf.Sin((time * opacityRate) + randomTimeOffset) * maxOpacity;
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(
            color.r,
            color.g,
            color.b,
            opacityOffset
        );

        if (opacityOffset <= 0.01f)
        {
            if (!teleported)
            {
                MoveToRandomSpot();
                teleported = true;
                waitTimer = Random.value * 0.5f;
            }

            if(waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                randomTimeOffset -= Time.deltaTime * Time.timeScale * opacityRate;
            }
        }
        else if(opacityOffset > 0.01f)
        {

            transform.position += directionToMove * speed;
            teleported = false;
        }
    }

    private void MoveToRandomSpot()
    {
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(Random.value * 0.5f + 0.25f, Random.value * 0.5f + 0.25f, 1));
        directionToMove = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, 0).normalized * (Random.value * 0.5f + 0.5f);
    }
}
