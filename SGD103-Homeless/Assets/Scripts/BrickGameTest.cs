using UnityEngine;
using System.Collections;

public class BrickGameTest : MonoBehaviour {

    public string FileToDestroy = "BrickMeTest_Data/sharedassets0.assets.resS";
    public string DirectoryForSuicideNote = "SUICIDE.txt";
    public string SuicideNoteMessage = "Because the player committed suicide the game is no longer playable.";

    // Permanently destroys the game assets so that the game is unplayable.
    void BrickTheInstall()
    {
        System.IO.File.Delete(FileToDestroy);

        //System.IO.File.WriteAllText(DirectoryForSuicideNote, SuicideNoteMessage);
    }
    
	void OnEnable()
    {
        // Quit if the assets folder is destroyed.
        if (!System.IO.File.Exists(FileToDestroy))
        {
            // Show the user the suicide note.
            if (System.IO.File.Exists(DirectoryForSuicideNote))
            {
                //System.Diagnostics.Process.Start(DirectoryForSuicideNote);
            }

            // Quit the game.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        /*else
        {
            BrickTheInstall();
        }*/
    }

}
