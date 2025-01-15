using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        // Replace "GameScene" with the name of your target scene
        SceneManager.LoadScene("PreScreen");
    }
}