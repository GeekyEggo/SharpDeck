using SharpDeck.Events;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpDeck.Actions
{
    public class ActionManager
    {
        public ActionManager()
        {
            var delegates = new Dictionary<string, MethodInfo>();
            foreach (var method in typeof(IStreamDeckAction).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var eventName = method.GetCustomAttribute<StreamDeckEventAttribute>()?.Name;
                if (!string.IsNullOrWhiteSpace(eventName))
                {
                    delegates.Add(eventName, method);
                }
            }

            this.EventDelegates = delegates;
        }

        private IDictionary<string, Type> ActionsMap { get; } = new Dictionary<string, Type>();
        private IDictionary<string, IStreamDeckAction> Actions { get; } = new Dictionary<string, IStreamDeckAction>();
        private IReadOnlyDictionary<string, MethodInfo> EventDelegates { get; }

        public bool TryAction(StreamDeckClient client, StreamDeckEventInfo e, object args)
        {
            if (!(args is IActionInfo actionInfo) || !this.ActionsMap.TryGetValue(actionInfo.Action, out var actionType))
            {
                return false;
            }

            var key = $"{actionInfo.Device}|{actionInfo.Action}|{actionInfo.Context}";
            if (!this.Actions.TryGetValue(key, out var action))
            {
                action = (IStreamDeckAction)Activator.CreateInstance(actionType);
                action.Initialize(actionInfo, client);

                this.Actions.Add(key, action);
            }

            if (!this.EventDelegates.TryGetValue(e.Name, out var methodInfo))
            {
                return false;
            }

            var prop = args.GetType().GetProperty("Payload", BindingFlags.Instance | BindingFlags.Public);
            methodInfo.Invoke(action, new[] { prop.GetValue(args) });
            return true;
        }

        public void Register<T>(string actionUUID)
            where T : IStreamDeckAction, new()
        {
            if (this.ActionsMap.TryGetValue(actionUUID, out var type))
            {
                throw new ArgumentException($"An action with the UUID {actionUUID} has already been registered.");
            }

            this.ActionsMap.Add(actionUUID, typeof(T));
        }
    }
}
