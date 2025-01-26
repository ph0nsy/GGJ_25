using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute condAttr = (ConditionalFieldAttribute)attribute;
        SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttr.conditionalSourceField);

        if (sourceProperty != null && sourceProperty.intValue > condAttr.requiredValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalFieldAttribute condAttr = (ConditionalFieldAttribute)attribute;
        SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttr.conditionalSourceField);

        if (sourceProperty != null && sourceProperty.intValue > condAttr.requiredValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        else
        {
            return 0f;
        }
    }
}
