using System;
using System.Collections.Generic;
public class SubscriptionToken
{
    internal Type eventType;
    internal Action<object> action;
    
	internal SubscriptionToken(Type eventType, Action<object> action)
	{
		this.eventType = eventType;
		this.action = action;
	}
}
public static class EventBus
{
    private static Dictionary<Type, Action<object>> events = new Dictionary<Type, Action<object>>();

    public static SubscriptionToken Subscribe<T>(Action<T> listener)
    {
        Type type = typeof(T);

        Action<object> wrapper = (e) => listener((T)e);
        
        if (!events.ContainsKey(type))
            events[type] = null;

        events[type] += wrapper;

        return new SubscriptionToken(type, wrapper);
    }
    public static void Unsubscribe(SubscriptionToken token)
    {
        if(token != null)
            if (events.ContainsKey(token.eventType))
            {
                events[token.eventType] -= token.action;
            }
    }
    public static void Publish<T>(T e) where T: class
    {
        Type type = typeof(T);

        if (events.ContainsKey(type))
            events[type]?.Invoke(e);
    }

    public static void Clear()
    {
        events.Clear();
    }
}