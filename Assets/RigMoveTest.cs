using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigMoveTest : MonoBehaviour
{
    private ICameraService cameraService;

    private async void Start()
    {
        await ServiceManager.WaitUntilInitializedAsync();
        cameraService = ServiceManager.Instance.GetService<ICameraService>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(0f, 0f, 1f));
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(0f, 0f, -1f));
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(-1f, 0f, 0f));
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(1f, 0f, 0f));
        }


        if (Keyboard.current.qKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(0f, -1f, 0f));
        }
        else if (Keyboard.current.eKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(0f, 1f, 0f));
        }
        if (Keyboard.current.wKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(0f, 0f, 1f));
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(0f, 0f, -1f));
        }
        if (Keyboard.current.aKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(-1f, 0f, 0f));
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(1f, 0f, 0f));
        }
    }
}
