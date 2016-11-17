using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SonPhotoItem : InventoryItem
{
    private bool isShowingPhoto = false;
    private float timeAtPhotoShown;
    private bool openedFromInventory;

    public Image SonPhotoImage;

    public float ShowPhotoForSeconds = 3.0f;
    
    // Shows the photo of the the player character's son briefly.
    public void ShowPhoto()
    {
        isShowingPhoto = true;
        SonPhotoImage.gameObject.SetActive(true);
        timeAtPhotoShown = Time.time;
    }

    public void HidePhoto()
    {
        SonPhotoImage.gameObject.SetActive(false);
        isShowingPhoto = false;
    }

    // Don't allow sale.
    public override void OnSellRequested()
    {
        Main.MessageBox.ShowForTime("You can't sell this", null, gameObject);
    }

    public override void OnPrimaryAction()
    {
        if (!isShowingPhoto)
        {
            ShowPhoto();
            openedFromInventory = true;
        }
        else
        {
            HidePhoto();
        }
    }

    void Update()
    {
        if (isShowingPhoto)
        {
            // Close the photo after time or after a button press.
            if (Time.time - timeAtPhotoShown > ShowPhotoForSeconds)
            {
                HidePhoto();
            }

            // Close on the user key press.
            if (Input.anyKeyDown && !openedFromInventory)
            {
                HidePhoto();
            }
        }
    }

}
