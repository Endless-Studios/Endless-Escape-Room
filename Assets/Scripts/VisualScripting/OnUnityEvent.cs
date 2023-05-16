using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine.Events;

internal class OnUnityEventData : EventUnit<EventParameters>.Data 
{
    public object Event { get; set; }
}

public class EventParameters 
{
    public object t0 { get; set; }
    public object t1 { get; set; }
    public object t2 { get; set; }
    public object t3 { get; set; }
}

/// <summary>
/// Allows a more natural reference to Unity events with up to four parameters
/// </summary>
[UnitTitle("On Unity Event")]
[UnitCategory("Events")]
public class OnUnityEvent : EventUnit<EventParameters>
{
    protected override bool register => false;
    [DoNotSerialize] public ValueInput UnityEvent;
    Type type;

    public override IGraphElementData CreateData()
    {
        return new OnUnityEventData();
    }

    protected override void AssignArguments(Flow flow, EventParameters args)
    {
        int outputCount = valueOutputs.Count;

        if(outputCount > 0)
            flow.SetValue(valueOutputs[0], args.t0);
        if(outputCount > 1)
            flow.SetValue(valueOutputs[1], args.t1);
        if(outputCount > 2)
            flow.SetValue(valueOutputs[2], args.t2);
        if(outputCount > 3)
            flow.SetValue(valueOutputs[3], args.t3);
    }

    protected override void Definition()
    {
        base.Definition();

        UnityEvent = ValueInput<UnityEventBase>("event");

        if(type != null)
        {
            Type[] genericArguments = type.GetGenericArguments();
            for(int index = 0; index < genericArguments.Length; index++)
            {
                ValueOutput(genericArguments[index], $"arg{index}");
            }
        }
    }

    public override void StartListening(GraphStack stack)
    {
        OnUnityEventData data = stack.GetElementData<OnUnityEventData>(this);

        if(data.Event == null && UnityEvent.hasValidConnection)
        {
            Refresh();

            GraphReference graphReference = stack.ToReference();
            MethodInfo method = type.GetMethod(nameof(UnityEngine.Events.UnityEvent.AddListener));
            Type delegateType = method?.GetParameters()[0].ParameterType;
            data.Event = GetEventMethod(delegateType, graphReference);

            UnityEventBase eventBase = Flow.FetchValue<UnityEventBase>(UnityEvent, graphReference);
            method?.Invoke(eventBase, new[] { data.Event });
        }
    }

    public override void StopListening(GraphStack stack)
    {
        OnUnityEventData data = stack.GetElementData<OnUnityEventData>(this);

        if(data.Event != null)
        {
            if(UnityEvent != null)
            {
                GraphReference stackRef = stack.ToReference();
                UnityEventBase eventBase = Flow.FetchValue<UnityEventBase>(UnityEvent, stackRef);
                MethodInfo method = type.GetMethod(nameof(UnityEngine.Events.UnityEvent.RemoveListener));
                method?.Invoke(eventBase, new[] { data.Event });
            }

            data.Event = null;
        }
    }

    public void Refresh()
    {
        type = GetEventBaseType();
        Define();
    }

    private Type GetEventBaseType()
    {
        Type eventType = UnityEvent?.connection?.source?.type;

        while(eventType != null && eventType.BaseType != typeof(UnityEventBase))
            eventType = eventType.BaseType;

        return eventType;
    }

    private object GetEventMethod(Type delegateType, GraphReference graphReference)
    {
        if(delegateType.IsGenericType)
        {
            Type[] argumentTypes = delegateType.GetGenericArguments();
            int argumentLength = argumentTypes.Length;
            MethodInfo[] localMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach(MethodInfo methodInfo in localMethods)
            {
                if(methodInfo.ContainsGenericParameters && methodInfo.Name == nameof(ProcessEvent))
                {
                    Type[] arguments = methodInfo.GetGenericArguments();
                    if(argumentLength == arguments.Length)
                    {
                        return methodInfo.MakeGenericMethod(argumentTypes).Invoke(this, new object[] { graphReference });
                    }
                }
            }
        }
        return (UnityAction)(() => Trigger(graphReference, new EventParameters()));
    }

    internal UnityAction<T> ProcessEvent<T>(GraphReference graphReference)
    {
        return (t1) =>
        {
            Trigger(graphReference, new EventParameters
            {
                t0 = t1
            });
        };
    }

    internal UnityAction<T0, T1> ProcessEvent<T0, T1>(GraphReference graphReference)
    {
        return (t0, t1) =>
        {
            Trigger(graphReference, new EventParameters
            {
                t0 = t0,
                t1 = t1
            });
        };
    }

    internal UnityAction<T0, T1, T2> ProcessEvent<T0, T1, T2>(GraphReference graphReference)
    {
        return (t0, t1, t2) =>
        {
            Trigger(graphReference, new EventParameters
            {
                t0 = t0,
                t1 = t1,
                t2 = t2
            });
        };
    }

    internal UnityAction<T0, T1, T2, T3> ProcessEvent<T0, T1, T2, T3>(GraphReference graphReference)
    {
        return (t0, t1, t2, t3) =>
        {
            Trigger(graphReference, new EventParameters
            {
                t0 = t0,
                t1 = t1,
                t2 = t2,
                t3 = t3
            });
        };
    }
}
