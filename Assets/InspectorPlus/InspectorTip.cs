#if UNITY_EDITOR
[System.AttributeUsage(System.AttributeTargets.Field)]
public class InspectorTip : System.Attribute
{
    public string tip;

    public InspectorTip(string toolTip)
    {
        tip = toolTip;
    }
}
#endif