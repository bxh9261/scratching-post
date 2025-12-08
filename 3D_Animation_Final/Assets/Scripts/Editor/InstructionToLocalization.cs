using UnityEngine;
using UnityEditor;

public class InstructionToLocalization : EditorWindow
{
    [MenuItem("Tools/Export Instructions to Localization")]
    public static void Export()
    {
        var instructions = GameObject.FindObjectOfType<FloatingInstructions>();
        if (instructions == null || instructions.steps == null)
        {
            Debug.LogError("No FloatingInstructions found in scene.");
            return;
        }

        // create or load the table
        var table = ScriptableObject.CreateInstance<LocalizationTable>();
        foreach (var step in instructions.steps)
        {
            var entry = new LocalizationTable.Entry();
            entry.key = "step." + step.stepName.ToLowerInvariant();
            entry.en = step.instruction;
            entry.fr = "";
            table.entries.Add(entry);
        }

        AssetDatabase.CreateAsset(table, "Assets/strings_auto.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("Export complete: Assets/strings_auto.asset");
    }
}
