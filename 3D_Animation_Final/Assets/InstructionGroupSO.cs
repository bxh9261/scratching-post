using UnityEngine;

[CreateAssetMenu(fileName = "InstructionGroup", menuName = "Instructions/Instruction Group")]
public class InstructionGroupSO : ScriptableObject
{
    public string displayName;
    public bool isLocked = false;
}
