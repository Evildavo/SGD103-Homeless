using UnityEngine;
using System.Collections;

public class CarBackground : MonoBehaviour {

    public float Speed = 6.0f;
    public Zone WrapFrom;
    public Zone WrapTo;

    void Start () {
	
	}
	
	void Update () {

        // Move forward.
        transform.Translate(new Vector3(Speed, 0.0f, 0.0f) * Time.deltaTime);
	}
    
    public void OnTriggerEnter(Collider other)
    {
        // Wrap over when running out of road.
        if (other.gameObject == WrapFrom.gameObject)
        {
            Vector3 wrapFromDelta = transform.position - WrapFrom.transform.position;
            Quaternion wrapToRotation = Quaternion.Euler(0.0f, -WrapTo.transform.eulerAngles.y, 0.0f);
            transform.position = WrapTo.transform.position + wrapToRotation * wrapFromDelta;
            transform.rotation = WrapTo.transform.rotation;
        }
    }
}
