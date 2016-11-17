using UnityEngine;
using System.Collections;

public class CoOpSign : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    public CoOpShelter CoOpShelter;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
    }

    public void OnTrigger()
    {
        CoOpShelter.ReadNoticeBoard();
    }
    
}
