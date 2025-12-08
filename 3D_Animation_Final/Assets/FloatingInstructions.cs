using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class FloatingInstructions : MonoBehaviour
{
    [Header("Player & Positioning")]
    public Transform playerCamera;
    public float distanceFromPlayer = 2.0f;
    public float heightOffset = -0.5f;
    public float horizontalOffset = 0.5f;
    public float smoothSpeed = 5f;
    public KeyCode toggleKey = KeyCode.X;

    [Header("Localization")]
    [SerializeField] private LocalizationTable strings;
    [SerializeField] private string stepKeyPrefix = "step.";

    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;

    [Header("Instruction Settings")]
    public float nonActionStepDelay = 5f;
    public List<InstructionStep> steps = new List<InstructionStep>();

    [Header("Grouped Instruction Lists")]
    public List<InstructionGroup> instructionGroups = new List<InstructionGroup>();

    [Header("Audio Settings")]
    public AudioSource audioSource;

    private bool isHidden = false;
    private bool canAdvance = true;
    private float originalHeightOffset;

    private string currentGroupName;

    private void Start()
    {
        if (instructionGroups.Count == 0 && steps.Count == 0)
        {
            Debug.LogError("No instructions or groups assigned!");
            return;
        }

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        originalHeightOffset = heightOffset;
        ShowNextStep();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleUIVisibility();

        if (Input.GetMouseButtonDown(0) && canAdvance)
            TryAdvanceStep();
    }

    private void LateUpdate()
    {
        if (playerCamera == null) return;

        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;
        Vector3 targetPos = playerCamera.position + forward * distanceFromPlayer + right * horizontalOffset;
        targetPos.y += heightOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0);
    }

    private void ToggleUIVisibility()
    {
        heightOffset = isHidden ? originalHeightOffset : -2.0f;
        isHidden = !isHidden;
    }

    private void TryAdvanceStep()
    {
        var step = GetNextAvailableStep();
        if (step == null) return;

        switch (step.actionType)
        {
            case ActionType.SimpleClick:
                CompleteStep(step);
                break;
            case ActionType.ClickObject:
                if (step.targetObject != null && WasObjectClicked(step.targetObject))
                    CompleteStep(step);
                break;
            case ActionType.CustomMethod:
                step.customActionEvent?.Invoke();
                break;
        }
    }

    private void CompleteStep(InstructionStep step)
    {
        step.actionComplete = true;
        HandlePostActions(step);
        ShowNextStep();
    }

    private void HandlePostActions(InstructionStep step)
    {
        foreach (var action in step.postActions)
        {
            switch (action.type)
            {
                case PostActionType.MoveObject:
                    if (action.objectToMove != null)
                        action.objectToMove.transform.position += action.moveDirection.normalized * action.moveDistance;
                    break;
                case PostActionType.MoveOntoBody:
                    if (action.worldObject != null) action.worldObject.SetActive(false);
                    if (action.playerObject != null) action.playerObject.SetActive(true);
                    break;
                case PostActionType.MoveBackIntoWorld:
                    if (action.worldObject != null) action.worldObject.SetActive(true);
                    if (action.playerObject != null) action.playerObject.SetActive(false);
                    break;
                case PostActionType.DestroyObject:
                    if (action.objectToDestroy != null) Destroy(action.objectToDestroy);
                    break;
                case PostActionType.MakeObjectVisible:
                    Debug.Log("Making object visible: " + action.objectToMakeVisible);
                    if (action.objectToMakeVisible != null) action.objectToMakeVisible.SetActive(true);
                    break;
                case PostActionType.DestroyAllWithTag:
                    foreach (var obj in GameObject.FindGameObjectsWithTag(action.tagToDestroy))
                        Destroy(obj);
                    break;
                case PostActionType.MarkStepIncomplete:
                    if (!string.IsNullOrEmpty(action.stepToMarkIncomplete))
                        MarkStepAsNotCompleted(action.stepToMarkIncomplete);
                    break;
                case PostActionType.CustomMethod:
                    action.postActionEvent?.Invoke();
                    break;
            }
        }
    }

    public void NotifyStepCompletedFromEvent(string stepName)
    {
        var step = FindStepByName(stepName);
        if (step != null)
        {
            CompleteStep(step);
        }
    }

    public bool IsStepCompleted(string stepName) 
    { 
        InstructionStep step = steps.FirstOrDefault(s => s.stepName == stepName);
        Debug.Log(step.stepName);
        Debug.Log("Is step complete? " + step.actionComplete);
        return step != null && step.actionComplete; 
    }

    public void ShowNextStep()
    {
        var step = GetNextAvailableStep();

        if (step != null)
        {
            currentGroupName = GetGroupOfStep(step)?.groupName;
            Debug.Log($"[NEXT STEP] Showing '{step.stepName}' from group '{currentGroupName}'");

            string display = (strings != null)
                ? strings.Get(MakeStepKey(step.stepName))
                : step.instruction;

            instructionText.text = display;
            PlayNarration(step.narrationClip);

            if (step.actionType == ActionType.SimpleClick)
                StartCoroutine(EnableClickAfterDelay());
        }
        else
        {
            instructionText.text = "Exercise Complete!";
            Debug.Log("[COMPLETE] All steps completed.");
        }
    }

    private InstructionStep GetNextAvailableStep()
    {
        foreach (var group in instructionGroups)
        {
            if (!group.isEnabled)
            {
                Debug.Log($"[SKIP] Group '{group.groupName}' disabled");
                continue;
            }

            var step = group.steps.FirstOrDefault(s => !s.actionComplete);
            if (step != null)
            {
                Debug.Log($"[FOUND] Step '{step.stepName}' in group '{group.groupName}'");
                return step;
            }
        }

        var legacyStep = steps.FirstOrDefault(s => !s.actionComplete);
        if (legacyStep != null)
            Debug.Log($"[FOUND] Legacy step '{legacyStep.stepName}'");

        return legacyStep;
    }

    private string MakeStepKey(string stepName)
    {
        if (string.IsNullOrWhiteSpace(stepName)) return string.Empty;
        var normalized = new string(stepName.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
        return (stepKeyPrefix ?? string.Empty) + normalized;
    }

    private bool WasObjectClicked(GameObject obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == obj;
    }

    private IEnumerator EnableClickAfterDelay()
    {
        canAdvance = false;
        yield return new WaitForSeconds(nonActionStepDelay);
        canAdvance = true;
    }

    private void PlayNarration(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private InstructionGroup GetGroupOfStep(InstructionStep step)
    {
        return instructionGroups.FirstOrDefault(g => g.steps.Contains(step));
    }

    public void EnableGroup(string groupName)
    {
        var group = instructionGroups.FirstOrDefault(g => g.groupName == groupName);
        if (group != null)
        {
            group.isEnabled = true;
            Debug.Log($"Group '{groupName}' enabled.");
            ShowNextStep();
        }
    }

    public void DisableGroup(string groupName)
    {
        var group = instructionGroups.FirstOrDefault(g => g.groupName == groupName);
        if (group != null)
        {
            group.isEnabled = false;
            Debug.Log($"Group '{groupName}' disabled.");
        }
    }

    public void MarkStepAsCompleted(string stepName)
    {
        var step = FindStepByName(stepName);
        if (step != null)
        {
            step.actionComplete = true;
            ShowNextStep();
        }
    }

    public void MarkStepAsNotCompleted(string stepName)
    {
        var step = FindStepByName(stepName);
        if (step != null)
        {
            step.actionComplete = false;
            ShowNextStep();
        }
    }

    private InstructionStep FindStepByName(string stepName)
    {
        foreach (var group in instructionGroups)
        {
            var step = group.steps.FirstOrDefault(s => s.stepName == stepName);
            if (step != null)
                return step;
        }

        return steps.FirstOrDefault(s => s.stepName == stepName);
    }
}
