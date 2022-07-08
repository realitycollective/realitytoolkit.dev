using RealityToolkit.EventDatum.Input;
using RealityToolkit.Interfaces.InputSystem.Handlers;
using RealityToolkit.Services.InputSystem.Listeners;
using UnityEngine;

public class DebugInput : InputSystemGlobalListener, IMixedRealityInputHandler
{
    public void OnInputDown(InputEventData eventData)
    {
        if (!InputSystem.TryGetController(eventData.InputSource, out var controller))
        {
            return;
        }

        Debug.Log($"{nameof(OnInputDown)} | {controller.GetType().Name} | {eventData.MixedRealityInputAction.Description}");
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (!InputSystem.TryGetController(eventData.InputSource, out var controller))
        {
            return;
        }

        Debug.Log($"{nameof(OnInputUp)} | {controller.GetType().Name} | {eventData.MixedRealityInputAction.Description}");
    }
}
