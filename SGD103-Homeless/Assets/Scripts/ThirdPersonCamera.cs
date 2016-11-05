using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    Vector3 initialPosition;
    Quaternion initialRotation;

    public float MaxTurnAngleDegrees;
    public float MaxCameraSidewaysShift;
    public float MaxCameraForwardsShift;

    void Start () {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Turn horizontally and shift the camera sideways based on how far the mouse is from horizontal centre.
        float halfScreenWidth = (Screen.width / 2.0f);
        float horizontalDelta = (Input.mousePosition.x - halfScreenWidth) / halfScreenWidth;
        transform.localRotation =
            initialRotation * Quaternion.Euler(
                0.0f,
                horizontalDelta * MaxTurnAngleDegrees, 
                0.0f);
        transform.localPosition =
            initialPosition + new Vector3(0.0f, horizontalDelta * MaxCameraSidewaysShift, 0.0f);

        // Shift the camera forward/backward based on how far the mouse is from vertical centre.
        float halfScreenHeight = (Screen.height / 2.0f);
        float verticalDelta = (Input.mousePosition.y - halfScreenWidth) / halfScreenWidth;
        transform.localPosition += new Vector3(verticalDelta * MaxCameraForwardsShift, 0.0f, 0.0f);
    }
}
