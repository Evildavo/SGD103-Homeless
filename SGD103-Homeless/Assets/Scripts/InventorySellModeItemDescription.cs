using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventorySellModeItemDescription : MonoBehaviour {

    public Text ItemName;
    public Text ItemValue;
    
    public Transform CentrePosition;

    [ReadOnly]
    public GameObject Source;

}
