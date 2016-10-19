using UnityEngine;
using System.Collections;

public class ResumeItem : MultiUseItem
{
    public MessageBox MessageBox;

    /*[Header("Resumes tailored to a current job openings are more effective")]
    public int RelevantToDay = 0;*/

    public override void OnPrimaryAction()
    {
        MessageBox.ShowForTime("[Insert backstory here]", 2.0f, gameObject);
    }

    void Update()
    {
        UpdateItemDescription();
    }
}
