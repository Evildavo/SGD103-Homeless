using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryItemDescription : MonoBehaviour {

    public Text ItemName;
    public Text ItemAction;

    [ReadOnly]
    public GameObject Source;
}
