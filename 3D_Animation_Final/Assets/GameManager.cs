using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Optional Global References")]
    public FloatingInstructions floatingInstructions;

    private void Start()
    {
        if (floatingInstructions == null)
        {
            Debug.LogWarning("GameManager: FloatingInstructions is not assigned.");
        }

        // Initialize any global systems or scene-wide setup here
    }

    // Example: Scene transition utility
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Example: Reset all instruction steps
    public void ResetInstructions()
    {
        if (floatingInstructions != null)
        {
            foreach (var step in floatingInstructions.steps)
            {
                step.actionComplete = false;
            }
            floatingInstructions.ShowNextStep();
        }
    }

    // Example: Global pause/resume
    public void SetPaused(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
    }
}