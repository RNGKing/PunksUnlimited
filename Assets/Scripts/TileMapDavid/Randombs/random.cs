using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class random : MonoBehaviour
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

        Ray ray = new Ray(transform.position + new Vector3(0,5,0), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 20))
        {
            mapController.curstate = MapController.editstate.oceans;
            mapController.ChangeTile(hit.transform.gameObject);
        }
        //mapController.tiles[(int)transform.position.x, (int)transform.position.z].(Resources)myTile.; < ------ Replace with function which adds resources to tiles below cloud.
    }
}
