using UnityEngine;
using System.Collections;

public class Lucy : Character
{
    enum Reputation
    {
        GOOD,
        STANDARD,
        BAD,
        END,
    }

    Reputation reputation = Reputation.STANDARD;

    public float TimeCostPerConversation;
    [Header("Not given when the character says they're busy")]
    public float MoraleRewardPerConversation;

    void Start()
    {
        var trigger = GetComponent<Trigger>();
        trigger.RegisterOnTriggerListener(OnTrigger);
        trigger.RegisterOnPlayerExitListener(OnPlayerExit);
        trigger.RegisterOnCloseRequested(Reset);
    }

    new void Update()
    {
        base.Update();
    }

    public void OnTrigger()
    {
        if (reputation == Reputation.STANDARD)
        {
            Speak("(cheerful) Hey there chin up, we've got a big day of stuff to do! ");
            Main.PlayerCharacter.ShowStandardDialogueMenu(
                "Anything in mind?",
                "That's alright... ",
                "Get any money from that job you applied for yet?",
                onResponseChosen);
        }
        else if (reputation == Reputation.GOOD)
        {
            Speak("Hey your looking well today, I was wondering why aren't you reviving the doll?");
            Main.PlayerCharacter.ShowStandardDialogueMenu(
                " My debts eat that up... ",
                "I'm fine with money okay...",
                "No they wouldn't give any to me, makes it rough.",
                onResponseChosen);
        }
        else if (reputation == Reputation.BAD)
        {
            Speak("Oh, hey things are a bit rough so i'd rather... how have you been?");
            Main.PlayerCharacter.ShowStandardDialogueMenu(
                "Normal I suppose. Are you okay? ",
                "Fine, why wouldn't I be?",
                "Back off, i'm just here to see if you have any food for me.",
                onResponseChosen);
        }
        else
        {
            Speak("Hey sorry i'm busy at the momment.");
            Reset();

        }
    }

    void onResponseChosen(PlayerCharacter.ResponseType response)
    {
        // Spend time having the conversation.
        Main.GameTime.SpendTime(TimeCostPerConversation);

        // Morale reward for conversation.
        Main.PlayerState.ChangeMorale(MoraleRewardPerConversation);

        if (reputation == Reputation.STANDARD)
        {
            if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
            {
                Speak("Well I was thinking of heading to the library later", null);
                AddCaptionChangeCue(3, "its always good to look for jobs and improve the resume.");
                AddCaptionChangeCue(6, "Plus reading always used to be a good way to pass the time.");
                SetFinishCallback(() =>
                {
                    Main.PlayerCharacter.Speak("Okay i'll have a browse if i'm free then.", null, () =>
                    {
                        Speak("Oh ok");
                        reputation = Reputation.GOOD;
                        Reset();
                    });
                });
            }
            else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
            {
                Main.PlayerCharacter.Speak("That's alright I've got my own stuff I need to take care of.", null, () =>
                {
                    Speak("Oh, okay then. Well I'll be about if you know need anything,");
                    AddCaptionChangeCue(5, "or just someone to talk to.");
                    SetFinishCallback(() =>
                    { 
                        Main.PlayerCharacter.Speak("Sure thing.");
                        Reset();
                    });
                });
            }
            else if (response == PlayerCharacter.ResponseType.SELFISH)
            {
                Speak("(Taken Back ) No of course not why would you ask that !?");
                AddCaptionChangeCue(4, "I umm haven't heard anything back yet..");
                AddCaptionChangeCue(6, "But i'm keeping hopes up!");
           
                reputation = Reputation.BAD;
                Reset();
            }
            else
            {
                Reset();
            }
        }
        else if (reputation == Reputation.GOOD)
        {
            if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
            {
                Main.PlayerCharacter.Speak("My debts eat that up... faster than I can come up with a good metaphor.");
                Main.PlayerCharacter.AddCaptionChangeCue(3, "I just barely break even....");
                Main.PlayerCharacter.SetFinishCallback(() =>
                {
                    Speak("Here I thought I had it tough.", null, () =>
                    {
                        Main.PlayerCharacter.Speak("It's not a competition we just have to march on.", null, () =>
                        {
                            Speak("Yeah.");
                            reputation = Reputation.END;
                            Reset();
                        });
                    });
                });
            }
            else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
            {
                Main.PlayerCharacter.Speak("I'm fine with money, well for the circumstances.", null, () =>
                {
                    Speak("Alrighty then well if you need anything don't be afraid to ask.");
                    reputation = Reputation.END;
                    Reset();
                });
            }
            else if (response == PlayerCharacter.ResponseType.SELFISH)
            {
                Speak("It must be, here its not much but take this.");
                reputation = Reputation.END;
                Reset();
            }
            else
            {
                Reset();
            }
        }
        else if (reputation == Reputation.BAD)
        {
            if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
            {
                Speak("Oh i'm not worth the worry.");
                AddCaptionChangeCue(2, "I mean i'm fine.");
                AddCaptionChangeCue(6, "What about yourself?");
                AddCaptionChangeCue(8, "Sorry I already asked that sorry. .");
                SetFinishCallback(() =>
                {
                    Main.PlayerCharacter.Speak("Lucy it's okay, can I help you with anything.", null, () =>
                    {
                        Speak("No i'm sorry to bother you", null, () =>
                        {
                            Main.PlayerCharacter.Speak("Just promise me you'll find me if you need anything.", null, () =>
                            {
                                Speak("Okay but-", null, () =>
                                {
                                    Main.PlayerCharacter.Speak("It's settled then.", null, () =>
                                    {
                                        reputation = Reputation.END;
                                        Reset();
                                    });
                                });
                            });
                        });
                    }, 3.0F);
                });
            }
            else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
            {
                Speak("Because this streets aren't kind, it would seem that nobody here is.", null);
                reputation = Reputation.END;
                Reset();
            }
            else if (response == PlayerCharacter.ResponseType.SELFISH)
            {
                Speak("......", null, () =>
                {
                    Main.PlayerCharacter.Speak("Well go on say something?", null, () =>
                    {
                        Speak("Why did I think you were different.");
                        AddCaptionChangeCue(2, "I mean i'm fine.");
                        SetFinishCallback(() =>

                        {
                            Main.PlayerCharacter.Speak("Easy there.", null, () =>
                            {
                                Speak("Sorry.");
                                AddCaptionChangeCue(3, "No, i'm not putting up with it.");
                                AddCaptionChangeCue(5, "I'm not that person anymore!");
                                AddCaptionChangeCue(7, "I want to help you.");
                                AddCaptionChangeCue(8, " I Really truly do..");
                                AddCaptionChangeCue(11, "But i'm not going to let you walk all over me again!");
                                AddCaptionChangeCue(14, "How long can I explaining this to my friends.");
                                AddCaptionChangeCue(16, " I don't-");
                                SetFinishCallback(() =>
                                {
                                    Main.PlayerCharacter.Speak("Again?", null, () =>
                                    {
                                        Speak("Just ");
                                        AddCaptionChangeCue(1, "....just please leave me alone.");
                                        reputation = Reputation.END;
                                        Reset();
                                    });
                                });
                            });
                        });
                    });
                });

            }
            else
            {
                Reset();
            }
        }
    }

    public void OnPlayerExit()
    {
        if (IsSpeaking)
        {
            Speak("Hey, where are you going?");
        }
    }

    public void Reset()
    {
        Main.Menu.Hide();
        GetComponent<Trigger>().Reset();
    }
}
