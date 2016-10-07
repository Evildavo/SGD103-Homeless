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
            Transform delta = transform;
            delta.Translate(-WrapTo.transform.position);
            delta.Rotate(new Vector3(0.0f, transform.eulerAngles.y, 0.0f));
            Vector3 position = delta.position;
            position.x = 0.0f;
            delta.position = position;
            delta.Translate(WrapTo.transform.position);
            transform.position = delta;
        }
    }
}
