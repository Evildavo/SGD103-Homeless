using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Zone : MonoBehaviour
{
    public string ZoneName;
    
    // Call from derived.
    public virtual void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    // Call from derived.
    public virtual void Update() {}

    // Override to do some action on entering.
    public virtual void OnTriggerEnter(Collider other) {}

    // Override to do some action on exiting.
    public virtual void OnTriggerExit(Collider other) {}

}
