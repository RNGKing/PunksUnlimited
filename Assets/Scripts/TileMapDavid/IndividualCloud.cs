using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualCloud : MonoBehaviour
{
    public float speed = 1;
    public float resources = 5;
    public MapController mapController;

    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(speed, 0, 0);

        if (transform.position.x > mapController.MapWidth)
        {
            Destroy(gameObject);
        }
        //mapController.tiles[(int)transform.position.x, (int)transform.position.z].(Resources)myTile.; < ------ Replace with function which adds resources to tiles below cloud.
    }


}
