using UnityEngine;

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string conditionalSourceField;
    public int requiredValue;

    public ConditionalFieldAttribute(string conditionalSourceField, int requiredValue)
    {
        this.conditionalSourceField = conditionalSourceField;
        this.requiredValue = requiredValue;
    }
}