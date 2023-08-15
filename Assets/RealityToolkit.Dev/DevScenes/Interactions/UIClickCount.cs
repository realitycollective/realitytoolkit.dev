using UnityEngine;

public class UIClickCount : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI counterText = null;

    private int count;

    public void Click()
    {
        count++;
        counterText.text = $"Click Count: {count}";
    }
}
