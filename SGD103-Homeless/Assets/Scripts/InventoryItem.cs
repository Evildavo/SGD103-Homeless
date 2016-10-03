using UnityEngine;
using System.Collections;

public class InventoryItem : MonoBehaviour {

    public string ItemName;
    public string PrimaryActionDescription;
    public int InventoryIndex;

    // Override to do some action when the primary item action is performed.
    public virtual void OnPrimaryAction() {}

}
