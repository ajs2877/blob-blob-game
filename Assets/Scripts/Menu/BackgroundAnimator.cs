using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimator : MonoBehaviour
{
    public GameObject blueBlobPrefab;
    public GameObject redBlobPrefab;
    public int numberOfBlobs = 1;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < numberOfBlobs; i++)
        {
            SpawnBackgroundBlob(i % 2 == 0 ? blueBlobPrefab : redBlobPrefab);
        }
    }

    private void SpawnBackgroundBlob(GameObject blobPrefab)
    {
        GameObject blobSpawned = Instantiate(blobPrefab);
        blobSpawned.AddComponent<BlobBackground>();
    }
}
