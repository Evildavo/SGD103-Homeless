using UnityEngine;
using System.Collections.Generic;

public class ThirdPersonCamera : MonoBehaviour {
    Vector3 initialPosition;
    Quaternion initialRotation;
    float controlDelaySeconds = 0.0f;
    Queue<InputState> stateQueue = new Queue<InputState>();
    float stateQueueTimeSum = 0.0f;

    struct InputState
    {
        public float x;
        public float y;

        public InputState(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public Main Main;

    [Header("Turns the camera on the spot")]
    public float MaxLookAngleDegrees;
    public float MaxCameraSidewaysShift;
    public float MaxCameraForwardsShift;
    public float MaxTurnAngleDegreesPerSecond;
    public float TurnDeadzone;
    [Header("Turns the camera around the player")]
    public float MaxLookSideAngleDegrees;
    public float DollyAtSide;



    public void SetControlDelay(float seconds)
    {
        controlDelaySeconds = seconds;
    }


    void Start () {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // read inputs
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        
        // Add a control delay.
        if (controlDelaySeconds > 0.0f)
        {
            // Delay input response.
            while (stateQueueTimeSum > controlDelaySeconds)
            {
                if (stateQueue.Count > 0)
                {
                    delayedInputUpdate(stateQueue.Dequeue());
                }
                else
                {
                    delayedInputUpdate(new InputState(x, y));
                }
                stateQueueTimeSum -= Time.deltaTime;
                stateQueueTimeSum = Mathf.Max(0.0f, stateQueueTimeSum);
            }

            // Add to the state queue.
            stateQueue.Enqueue(new InputState(x, y));
            stateQueueTimeSum += Time.deltaTime;
        }
        else
        {
            delayedInputUpdate(new InputState(x, y));
        }
    }

    void delayedInputUpdate(InputState inputState)
    {
        // Turn horizontally and shift the camera sideways based on how far the mouse is from horizontal centre (smooth curved).
        float halfScreenWidth = (Screen.width / 2.0f);
        float horizontalOffsetFromCentre =
            Mathf.Asin((inputState.x - halfScreenWidth) / halfScreenWidth * (Mathf.PI / 4.0f));
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

        // Shift camera forward/backward based on how far the mouse is from the horizontal centre.
        transform.localPosition += new Vector3(Mathf.Abs(horizontalOffsetFromCentre * DollyAtSide), 0.0f, 0.0f);

        // Shift the camera forward/backward based on how far the mouse is from vertical centre.
        float halfScreenHeight = (Screen.height / 2.0f);
        float verticalOffsetFromCentre = (inputState.y - halfScreenHeight) / halfScreenHeight;
        Mathf.Clamp(verticalOffsetFromCentre, 0.0f, MaxCameraForwardsShift);
        transform.localPosition += new Vector3(verticalOffsetFromCentre * MaxCameraForwardsShift, 0.0f, 0.0f);

        // Make player turn as mouse approaches the screen edge.
        if (!Main.UI.IsInModalMode() && Main.PlayerState.CurrentTrigger == null)
        {
            float distanceFromLeftEdge =
                Mathf.Min(
                    inputState.x / (halfScreenWidth * (1.0f - TurnDeadzone)), 1.0f);
            float distanceFromRightEdge =
                Mathf.Min(
                    (Screen.width - inputState.x) / (halfScreenWidth * (1.0f - TurnDeadzone)), 1.0f);
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
