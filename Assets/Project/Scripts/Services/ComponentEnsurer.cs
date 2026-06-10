using UnityEngine;

public static class ComponentEnsurer
{
    //
    public static T EnsureComponent<T>(T component, GameObject owner) where T : Component
    {
        if (component == null) component = owner.AddComponent<T>();
        return component;
    }
}
