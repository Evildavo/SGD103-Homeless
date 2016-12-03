using UnityEngine;
using System.Collections;

public class BeggingCollectionZone : MonoBehaviour {

    public Begging Begging;

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        Pedestrian pedestrian = other.GetComponent<Pedestrian>();
        if (pedestrian && pedestrian.IsInActiveHour)
        {
            Begging.CheckGotMoneyPedestrian();
        }
    }

}
