using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.


        struct InputState
        {
            public float h;
            public float v;
            public bool crouch;

            public InputState(float h, float v, bool crouch)
            {
                this.h = h;
                this.v = v;
                this.crouch = crouch;
            }
        }

        float controlDelaySeconds = 0.0f;
        Queue<InputState> stateQueue = new Queue<InputState>();
        float stateQueueTimeSum = 0.0f;


        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        
        public void SetControlDelay(float seconds)
        {
            controlDelaySeconds = seconds;
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

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
                        delayedInputUpdate(new InputState(h, v, crouch));
                    }
                    stateQueueTimeSum -= Time.deltaTime;
                    stateQueueTimeSum = Mathf.Max(0.0f, stateQueueTimeSum);
                }

                // Add to the state queue.
                stateQueue.Enqueue(new InputState(h, v, crouch));
                stateQueueTimeSum += Time.deltaTime;
            }
            else
            {
                delayedInputUpdate(new InputState(h, v, crouch));
            }
        }


        void delayedInputUpdate(InputState inputState)
        {
            float h = inputState.h;
            float v = inputState.v;
            bool crouch = inputState.crouch;

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }


    }
}
