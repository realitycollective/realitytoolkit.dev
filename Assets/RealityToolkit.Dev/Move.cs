using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    private Transform target;

    private void Awake()
    {
        target = transform;
    }

    private void Update()
    {
        var position = target.position;
        position.z += Time.deltaTime * speed;
        target.position = position;
    }
}
