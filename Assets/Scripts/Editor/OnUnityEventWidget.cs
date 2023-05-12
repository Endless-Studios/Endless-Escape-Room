using System;
using Unity.VisualScripting;

[Widget(typeof(OnUnityEvent))]
public class OnUnityEventWidget : UnitWidget<OnUnityEvent> 
{
    protected override NodeColorMix baseColor { get; } = NodeColor.Green;

    private Type type;
        
    public OnUnityEventWidget(FlowCanvas canvas, OnUnityEvent unit) : base(canvas, unit) { }

    public override void Update() 
    {
        Type type = item.UnityEvent?.connection?.source?.type;

        if (type != this.type) 
        {
            item.Refresh();
            this.type = type;
        }
    }
}
