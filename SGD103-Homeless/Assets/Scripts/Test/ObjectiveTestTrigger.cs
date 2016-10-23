using UnityEngine;
using System.Collections;

public class ObjectiveTestTrigger : MonoBehaviour {

    public Main Main;
    public Trigger Trigger;

	void Start () {
        Trigger.RegisterOnTriggerListener(onTrigger);
    }

    void onTrigger()
    {
        Main.ObjectiveList.NewObjective("Get more alcohol");
        Trigger.Reset();
    }
	
}
