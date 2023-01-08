using UnityEngine;
using Random = UnityEngine.Random;

public class MoveInDirection : MonoBehaviour
{
    public Vector2 direction;
    public float randomize = 0.2f;

    private void Start()
    {
        direction = direction.normalized * direction.magnitude * Random.Range(1 - randomize, 1+ randomize);
    }

    public void Update()
    {
        transform.Translate(direction * Time.deltaTime);
    }
}
