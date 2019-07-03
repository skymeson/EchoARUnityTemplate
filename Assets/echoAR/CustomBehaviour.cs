using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Entry entry;

    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    // Use this for initialization
    void Start()
    {
        // Add RemoteTransformations script to object and set its entry
        this.gameObject.AddComponent<RemoteTransformations>().entry = entry;

        // Qurey additional data to get the name
        string value = "";
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            this.gameObject.name = value;
            Debug.Log("Setting gameobject name to " + value + ".");
        }

        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("xAngle", out value))
        {
            // Rotate around y axis
            this.gameObject.transform.Rotate(Vector3.up, float.Parse(value)) ;
            Debug.Log("Rotating around axis by " + value + " degrees.");
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}