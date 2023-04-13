using RealityCollective.ServiceFramework.Services;
using RealityToolkit.CameraService.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigMoveTest : MonoBehaviour
{
    private ICameraService cameraService;

    private void Start()
    {
        cameraService = ServiceManager.Instance.GetService<ICameraService>();
        cameraService.CameraOutOfBounds += CameraService_CameraOutOfBounds;
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(0f, 0f, 1f));
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(0f, 0f, -1f));
        }
        if (Keyboard.current.aKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(-1f, 0f, 0f));
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            cameraService.CameraRig.Move(new Vector3(1f, 0f, 0f));
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(0f, 0f, 1f));
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(0f, 0f, -1f));
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(-1f, 0f, 0f));
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            cameraService.CameraRig.CameraTransform.Translate(Time.deltaTime * new Vector3(1f, 0f, 0f));
        }
    }

    private void CameraService_CameraOutOfBounds(Vector3 returnToBoundsDirection)
    {
        Debug.Log("CAMERA OUT OF BOUNDS");
    }
}
