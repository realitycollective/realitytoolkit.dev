using UnityEngine;

public class UIToggleState : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI counterText = null;

    public void ValueChanged(bool isOn)
    {
        counterText.text = $"Toggle State: {(isOn ? "On" : "Off")}";
    }
}
