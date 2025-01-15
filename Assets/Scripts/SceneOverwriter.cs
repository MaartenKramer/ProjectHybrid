using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneSwitcher : MonoBehaviour
{
    void Update()
    {
        // Check for key press to load specific scenes
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadScene("Art_test_Scene");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadScene("ENDscene");
        }
    }

    void LoadScene(string sceneName)
    {
        // Log and load the scene
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
