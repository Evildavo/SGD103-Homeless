using UnityEngine;
using System.Collections;

public class AlcoholItem : MultiUseItem
{
    public MessageBox MessageBox;
    
    public override void OnPrimaryAction()
    {
        Debug.Log("Drank alcohol");
    }

    void Update()
    {
        UpdateItemDescription();
    }
}
