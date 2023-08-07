using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [Tooltip("Register the name of the first scene to load")]
    [SerializeField]
    private string sceneToLoad;

    void Start()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}