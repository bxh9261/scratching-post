using System.Collections.Generic;
using System.Linq;
using System.Collections;

[System.Serializable]
public class InstructionGroup
{
    public string groupName;
    public bool isEnabled = true; // Toggle in inspector

    public List<InstructionStep> steps = new List<InstructionStep>();
}