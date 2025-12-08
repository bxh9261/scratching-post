using UnityEngine;
using UnityEditor;

public class CenterPivotCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Pivot at Renderer Center")]
    static void CreatePivotAtCenter()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        Renderer renderer = selected.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Selected GameObject has no Renderer.");
            return;
        }

        Vector3 boundsCenter = renderer.bounds.center;

        // Create the new pivot GameObject at the bounds center
        GameObject pivot = new GameObject(selected.name + "_Pivot");
        pivot.transform.position = boundsCenter;

        // Calculate offset from current pivot to bounds center
        Vector3 offset = selected.transform.position - boundsCenter;

        // Parent the selected object to the new pivot
        selected.transform.SetParent(pivot.transform, true);

        // Move the child so its pivot aligns with the parent's origin
        selected.transform.localPosition = offset;

        // Select the new pivot in the editor
        Selection.activeGameObject = pivot;

        Debug.Log("Pivot created at renderer center and turbine repositioned.");
    }
}
