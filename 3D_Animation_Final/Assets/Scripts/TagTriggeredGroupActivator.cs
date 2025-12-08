//DEPRECATED
/*
 * using UnityEngine;

public class TagTriggeredGroupActivator : MonoBehaviour
{
    [Header("Detection Settings")]
    public string tagToDetect = "Broken";
    public float checkInterval = 0.5f; // how often to check (seconds)

    [Header("Instruction Manager")]
    public FloatingInstructions instructionManager;
    public string interruptingGroup;

    private bool triggered = false;
    private float nextCheckTime = 0f;

    void Update()
    {
        // Skip if too soon since last check
        if (Time.time < nextCheckTime) return;
        nextCheckTime = Time.time + checkInterval;

        if (instructionManager == null) return;

        // Already triggered: stop checking until group is fully complete
        if (triggered)
        {
            var group = instructionManager.instructionGroups
                .Find(g => g.groupName == interruptingGroup);

            // If group completed, reset trigger for future events
            if (group != null && group.steps.TrueForAll(s => s.actionComplete))
            {
                Debug.Log($"[TAG ACTIVATOR] Group '{interruptingGroup}' completed. Resetting trigger.");
                triggered = false;
                group.isConditionMet = false;
            }
            return; // Don't re-trigger while active
        }

        // Only check if not already triggered
        bool foundTaggedObject = GameObject.FindGameObjectsWithTag(tagToDetect).Length > 0;

        if (foundTaggedObject)
        {
            var group = instructionManager.instructionGroups
                .Find(g => g.groupName == interruptingGroup);

            if (group == null)
            {
                Debug.LogWarning($"[TAG ACTIVATOR] Group '{interruptingGroup}' not found in FloatingInstructions.");
                return;
            }

            // Mark condition only once when entering SpillCleanup
            group.isConditionallyTriggered = true;
            group.isConditionMet = true;

            Debug.Log($"[TAG ACTIVATOR] Tag '{tagToDetect}' detected. Activating group '{interruptingGroup}'.");

            // Interrupt current flow and begin cleanup
            instructionManager.EnableGroup(interruptingGroup);
            instructionManager.InterruptWithGroup(interruptingGroup);

            // Stop all further detection until cleanup group is finished
            triggered = true;
        }
    }
}
*/
