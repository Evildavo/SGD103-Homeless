using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Pedestrian : Character
{
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;

	Rigidbody m_Rigidbody;
	Animator m_Animator;
	bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;

    Waypoint turnTarget;
    Waypoint lastWaypoint;
    bool isEntering;
    bool hasPlayerAskedForMoneyToday;
    bool hasGivenMoney;
    int dayLastAskedMoney;
    bool isTalkingToPlayer;
    float timeStartedTalkingToPlayer;
    bool hasBeenRepulsed;
    float timeAtLastRepulsion;

    [Space(20.0f)]
    public Trigger Trigger;

    [Header("Pedestrian Settings:")]
    public bool IsVisible = true;
    public bool ReverseDirection;
    public float TurnSpeed;
    public float TurnToFacePlayerSpeed;
    public float WalkSpeed;
    public string WayPointGroupName;
    public float StopAnimatingAboveTimeScale = 800.0f;
    [Header("Note: Also supports wrapping over (e.g. 11pm to 2am)")]
    [Range(0.0f, 24.0f)]
    public float ActiveFromHour = 0.0f;
    [Range(0.0f, 24.0f)]
    public float ActiveToHour = 24.0f;
    public float MaxChanceMoneyGainedWhenBegging;
    public float HealthAffectsChanceFactor = 1.0f;
    public float MoraleAffectsChanceFactor = 1.0f;
    public float CleanlinessAffectsChanceFactor = 1.0f;
    public float[] PossibleMoniesGained;
    public float MoraleLostForActiveBegging;
    public float MoraleLostForAskingTimeOrDay;
    public float WalksAfterSeconds;

    [Header("Factors that control how repellent the player character is")]
    public float PoorHealthRepellenceFactor = 1.0f;
    public float LowMoraleRepellenceFactor = 1.0f;
    public float UncleanlinessRepellenceFactor = 1.0f;
    public float InebriationRepellenceFactor = 1.0f;
    public float IgnorePlayerAtRepellance;
    public float WalkAwayFromPlayerAtRepellance;
    public float TurnAroundCooloffSeconds;
    public float ChanceWalkAwayFromRepulsion;

    [ReadOnly]
    public bool IsInActiveHour;
    [ReadOnly]
    public float PlayerRepellence;


    void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;

        transform.Rotate(new Vector3(0.0f, -90.0f, 0.0f));


        // Set trigger active hours to match our hours.
        Trigger.ActiveFromHour = ActiveFromHour;
        Trigger.ActiveToHour = ActiveToHour;

        // Register listeners.
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        Trigger.RegisterOnCloseRequested(Reset);
    }


    public void OnTrigger()
    {
        // Stop talking to the player if already talking to them.
        if (isTalkingToPlayer)
        {
            Reset();
            return;
        }

        // Player refuses to talk if their morale is too low.
        if (Main.PlayerState.RefusesToTalkToStrangersWhenDepressed &&
            Main.PlayerState.Morale < Main.PlayerState.PoorMoraleEffectsBelowLevel)
        {
            Main.MessageBox.ShowForTime("You don't feel like talking to anyone right now");
        }
        else
        {
            // Having low health, low morale and/or being intoxicated repels the pedestrian.
            if (PlayerRepellence > IgnorePlayerAtRepellance)
            {
                // Ignore player.
                Main.PlayerCharacter.Speak("Excuse me");
                Reset();
            }
            else
            {
                isTalkingToPlayer = true;

                // Player introduces themselves.
                Main.PlayerCharacter.Speak("Excuse me", null, () =>
                {
                    Speak("Yes?", null, () =>
                    {
                        timeStartedTalkingToPlayer = Time.time;

                        // Open conversation menu.
                        if (Main.UI.CurrentTrigger == Trigger)
                        {
                            List<Menu.Option> options = new List<Menu.Option>();
                            options.Add(new Menu.Option(AskForTime, "What's the time?"));
                            options.Add(new Menu.Option(AskForDate, "What day is it today?"));
                            options.Add(new Menu.Option(AskForMoney, "Could you spare some change?"));
                            //options.Add(new Menu.Option(null, "GIVE ME YOUR MONEY NOW!", 0, false));
                            options.Add(new Menu.Option(Reset, "Exit", 0, true, null, true));
                            Main.Menu.Show(options);
                        }
                    });
                });
            }
        }
    }

    public void AskForTime()
    {
        // Apply morale penalty for asking.
        Main.PlayerState.ChangeMorale(-MoraleLostForAskingTimeOrDay);
        
        Main.Menu.Hide();
        Speak("It's " + Main.GameTime.GetTimeAsHumanString() + ".", null, () =>
        {
            Reset();
        });
    }

    public void AskForDate()
    {
        // Apply morale penalty for asking.
        Main.PlayerState.ChangeMorale(-MoraleLostForAskingTimeOrDay);

        Main.Menu.Hide();
        Speak("It's " + Main.GameTime.DayOfTheWeekAsString() + ".", null, () =>
        {
            Reset();
        });
    }

    public void AskForMoney()
    {
        Main.Menu.Hide();

        // Apply morale penalty for asking.
        Main.PlayerState.ChangeMorale(-MoraleLostForActiveBegging);

        // Determine chance of getting money based on health/morale/cleanliness.
        float chance = MaxChanceMoneyGainedWhenBegging * (
            Main.PlayerState.HealthTiredness * HealthAffectsChanceFactor +
            Main.PlayerState.Morale * MoraleAffectsChanceFactor +
            Main.PlayerState.CurrentClothingCleanliness * CleanlinessAffectsChanceFactor) / 3f;
        
        // Check if we got any money.
        bool hasGivenMoneyToday = (hasGivenMoney && hasPlayerAskedForMoneyToday);
        if (!hasPlayerAskedForMoneyToday && !hasGivenMoneyToday && 
            Random.Range(0.0f, 1.0f) < chance)
        {
            float moneyEarned = PossibleMoniesGained[Random.Range(0, PossibleMoniesGained.Length)];
            hasGivenMoney = true;

            // Add money.
            Main.PlayerState.Money += moneyEarned;

            // Tell the user that they got money and how much.
            Speak("Here", null, () =>
            {
                Reset();
            });
            Main.MessageBox.ShowForTime("$" + moneyEarned.ToString("f2") + " gained");
        }
        else if (hasGivenMoneyToday)
        {
            Speak("I already gave you money", null, () =>
            {
                Reset();
            });
        }
        else if (hasPlayerAskedForMoneyToday)
        {
            // Just ignore the player and walk away.
            Reset();
        }
        else
        {
            hasGivenMoney = false;
            Speak("Sorry, no", null, () =>
            {
                Reset();
            });
        }
        hasPlayerAskedForMoneyToday = true;
        dayLastAskedMoney = Main.GameTime.Day;
    }

    public void OnPlayerExit()
    {
        Reset();
    }

    public void Reset()
    {
        isTalkingToPlayer = false;
        Main.Menu.Hide();        
        Trigger.ResetWithCooloff();
    }


    new void Update()
    {
        base.Update();

        // Disable the trigger while the player is doing something.
        if (Main.PlayerState.CurrentTrigger && Main.Splash.IsDisplayed())
        {
            Trigger.Reset(false);
            ForceStopSpeaking();
        }
        else
        {
            Trigger.Reset();
        }

        // Update player repellence, based on health, morale and inebriation.
        {
            PlayerRepellence =
                ((1.0f - Main.PlayerState.HealthTiredness) * PoorHealthRepellenceFactor +
                 (1.0f - Main.PlayerState.Morale) * LowMoraleRepellenceFactor +
                 (1.0f - Main.PlayerState.CurrentClothingCleanliness) * UncleanlinessRepellenceFactor +
                 Main.PlayerState.Inebriation * InebriationRepellenceFactor) / 3f;
        }

        if (isTalkingToPlayer)
        {
            // If the day changes the player can ask for money again.
            if (Main.GameTime.Day != dayLastAskedMoney)
            {
                hasPlayerAskedForMoneyToday = false;
                hasGivenMoney = false;
            }

            // Walk away if the player doesn't say anything for a while.
            if (Main.Menu.IsDisplayed() &&
                Time.time - timeStartedTalkingToPlayer > WalksAfterSeconds)
            {
                Reset();
            }

            // Walk away if the player has triggered something else.
            if (Main.UI.CurrentTrigger != Trigger)
            {
                Reset();
            }
        }

        // Determine if we're in the active hour. If from and to are flipped the period wraps (e.g. 11pm to 2am).
        if (Main.GameTime)
        {
            float time = Main.GameTime.TimeOfDayHours;
            if (ActiveFromHour < ActiveToHour)
            {
                IsInActiveHour = (time >= ActiveFromHour && time <= ActiveToHour);
            }
            else
            {
                IsInActiveHour = (time >= ActiveFromHour || time <= ActiveToHour);
            }
        }
        
        // ReActivate if we're in the active hour.
        if (!IsVisible && IsInActiveHour)
        {
            IsVisible = true;
            GetComponentInChildren<Renderer>().enabled = true;
        }
    }

    void FixedUpdate()
    {
        if (!isTalkingToPlayer)
        {
            // Make walk animation follow gametime. If game-time is too fast for reliable navigation don't animate.
            if (IsVisible && Main.GameTime.TimeScale < StopAnimatingAboveTimeScale)
            {
                // Update turning
                if (turnTarget)
                {
                    Vector3 delta = turnTarget.transform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(delta);
                    Quaternion rotation =
                        Quaternion.RotateTowards(transform.rotation, lookRotation, TurnSpeed * Time.deltaTime);
                    Vector3 eulerAngles = transform.eulerAngles;
                    eulerAngles.y = rotation.eulerAngles.y;
                    transform.eulerAngles = eulerAngles;
                }

                // Walk forward.
                m_AnimSpeedMultiplier = WalkSpeed * Time.deltaTime;
                m_MoveSpeedMultiplier = 1.0f;
                Move(transform.rotation * Vector3.forward * WalkSpeed * Time.deltaTime, false, false);
            }
            else
            {
                m_AnimSpeedMultiplier = 0.0f;
                m_MoveSpeedMultiplier = 0.0f;
                m_TurnAmount = 0.0f;

                // Instantly set player state.
                if (!IsInActiveHour)
                {
                    IsVisible = false;
                    GetComponentInChildren<Renderer>().enabled = false;
                }
            }
        }
        else
        {
            // Stop moving when talking to the player.
            //m_AnimSpeedMultiplier = 0.0f;
            m_MoveSpeedMultiplier = 0.0f;
            m_TurnAmount = 0.0f;
            Move(Vector3.zero, false, false);

            // Turn to face the player.
            if (turnTarget)
            {
                turnToFace(TurnToFacePlayerSpeed * Time.deltaTime);
            }
        }
    }

    void turnToFace(float maxAngle)
    {
        Vector3 delta = Main.PlayerCharacter.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(delta);
        Quaternion rotation =
            Quaternion.RotateTowards(transform.rotation, lookRotation, maxAngle);
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.y = rotation.eulerAngles.y;
        transform.eulerAngles = eulerAngles;
    }



    public void OnTriggerEnter(Collider other)
    {
        // Actively avoid the player if they're highly repellent.
        PlayerRepellenceZone repellenceZone = other.GetComponent<PlayerRepellenceZone>();
        if (repellenceZone && 
            (!hasBeenRepulsed || Time.time - timeAtLastRepulsion > TurnAroundCooloffSeconds) && 
            PlayerRepellence > WalkAwayFromPlayerAtRepellance)
        {
            hasBeenRepulsed = true;
            timeAtLastRepulsion = Time.time;

            // Change direction if they're heading towards the player.
            const float TOLERANCE = 0.1f;
            float dot = Vector3.Dot(
                transform.forward, 
                (transform.position - Main.PlayerCharacter.transform.position));
            bool facingEachOther = (dot < TOLERANCE);
            if (facingEachOther &&
                Random.Range(0.0f, 1.0f) < ChanceWalkAwayFromRepulsion)
            {
                if (!ReverseDirection)
                {
                    turnTarget = lastWaypoint.Previous;
                    ReverseDirection = true;
                }
                else
                {
                    turnTarget = lastWaypoint.Next;
                    ReverseDirection = false;
                }
            }
        }

        // Handle waypoint.
        Waypoint waypoint = other.GetComponent<Waypoint>();
        if (waypoint && waypoint.GroupName == WayPointGroupName)
        {
            lastWaypoint = waypoint;

            // Return to the loop.
            if (waypoint.Exit)
            {
                isEntering = false;
            }

            // Exit at point if inactive.
            if (waypoint.IsExitPoint && !IsInActiveHour && IsVisible)
            {
                IsVisible = false;
                isEntering = true;

                // Switch direction if not teleporting.
                if (!waypoint.TeleportToNext && !waypoint.TeleportToPrevious)
                {
                    transform.forward = -transform.forward;
                    turnTarget = waypoint.Previous;
                }
                GetComponentInChildren<Renderer>().enabled = false;
            }

            // Teleport to the next waypoint.
            if (waypoint.TeleportToNext || waypoint.TeleportToPrevious)
            {
                Vector3 position = transform.position;
                if (!ReverseDirection && waypoint.TeleportToNext)
                {
                    position.x = waypoint.Next.transform.position.x;
                    position.z = waypoint.Next.transform.position.z;
                    turnTarget = waypoint.Next.Next;
                    turnToFace(360.0f);
                }
                else if (ReverseDirection && waypoint.TeleportToPrevious)
                {
                    position.x = waypoint.Previous.transform.position.x;
                    position.z = waypoint.Previous.transform.position.z;
                    turnTarget = waypoint.Previous.Previous;
                    turnToFace(360.0f);
                }
                transform.position = position;
            }
            // Turn towards exit route if inactive.
            else if (!IsInActiveHour && waypoint.Exit)
            {
                turnTarget = waypoint.Exit;
            }
            // Turn towards next waypoint.
            else if (waypoint.Next && !ReverseDirection)
            {
                turnTarget = waypoint.Next;
            }
            // Turn towards previous waypoint.
            else if (waypoint.Previous && ReverseDirection)
            {
                turnTarget = waypoint.Previous;
            }
        }
    }



    public void Move(Vector3 move, bool crouch, bool jump)
	{
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;
        
        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            HandleGroundedMovement(crouch, jump);
        }
        else
        {
            HandleAirborneMovement();
        }

        //ScaleCapsuleForCrouching(crouch);
        //PreventStandingInLowHeadroom();
                
        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }


	void ScaleCapsuleForCrouching(bool crouch)
	{
        if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Crouching = true;
		}
		else
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
				return;
			}
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Crouching = false;
		}
	}

	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
		}
	}


	void UpdateAnimator(Vector3 move)
	{
		// update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        m_Animator.SetBool("Crouch", m_Crouching);
		m_Animator.SetBool("OnGround", m_IsGrounded);
		if (!m_IsGrounded)
		{
			m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
		}

		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle =
			Mathf.Repeat(
				m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
		if (m_IsGrounded)
		{
			m_Animator.SetFloat("JumpLeg", jumpLeg);
		}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (m_IsGrounded && move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
	}


	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);

		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
		{
			// jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
			m_IsGrounded = false;
			m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}


	public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (m_IsGrounded && Time.deltaTime > 0)
		{
			Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = v;
		}
	}


	void CheckGroundStatus()
	{
		//RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		/*Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));*/
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		/*if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))*/
		{
			m_GroundNormal = Vector3.up /*hitInfo.normal*/;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
		}
		/*else
		{
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			m_Animator.applyRootMotion = false;
		}*/
	}
}
