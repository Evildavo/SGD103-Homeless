using UnityEngine;
using System.Collections;

public class ResumeItem : MultiUseItem
{
    /*[Header("Resumes tailored to a current job openings are more effective")]
    public int RelevantToDay = 0;*/

    public override void OnPrimaryAction()
    {
        Main.MessageBox.ShowForTime("[Insert backstory here]", 2.0f, gameObject);
    }

    new void Start()
    {
        base.Start();
    }

    void Update()
    {
        UpdateItemDescription();
    }
}
