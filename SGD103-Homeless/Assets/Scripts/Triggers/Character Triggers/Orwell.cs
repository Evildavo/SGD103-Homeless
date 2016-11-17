using UnityEngine;
using System.Collections;

public class Orwell : Character
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
            Speak("Gamba daru friend. What brings you to my little corner of the world.");
            Main.PlayerCharacter.ShowStandardDialogueMenu(
                "I was looking for some company. ",
                "I'm just here for the food.",
                "Shove off your taking all the good stuff.",
                onResponseChosen);
        }
        else if (reputation == Reputation.GOOD)
        {
            Speak("Ah welcome back friend. Here to dwell on the nature of the universe with me?");
            AddCaptionChangeCue(3, "To steal a catch of phrase from Tadao Ando.");
            AddCaptionChangeCue(5, "If you give people nothingness, they can ponder what can be achieved from that nothingness. ");
            SetFinishCallback(() =>
            {
                Main.PlayerCharacter.ShowStandardDialogueMenu(
                    "Let's act on such impulse not speak it. ",
                    "I'll never have nothing...",
                    "Sometimes nothingness, leave us without...",
                    onResponseChosen);
            });
        }
        else if (reputation == Reputation.BAD)
        {
            Speak("Oh it you. Stronger than lover's love is lover's hate. ");
            AddCaptionChangeCue(2, "Incurable, in each, the wounds they make.");
            AddCaptionChangeCue(4, "And i'm sure you have an incurable love.");
            SetFinishCallback(() =>
            {
                Main.PlayerCharacter.ShowStandardDialogueMenu(
                    "Perhaps, I'm just trying to get by after all.",
                    "That's really none of your concern.",
                    "Don't confuse it with an incurable hate.",
                    onResponseChosen);
            });
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
                Speak(" Aren't we all, but let us come and go and talk of Michelangelo. ", null);
                Reset();
            }
            else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
            {
                Speak("Ah yes The harbour of refuge,");
                AddCaptionChangeCue(2, "the cool cave,");
                AddCaptionChangeCue(4, "the island amidst the floods,");
                AddCaptionChangeCue(6, "the place of bliss,");
                AddCaptionChangeCue(7, "emancipation,");
                AddCaptionChangeCue(8, "liberation,");
                AddCaptionChangeCue(9, "safety,");
                AddCaptionChangeCue(10, "the supreme,");
                AddCaptionChangeCue(11, "the transcendent,");
                AddCaptionChangeCue(12, "the uncreated,");
                AddCaptionChangeCue(13, "the tranquil,");
                AddCaptionChangeCue(14, "the home of peace,");
                AddCaptionChangeCue(15, "the calm,");
                AddCaptionChangeCue(17, "the end of suffering,");
                AddCaptionChangeCue(19, "the medicine for all evil,");
                AddCaptionChangeCue(20, "the unshaken,");
                AddCaptionChangeCue(23, "the ambrosia.");
                SetFinishCallback(() =>
                {
                    reputation = Reputation.GOOD;
                    Reset();
                });
            }
            else if (response == PlayerCharacter.ResponseType.SELFISH)
            {
                Speak("Earth provides enough to satisfy every man's needs, but not every man's greed");

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
            if (response == PlayerCharacter.ResponseType.PRIDEFUL)
            {
                Main.PlayerCharacter.Speak("I'll never have nothing,");
                Main.PlayerCharacter.AddCaptionChangeCue(3, "I've dreams and the ambitions that i'm obligated to. ");
                Main.PlayerCharacter.SetFinishCallback(() =>
                {
                    Speak("If you want to live a happy life, tie it to this goal, not to people or thing. ", null, () =>
                    {
                        Main.PlayerCharacter.Speak("but my goal is to be able to earn back the right to wish my son goodnight.");
                        Main.PlayerCharacter.AddCaptionChangeCue(3, "I couldn't survive in a world without him. ");
                        Main.PlayerCharacter.SetFinishCallback(() =>
                        {
                            Speak("(Comforting) And yet that is the world you live.");
                            AddCaptionChangeCue(4, "Here I hope this helps you. ");
                            SetFinishCallback(() =>
                            {
                                reputation = Reputation.END;
                                Reset();


                            });
                        });
                    });
                });
         }
            else if (response == PlayerCharacter.ResponseType.SUBMISSIVE)
            {
                Speak("What did you have in mind, after all; All the world's a stage, And all the men and women merely players;");
                AddCaptionChangeCue(4, "We have our exits and our entrances-");
                SetFinishCallback(() =>
                    {
                        Main.PlayerCharacter.Speak("-And one man in his time plays many parts.");
                        Main.PlayerCharacter.AddCaptionChangeCue(3, "what characters pray tell have you played?");
                        Main.PlayerCharacter.SetFinishCallback(() =>
                            {
                                Speak("None freer than the one i'm playing now.");
                                AddCaptionChangeCue(3, "I do not care so much what I am to the audience as I care what I am to myself.");
                                reputation = Reputation.END;
                                Reset();
                            });
                     });
            }
            else if (response == PlayerCharacter.ResponseType.SELFISH)
            {
                Speak("Do not be afraid; our fate cannot be taken from us; it is a gift.");
                AddCaptionChangeCue(4, "My friend, amongst us rough sleepers it is our pride that sees us through the cold and the hunger.");
                AddCaptionChangeCue(8, "Our minds become swords with which to battle the idleness of regretful days.");
                AddCaptionChangeCue(11, "If you let your mind blunt then you you'll be left with no strength all together. ");
                SetFinishCallback(() =>
                {
                    reputation = Reputation.END;
                    Reset();
                });
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
                Speak("This is true, To love or have loved, that is enough.");
                AddCaptionChangeCue(3, "Ask nothing further");
                AddCaptionChangeCue(6, "There is no other pearl to be found in the dark folds of life.");
                SetFinishCallback(() =>
                {
                    reputation = Reputation.END;
                    Reset();
                });
            }
            else if (response == PlayerCharacter.ResponseType.PRIDEFUL)
            {
                Speak("My great concern is not whether you have failed, but whether you are content with your failure.", null);
                reputation = Reputation.END;
                Reset();
            }
            else if (response == PlayerCharacter.ResponseType.SELFISH)
            {
                Speak("They say The risk of a wrong decision is preferable to the terror of indecision.");
                AddCaptionChangeCue(4, "My attempts to reason with you have clearly been the misjudged.");
                SetFinishCallback(() =>
                {
                    reputation = Reputation.END;
                    Reset();
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
