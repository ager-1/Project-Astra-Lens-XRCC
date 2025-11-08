using UnityEngine;

public class CircularPath : MonoBehaviour
{

    public Transform target;
    public float speed = 2f;
    public float radius = 1f;
    public float angle = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float x = target.position.x + Mathf.Cos(angle) * radius;
        float y = target.position.y;
        float z = target.position.z + Mathf.Sin(angle) * radius;


        transform.position = new Vector3(x, y, z);

        angle += speed * Time.deltaTime;

    }
}
