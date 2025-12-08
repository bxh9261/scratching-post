using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum ActionType
{
    SimpleClick,
    ClickObject,
    CustomMethod
}

public enum PostActionType
{
    SimpleProceed,
    MoveObject,
    MoveOntoBody,
    MakeObjectVisible,
    DestroyObject,
    DestroyAllWithTag,
    MarkStepIncomplete,
    MoveBackIntoWorld,
    CustomMethod
}

[System.Serializable]
public class PostAction
{
    public PostActionType type;

    public GameObject objectToMove;
    public Vector3 moveDirection;
    public float moveDistance = 1f;

    public GameObject worldObject;
    public GameObject playerObject;

    public GameObject objectToMakeVisible;

    public GameObject objectToDestroy;
    public string tagToDestroy;

    public string stepToMarkIncomplete;

    public UnityEvent postActionEvent;
}

[System.Serializable]
public class InstructionStep
{
    [Header("Basic Info")]
    public string stepName;
    public AudioClip narrationClip;

    [Header("Deprecated (use localization key via stepName)")]
    [TextArea(2, 5)] public string instruction;

    [Header("Step Control")]
    public bool actionComplete = false;

    [Header("Action To Complete")]
    public ActionType actionType = ActionType.SimpleClick;
    public GameObject targetObject;
    public UnityEvent customActionEvent;

    [Header("After Action Completed")]
    public List<PostAction> postActions = new List<PostAction>();
}