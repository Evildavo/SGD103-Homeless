﻿using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    Vector3 initialPosition;
    Quaternion initialRotation;

    public Main Main;

    [Header("Turns the camera on the spot")]
    public float MaxLookAngleDegrees;
    public float MaxCameraSidewaysShift;
    public float MaxCameraForwardsShift;
    public float MaxTurnAngleDegreesPerSecond;
    public float TurnDeadzone;
    [Header("Turns the camera around the player")]
    public float MaxLookSideAngleDegrees;
    //public float DollyAtSide;

    void Start () {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Turn horizontally and shift the camera sideways based on how far the mouse is from horizontal centre.
        float halfScreenWidth = (Screen.width / 2.0f);
        float horizontalOffsetFromCentre = (Input.mousePosition.x - halfScreenWidth) / halfScreenWidth;
        transform.localRotation =
            initialRotation * Quaternion.Euler(
                0.0f,
                horizontalOffsetFromCentre * MaxLookAngleDegrees,
                0.0f);
        Mathf.Clamp(horizontalOffsetFromCentre, 0.0f, MaxCameraSidewaysShift);
        transform.localPosition =
            initialPosition + new Vector3(0.0f, horizontalOffsetFromCentre * MaxCameraSidewaysShift, 0.0f);

        // Turn the camera around the player based on how far the mouse is from the horizontal centre.
        transform.RotateAround(GetComponentInParent<PlayerCharacter>().transform.position, 
            Vector3.up, horizontalOffsetFromCentre * MaxLookSideAngleDegrees);

        // Shift the camera forward/backward based on how far the mouse is from vertical centre.
        float halfScreenHeight = (Screen.height / 2.0f);
        float verticalOffsetFromCentre = (Input.mousePosition.y - halfScreenHeight) / halfScreenHeight;
        Mathf.Clamp(verticalOffsetFromCentre, 0.0f, MaxCameraForwardsShift);
        transform.localPosition += new Vector3(verticalOffsetFromCentre * MaxCameraForwardsShift, 0.0f, 0.0f);
        
        // Make player turn as mouse approaches the screen edge.
        if (!Main.UI.IsInModalMode() && Main.PlayerState.CurrentTrigger == null)
        {
            float distanceFromLeftEdge =
                Mathf.Min(
                    Input.mousePosition.x / (halfScreenWidth * (1.0f - TurnDeadzone)), 1.0f);
            float distanceFromRightEdge =
                Mathf.Min(
                    (Screen.width - Input.mousePosition.x) / (halfScreenWidth * (1.0f - TurnDeadzone)), 1.0f);
            if (distanceFromLeftEdge < distanceFromRightEdge)
            {
                GetComponentInParent<PlayerCharacter>().Turn(
                    (distanceFromLeftEdge - 1.0f) * MaxTurnAngleDegreesPerSecond * Time.deltaTime);
            }
            else
            {
                GetComponentInParent<PlayerCharacter>().Turn(
                    (1.0f - distanceFromRightEdge) * MaxTurnAngleDegreesPerSecond * Time.deltaTime);
            }
        }
    }
}
