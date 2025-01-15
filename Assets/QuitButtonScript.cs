using UnityEngine;

public class QuitButtonScript: MonoBehaviour
{
    public void OnQuitButtonPressed()
    {
        // Logs a message to confirm the quit button was pressed (useful during testing in the editor)
        Debug.Log("Quit button pressed!");

        // Quits the application (this will only work in a built game, not in the editor)
        Application.Quit();
    }
}
