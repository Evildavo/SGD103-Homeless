using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    Vector3 initialPosition;
    Quaternion initialRotation;

    public float MaxTurnAngleDegrees;
    public float MaxCameraSidewaysShift;

    void Start () {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Turn horizontally and shift the camera sideways based on how far the mouse is from centre.
        float halfScreenWidth = (Screen.width / 2.0f);
        float delta = (Input.mousePosition.x - halfScreenWidth) / halfScreenWidth;
        transform.localRotation =
            initialRotation * Quaternion.Euler(
                0.0f,
                delta * MaxTurnAngleDegrees, 
                0.0f);
        transform.localPosition =
            initialPosition + new Vector3(0.0f, delta * MaxCameraSidewaysShift, 0.0f);
	}
}
