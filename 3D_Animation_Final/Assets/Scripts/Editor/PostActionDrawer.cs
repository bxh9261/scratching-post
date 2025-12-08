using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(PostAction))]
public class PostActionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;
        SerializedProperty typeProp = property.FindPropertyRelative("type");

        height += EditorGUIUtility.singleLineHeight; // for the type dropdown

        switch ((PostActionType)typeProp.enumValueIndex)
        {
            case PostActionType.MoveObject:
                height += 3 * EditorGUIUtility.singleLineHeight;
                break;
            case PostActionType.MoveOntoBody:
                height += 2 * EditorGUIUtility.singleLineHeight;
                break;
            case PostActionType.MakeObjectVisible:
            case PostActionType.DestroyObject:
            case PostActionType.DestroyAllWithTag:
            case PostActionType.MarkStepIncomplete:
                height += EditorGUIUtility.singleLineHeight;
                break;
            case PostActionType.CustomMethod:
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("postActionEvent"));
                break;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(line, typeProp);

        line.y += EditorGUIUtility.singleLineHeight;

        switch ((PostActionType)typeProp.enumValueIndex)
        {
            case PostActionType.MoveObject:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("objectToMove"));
                line.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(line, property.FindPropertyRelative("moveDirection"));
                line.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(line, property.FindPropertyRelative("moveDistance"));
                break;

            case PostActionType.MoveOntoBody:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("worldObject"));
                line.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(line, property.FindPropertyRelative("playerObject"));
                break;

            case PostActionType.MakeObjectVisible:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("objectToMakeVisible"));
                break;

            case PostActionType.DestroyObject:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("objectToDestroy"));
                break;

            case PostActionType.DestroyAllWithTag:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("tagToDestroy"));
                break;

            case PostActionType.MarkStepIncomplete:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("stepToMarkIncomplete"));
                break;

            case PostActionType.CustomMethod:
                EditorGUI.PropertyField(line, property.FindPropertyRelative("postActionEvent"));
                break;
        }

        EditorGUI.EndProperty();
    }
}
