using UnityEngine;
using System.Collections;

public class CoOpSign : MonoBehaviour
{
    public Main Main;
    public Trigger Trigger;
    public CoOpShelter CoOpShelter;
    public MeshRenderer Face;
    public Material FaceNone;
    public Material FaceSoupKitchen;
    public Material FaceTherapy;
    public Material FaceAddictSupport;

    void Start()
    {
        Trigger.RegisterOnTriggerListener(OnTrigger);
        Trigger.RegisterOnCloseRequested(Reset);
    }

    void Update()
    {
        if (CoOpShelter.SoupKitchenEvent.IsOpen)
        {
            Face.material = FaceSoupKitchen;
        }
        else if (CoOpShelter.CounsellingEvent.IsOpen)
        {
            Face.material = FaceTherapy;
        }
        else if (CoOpShelter.AddictionSupportEvent.IsOpen)
        {
            Face.material = FaceAddictSupport;
        }
        else
        {
            Face.material = FaceNone;
        }
    }

    public void OnTrigger()
    {
        CoOpShelter.ReadNoticeBoard();
    }

    public void Reset()
    {
        Main.MessageBox.ShowNext();
        Trigger.Reset();
    }
    
}
