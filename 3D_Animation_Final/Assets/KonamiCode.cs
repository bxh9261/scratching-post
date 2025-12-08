using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KonamiCode : MonoBehaviour
{
    private List<KeyCode> konamiSequence = new List<KeyCode> {
        KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.B, KeyCode.A, KeyCode.Return
    };

    private List<KeyCode> currentInput = new List<KeyCode>();
    private float resetTime = 5f; // Time before input resets
    private Coroutine resetCoroutine;
    private bool easterEggTriggered = false;

    [Header("🔥 Lab Fire Effect 🔥")]
    public GameObject FlamesKonami; // Assign this in the Inspector (Disable it at start)

    private void Update()
    {
        if (easterEggTriggered) return;

        if (Input.anyKeyDown)
        {
            KeyCode keyPressed = GetCurrentKeyPressed();

            if (keyPressed != KeyCode.None)
            {
                currentInput.Add(keyPressed);
                Debug.Log("Konami Code Progress: " + currentInput.Count + "/" + konamiSequence.Count);

                // Reset if input is incorrect
                if (currentInput.Count > konamiSequence.Count || currentInput[currentInput.Count - 1] != konamiSequence[currentInput.Count - 1])
                {
                    Debug.Log("Wrong input! Resetting...");
                    ResetInput();
                    return;
                }

                // Start reset timer
                if (resetCoroutine != null) StopCoroutine(resetCoroutine);
                resetCoroutine = StartCoroutine(ResetAfterDelay());

                // Check if the code is complete
                if (currentInput.Count == konamiSequence.Count)
                {
                    TriggerLabFire();
                }
            }
        }
    }

    private KeyCode GetCurrentKeyPressed()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key)) return key;
        }
        return KeyCode.None;
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(resetTime);
        ResetInput();
    }

    private void ResetInput()
    {
        currentInput.Clear();
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
    }

    private void TriggerLabFire()
    {
        easterEggTriggered = true;
        Debug.Log("🔥🔥🔥 Konami Code Activated! The Lab is on Fire! 🔥🔥🔥");

        // Enable FlamesKonami GameObject
        if (FlamesKonami != null)
        {
            FlamesKonami.SetActive(true);
        }
        else
        {
            Debug.LogWarning("FlamesKonami GameObject is not assigned!");
        }

        // 🔥 Reset after 10 seconds
        StartCoroutine(ResetLabFire());
    }

    private IEnumerator ResetLabFire()
    {
        yield return new WaitForSeconds(10f);

        if (FlamesKonami != null)
        {
            FlamesKonami.SetActive(false);
        }

        Debug.Log("🔥 Fire has been extinguished.");
        easterEggTriggered = false;
        ResetInput();
    }
}
