using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudControl : MonoBehaviour
{
    public GameObject chuthuluprefab;
    public GameObject cloudPrefab;

    public MapController mapController;

    public float mintime = 0.1f;
    public float maxtime = 1.0f;

    float chance = 0.005f;
    bool special = false;

    private void Awake()
    {
        if (mapController == null) { this.enabled = false; }

        Populate();

        StartCoroutine(Spawn());
    }

    void Populate()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject cloud = Instantiate(cloudPrefab, new Vector3(Random.Range(0.0f, (float)mapController.MapWidth), 0, Random.Range(0.0f, (float)mapController.MapHeight)), Quaternion.identity);
            cloud.GetComponent<IndividualCloud>().mapController = mapController;
        }
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(mintime, maxtime));

            GameObject cloud = Instantiate(cloudPrefab, new Vector3(0, 0, Random.Range(0.0f, (float)mapController.MapHeight - 1)), Quaternion.identity);
            cloud.GetComponent<IndividualCloud>().mapController = mapController;


        }
    }
}
