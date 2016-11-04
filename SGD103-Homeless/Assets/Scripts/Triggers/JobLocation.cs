using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class JobLocation : MonoBehaviour
{
    private int dayLastChecked;
    private bool hasChecked = false;
    private int daysChecked = 0;
    private GameTime.DayOfTheWeekEnum jobStartsAfter;
    private bool workWeekStarted = false;
    private float timeAtShiftStart = 0.0f;
    private float payDue = 0.0f;
    private float hoursWorkedThisWeek = 0.0f;
    private bool playerStartedLate = false;
    private List<NoticeReason> playerOnNoticeForReasons = new List<NoticeReason>();
    private int numUpdateTicksDuringShift;
    private float healthDuringShiftSum;
    private float moraleDuringShiftSum;

    // When the player is on notice for some issue.
    enum NoticeReason
    {
        NONE,
        LATE_FOR_WORK,
        POOR_HEALTH,
        POOR_MORALE,
        UNCLEAN_CLOTHES,
        UNDER_INFLUENCE_OF_ALCOHOL // Special. No visible warning is given.
    }

    // Represents a job position.
    [System.Serializable]
    public class JobPositionProfile
    {
        public string Role;
        public float PayPerHour;
        public float MinHealthNeededToQualify;
        public float MinMoraleNeededToQualify;
        public float MinClothesCleanlinessToQualify;
        public bool NeedResumeToQualify = true;
        public float ChanceOfSuccessFactor = 1.0f;
        public GameTime.DayOfTheWeekEnum WorkFromDay;
        public GameTime.DayOfTheWeekEnum WorkToDay;
        [Header("Note: Supports wrapping over (e.g. 11pm to 2am)")]
        [Range(0.0f, 24.0f)]
        public float ShiftFromHour;
        [Range(0.0f, 24.0f)]
        public float ShiftToHour;
        public GameTime.DayOfTheWeekEnum PayDay;
        public float PayTime;
        public float TimeAllowedEarly = 0.5f;
        public float TimeAllowedLateBeforeNotice = 0.1f;
        public float TimeAllowedLateBeforeDismissal = 0.5f;
        
        [Header("Receiving a notice and then not correcting ", order = 0)]
        [Space(-10, order = 1)]
        [Header("it the next day results in dismissal", order = 2)]
        public float MinAverageHealthDuringShiftBeforeNotice = 0.3f;
        public float MinAverageMoraleDuringShiftBeforeNotice = 0.3f;
        public float MinClothingCleanlinessBeforeNotice = 0.3f;
        public float MaxInebriationBeforeNotice = 0.3f;
        [Header("Player is fired immediately for turning up to work drunk")]
        public float MaxInebriationBeforeDismissal = 0.6f;

        [ReadOnly]
        public JobLocation Location;
        [ReadOnly]
        public int DaysWorkPerWeek;
        [ReadOnly]
        public int HoursWorkPerShift;
        [ReadOnly]
        public int HoursWorkPerWeek;
        [ReadOnly]
        public float PayPerWeek;

        public void Calculate()
        {
            // Calculate the number of days worked per week.
            DaysWorkPerWeek = 1;
            GameTime.DayOfTheWeekEnum dayOfTheWeek = WorkFromDay;
            while (dayOfTheWeek != WorkToDay && DaysWorkPerWeek < 7)
            {
                DaysWorkPerWeek++;
                if (dayOfTheWeek == GameTime.DayOfTheWeekEnum.SUNDAY)
                {
                    dayOfTheWeek = GameTime.DayOfTheWeekEnum.MONDAY;
                }
                else
                {
                    dayOfTheWeek++;
                }
            }

            // Calculate the number of hours worked per week.
            if (ShiftFromHour < ShiftToHour)
            {
                HoursWorkPerShift = Mathf.RoundToInt(ShiftToHour - ShiftFromHour);
            }
            else
            {
                HoursWorkPerShift = Mathf.RoundToInt((24.0f - ShiftFromHour) + ShiftToHour);
            }
            HoursWorkPerWeek = HoursWorkPerShift * DaysWorkPerWeek;

            // Calculate pay per week.
            PayPerWeek = PayPerHour * HoursWorkPerWeek;
        }
    }

    public Main Main;
    public ResumeItem ResumePrefab;
    public JobTrigger JobTrigger;

    public string Name;
    public float ChanceJobAvailablePerDay = 0.05f;
    [Header("Make job available after so many tries (-1 to disable).")]
    public int GuaranteedAvailableAfterDaysChecked = -1;
    public float FadeToBlackTime = 2.0f;
    public float FadeInFromBlackTime = 2.0f;
    public float WorkTimeScale = 8000.0f;
    public float MoraleGainedAtShiftEnd;
    public float MoraleRewardForSuccessfulApplication;
    public float MoralePenaltyForRejectedApplication;
    public float MoralePenaltyPerNoticeReceived;
    public float MoralePenaltyForDismissal;
    public float TimeCostToApplyForJob;

    public bool IsJobAvailableToday;
    public JobPositionProfile Job;

    public bool PlayerHasJobHere;
    public bool IsPlayerAtWork;
    public bool PlayerWasDismissedHere;
    public int LastDayWorked;
    [ReadOnly]
    public bool CanWorkNow;

    // Returns a short summary of the work days and times.
    public string GetWorkTimeSummaryShort()
    {
        return GameTime.DayOfTheWeekAsShortString(Job.WorkFromDay) + " to " +
               GameTime.DayOfTheWeekAsShortString(Job.WorkToDay) + ", " +
               GameTime.GetTimeAsString(Job.ShiftFromHour) + " - " +
               GameTime.GetTimeAsString(Job.ShiftToHour);
    }

    // Checks for a job and optionally displays a message if one is available.
    public void CheckForJob(bool showMessage = false)
    {
        // Make sure we don't already have this job.
        // Also, don't re-hire when the player was dismissed.
        if (PlayerHasJobHere || PlayerWasDismissedHere)
        {
            IsJobAvailableToday = false;
        }
        else
        {
            // Check if job is available.
            if (!IsJobAvailableToday)
            {
                // Check if it's a new day (or we've never checked before).
                if (Main.GameTime.Day != dayLastChecked || !hasChecked)
                {
                    // Randomly decide if a job is available today.
                    float value = Random.Range(0.0f, 1.0f);
                    if (value <= ChanceJobAvailablePerDay)
                    {
                        IsJobAvailableToday = true;
                    }
                    hasChecked = true;
                    dayLastChecked = Main.GameTime.Day;
                    ++daysChecked;
                }

                // Make job available as a one-time kindness to the player after so many tries.
                const int DISABLED = -1;
                if ((GuaranteedAvailableAfterDaysChecked != DISABLED &&
                    daysChecked > GuaranteedAvailableAfterDaysChecked) || IsJobAvailableToday)
                {
                    IsJobAvailableToday = true;
                    GuaranteedAvailableAfterDaysChecked = DISABLED;
                }
            }

            // Show message about the job if it's available.
            if (IsJobAvailableToday && Job != null)
            {
                string message = "A job position is available today as: " + Job.Role + "\n" +
                                 "$" + Job.PayPerHour.ToString("f2") + "/hr, " +
                                 Job.HoursWorkPerWeek + " hours per week.";
                Main.MessageBox.Show(message, gameObject);
            }
        }
    }

    void RejectApplication(string reason)
    {
        Main.MessageBox.Show("Application rejected. " + reason, gameObject);
        Main.PlayerState.ChangeMorale(-MoralePenaltyForRejectedApplication);
    }

    // Applies for the job that's available.
    public void ApplyForJob()
    {
        // Apply time cost.
        Main.GameTime.SpendTime(TimeCostToApplyForJob);
        
        // Check the basic criteria for acceptance.
        bool healthOk = false;
        bool moraleOk = false;
        bool clothesOk = false;
        bool resumeOk = false;
        if (Main.PlayerState.HealthTiredness >= Job.MinHealthNeededToQualify)
        {
            healthOk = true;
        }
        else
        {
            RejectApplication("You seem too unwell to handle the job");
        }
        if (Main.PlayerState.Morale >= Job.MinMoraleNeededToQualify)
        {
            moraleOk = true;
        }
        else
        {
            RejectApplication("You need to have a more positive attitude");
        }
        if (Main.PlayerState.CurrentClothingCleanliness >= Job.MinClothesCleanlinessToQualify)
        {
            clothesOk = true;
        }
        else
        {
            RejectApplication("You should take better care of your appearance");
        }
        if (!Job.NeedResumeToQualify || (ResumePrefab && Main.Inventory.HasItem(ResumePrefab)))
        {
            resumeOk = true;
        }
        else
        {
            RejectApplication("You should work on your resume");
        }

        // After the basic criteria there's a chance of success. Having a resume guarantees this step.
        if (healthOk && moraleOk && clothesOk && resumeOk)
        {
            // Determine if we're successful. Health and morale affect the chance.
            bool success = false;
            {
                var PlayerState = Main.PlayerState;
                float baseChanceSuccess = (PlayerState.HealthTiredness + PlayerState.Morale) / 2.0f;
                float value = Random.Range(0.0f, 1.0f);
                success = (value <= baseChanceSuccess * Job.ChanceOfSuccessFactor);
            }

            // Consume resume item.
            if (ResumePrefab && Main.Inventory.HasItem(ResumePrefab))
            {
                ResumeItem resume = Main.Inventory.ItemContainer.GetComponentInChildren<ResumeItem>();
                resume.NumUses -= 1;
                if (resume.NumUses == 0)
                {
                    Main.Inventory.RemoveItem(resume);
                }
                Main.Inventory.ShowPreview();
            }

            // Report final decision.
            if (success)
            {
                string message = "Congratulations! From tomorrow you work " +
                                 GameTime.DayOfTheWeekAsShortString(Job.WorkFromDay) + " to " +
                                 GameTime.DayOfTheWeekAsShortString(Job.WorkToDay) + " from " +
                                 GameTime.GetTimeAsString(Job.ShiftFromHour) + " to " +
                                 GameTime.GetTimeAsString(Job.ShiftToHour) + ". Don't be late!";

                Main.MessageBox.Show(message, gameObject);
                PlayerHasJobHere = true;
                workWeekStarted = false;
                jobStartsAfter = Main.GameTime.DayOfTheWeek;
                Main.PlayerState.ChangeMorale(MoraleRewardForSuccessfulApplication);
            }
            else
            {
                RejectApplication("Unfortunately the job is already taken. Try again another time");
            }
        }

        // In any case the job is no longer available today.
        IsJobAvailableToday = false;
    }
    
    // Fires the player immediately with the given reason as explanation.
    public void Dismiss(string reason)
    {
        PlayerWasDismissedHere = true;
        PlayerHasJobHere = false;
        playerOnNoticeForReasons.Clear();

        // If we're at work, stop working.
        if (IsPlayerAtWork)
        {
            stopWork();
        }
                
        // Show dismissed message and give final pay.
        string message = "You have been dismissed from employment. Reason: " + reason + ".";
        if (payDue > 0.0f)
        {
            message += "Your pay comes to $" + payDue.ToString("f2");
            Main.PlayerState.Money += payDue;
            payDue = 0.0f;
            hoursWorkedThisWeek = 0.0f;
        }
        Main.MessageBox.ShowQueued(message, null, gameObject);

        // Receive morale penalty.
        Main.PlayerState.ChangeMorale(-MoralePenaltyForDismissal);
    }

    void OnFadeOutComplete()
    {
        if (IsPlayerAtWork)
        {
            Main.MessageBox.Show("Working...", gameObject);
            Main.GameTime.AccelerateTime(WorkTimeScale);
        }
    }

    public void OnTriggerJob()
    {
        // Start work.
        if (!IsPlayerAtWork)
        {
            // Dismiss for drunkness.
            if (Main.PlayerState.Inebriation > Job.MaxInebriationBeforeDismissal)
            {
                Dismiss("Under the influence of alcohol");
            }
            else
            {
                IsPlayerAtWork = true;
                Main.PlayerState.IsAtWork = true;
                LastDayWorked = Main.GameTime.Day;
                timeAtShiftStart = Main.GameTime.TimeOfDayHours;
                numUpdateTicksDuringShift = 0;
                healthDuringShiftSum = 0.0f;
                moraleDuringShiftSum = 0.0f;
                Main.UI.EnableModalMode();

                // Fade to black.
                Main.ScreenFader.fadeTime = FadeToBlackTime;
                Main.ScreenFader.fadeIn = false;
                Invoke("OnFadeOutComplete", FadeToBlackTime);

                // Hide UI.
                Main.UI.Hide();
            }
        }
    }

    public void OnPlayerExitJob()
    {
        if (IsPlayerAtWork)
        {
            Main.UI.Show();
            Dismiss("Leaving work early");
        }
        JobTrigger.Reset(false);
    }

    void checkCanWorkNow()
    {
        // Determine if work is about to start/has started.
        CanWorkNow = false;
        if (PlayerHasJobHere && !IsPlayerAtWork)
        {
            var GameTime = Main.GameTime;

            // After the day the player got the job we start the first work week.
            if (!workWeekStarted && Main.GameTime.DayOfTheWeek != jobStartsAfter)
            {
                workWeekStarted = true;
            }

            // Check date and time.
            if (workWeekStarted)
            {
                // Check if today is a work day.
                bool workToday = false;
                GameTime.DayOfTheWeekEnum dotw = Job.WorkFromDay;
                int nDaysChecked = 0;
                while (!workToday && dotw != Job.WorkToDay && nDaysChecked < 7)
                {
                    if (dotw == GameTime.DayOfTheWeek)
                    {
                        workToday = true;
                    }
                    dotw = GameTime.NextDayAfter(dotw);
                    nDaysChecked++;
                }
                if (!workToday && dotw == GameTime.DayOfTheWeek)
                {
                    workToday = true;
                }

                // Check the time.
                if (workToday)
                {
                    // Check if we're within time.
                    GameTime.Delta delta = GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, Job.ShiftFromHour);
                    bool earlyOrOnTime = (delta.forward <= delta.backward);
                    if (earlyOrOnTime)
                    {
                        if (delta.forward <= Job.TimeAllowedEarly)
                        {
                            CanWorkNow = true;
                            playerStartedLate = false;
                        }
                    }

                    // Check if we're late.
                    else
                    {
                        // A little late is forgiven.
                        if (delta.backward <= Job.TimeAllowedLateBeforeNotice)
                        {
                            CanWorkNow = true;
                            playerStartedLate = false;
                        }

                        // A bit more late puts the player on notice.
                        else if (delta.backward <= Job.TimeAllowedLateBeforeDismissal)
                        {
                            CanWorkNow = true;
                            playerStartedLate = true;
                        }

                        // Really late results in dismissal.
                        else if (!IsPlayerAtWork && delta.backward <= Job.TimeAllowedLateBeforeDismissal + GameTime.GameTimeDelta)
                        {
                            Dismiss("Late for work");
                        }
                    }
                }
            }
        }
    }

    void Start()
    {
        Job.Location = this;
        Job.Calculate();
        
        JobTrigger.RegisterOnTriggerListener(OnTriggerJob);
        JobTrigger.RegisterOnPlayerExitListener(OnPlayerExitJob);
    }

    void Update()
    {
#if UNITY_EDITOR
        Job.Calculate();
#endif

        // Pay day (and time).
        var GameTime = Main.GameTime;
        if (GameTime.DayOfTheWeek == Job.PayDay && 
            GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, Job.PayTime).shortest <= GameTime.GameTimeDelta &&
            payDue > 0.0f)
        {
            string message = "Work week complete. You worked a total of " +
                             System.Math.Round(hoursWorkedThisWeek, 2).ToString() + " hours and have now earned $" +
                             payDue.ToString("f2");
            Main.PlayerState.Money += payDue;
            payDue = 0.0f;
            hoursWorkedThisWeek = 0.0f;
            Main.MessageBox.ShowQueued(message, null, gameObject);
        }

        if (IsPlayerAtWork)
        {
            // Update the data for calculating average health over the shift. 
            healthDuringShiftSum += Main.PlayerState.HealthTiredness;
            moraleDuringShiftSum += Main.PlayerState.Morale;
            numUpdateTicksDuringShift++;

            // Stop work at the end of shift.
            if (GameTime.TimeOfDayHoursDelta(GameTime.TimeOfDayHours, Job.ShiftToHour).shortest <= GameTime.GameTimeDelta)
            {
                stopWork();
            }
        }
        else
        {
            checkCanWorkNow();
        }        
    }

    void onFadeInComplete()
    {
        int numNoticesReceived = 0;

        // Calculate hours worked. 
        // No overpay is given for starting early, but no penalty is removed for starting very slightly late.
        float hoursWorked;
        if (playerStartedLate)
        {
            hoursWorked = GameTime.TimeOfDayHoursDelta(timeAtShiftStart, Job.ShiftToHour).forward;
        }
        else
        {
            hoursWorked = Job.HoursWorkPerShift;
        }

        // Calculate earnings.
        float pay = hoursWorked * Job.PayPerHour;
        payDue += pay;
        hoursWorkedThisWeek += hoursWorked;

        // Give a report on the day's work.
        var MessageBox = Main.MessageBox;
        string message = "Work day complete. You worked " +
                            System.Math.Round(hoursWorked, 2).ToString() + " hours and earned $" +
                            pay.ToString("f2") + " (to be payed on " +
                            GameTime.DayOfTheWeekAsShortString(Job.PayDay) + " at " +
                            GameTime.GetTimeAsString(Job.PayTime) + ")";
        MessageBox.ShowForTime(message, null, gameObject);

        // Handle warning notices for lateness to work.
        if (playerStartedLate)
        {
            // Already on notice for this reason, so dismiss.
            if (playerOnNoticeForReasons.Contains(NoticeReason.LATE_FOR_WORK))
            {
                Dismiss("Repeatedly late for work");
                return;
            }

            // Give a notice.
            else
            {
                playerOnNoticeForReasons.Add(NoticeReason.LATE_FOR_WORK);
                MessageBox.ShowQueued("Warning Notice: You started work late today.", null, gameObject, true);
                numNoticesReceived++;
            }
        }
        else
        {
            // No longer on notice for this reason.
            playerOnNoticeForReasons.Remove(NoticeReason.LATE_FOR_WORK);
        }

        // Handle warning notices for poor health.
        float averageHealthDuringShift = healthDuringShiftSum / numUpdateTicksDuringShift;
        if (averageHealthDuringShift < Job.MinAverageHealthDuringShiftBeforeNotice)
        {
            // Already on notice for this reason, so dismiss.
            if (playerOnNoticeForReasons.Contains(NoticeReason.POOR_HEALTH))
            {
                Dismiss("Poor work performance");
                return;
            }

            // Give a notice.
            else
            {
                playerOnNoticeForReasons.Add(NoticeReason.POOR_HEALTH);
                MessageBox.ShowQueued("Warning Notice: You performed poorly today.", null, gameObject, true);
                numNoticesReceived++;
            }
        }
        else
        {
            // No longer on notice for this reason.
            playerOnNoticeForReasons.Remove(NoticeReason.POOR_HEALTH);
        }

        // Handle warning notices for poor morale.
        float averageMoraleDuringShift = moraleDuringShiftSum / numUpdateTicksDuringShift;
        if (averageMoraleDuringShift < Job.MinAverageMoraleDuringShiftBeforeNotice)
        {
            // Already on notice for this reason, so dismiss.
            if (playerOnNoticeForReasons.Contains(NoticeReason.POOR_MORALE))
            {
                Dismiss("Bad attitude");
                return;
            }

            // Give a notice.
            else
            {
                playerOnNoticeForReasons.Add(NoticeReason.POOR_MORALE);
                MessageBox.ShowQueued("Warning Notice: You had a bad attitude today.", null, gameObject, true);
                numNoticesReceived++;
            }
        }
        else
        {
            // No longer on notice for this reason.
            playerOnNoticeForReasons.Remove(NoticeReason.POOR_MORALE);
        }
        
        // Handle warning notices for unclean clothes.
        if (Main.PlayerState.CurrentClothingCleanliness < Job.MinClothingCleanlinessBeforeNotice)
        {
            // Already on notice for this reason, so dismiss.
            if (playerOnNoticeForReasons.Contains(NoticeReason.UNCLEAN_CLOTHES))
            {
                Dismiss("Unhygienic attire");
                return;
            }

            // Give a notice.
            else
            {
                playerOnNoticeForReasons.Add(NoticeReason.UNCLEAN_CLOTHES);
                MessageBox.ShowQueued("Warning Notice: You wore unhygienic clothing today.", null, gameObject, true);
                numNoticesReceived++;
            }
        }
        else
        {
            // No longer on notice for this reason.
            playerOnNoticeForReasons.Remove(NoticeReason.UNCLEAN_CLOTHES);
        }

        // Handle warning notices for being under the influence of alcohol.
        if (Main.PlayerState.Inebriation > Job.MaxInebriationBeforeNotice)
        {
            // Already on notice for this reason, so dismiss.
            if (playerOnNoticeForReasons.Contains(NoticeReason.UNDER_INFLUENCE_OF_ALCOHOL))
            {
                Dismiss("Found under the influence of alcohol");
                return;
            }

            // Record a notice (hidden from the player).
            else
            {
                playerOnNoticeForReasons.Add(NoticeReason.UNDER_INFLUENCE_OF_ALCOHOL);
            }
        }
        else
        {
            // No longer on notice for this reason.
            playerOnNoticeForReasons.Remove(NoticeReason.UNDER_INFLUENCE_OF_ALCOHOL);
        }

        // Give the player a morale boost for completing the shift, but penalise them for notices received.
        Main.PlayerState.ChangeMorale(MoraleGainedAtShiftEnd - numNoticesReceived * MoralePenaltyPerNoticeReceived);
    }

    void stopWork()
    {
        if (IsPlayerAtWork)
        {
            IsPlayerAtWork = false;
            Main.PlayerState.IsAtWork = false;
            Main.GameTime.ResetToNormalTime();
            Main.MessageBox.Hide();
            JobTrigger.Reset(false);
            Main.UI.DisableModalMode();

            // Show UI.
            Main.UI.Show();

            // Fade in from black.
            Main.ScreenFader.fadeTime = FadeInFromBlackTime;
            Main.ScreenFader.fadeIn = true;

            Invoke("onFadeInComplete", FadeInFromBlackTime);
        }
    }

}
