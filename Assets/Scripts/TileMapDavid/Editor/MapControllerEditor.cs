using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {

            MapController targetMap = (MapController)target;
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                targetMap.ChangeTile(hit.transform.root.gameObject);
            }
        }
    }
    
    public override void OnInspectorGUI()
    {
        MapController targetMap = (MapController)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Rebuild Map"))
        {
            if (!targetMap.Initialized)
            {
                targetMap.GenerateMap();
            }
            else
            {
                targetMap.Clean();
            }
        }

        if (GUILayout.Button("EncodeResourceInformation"))
        {
            
        }
    }
    
}
