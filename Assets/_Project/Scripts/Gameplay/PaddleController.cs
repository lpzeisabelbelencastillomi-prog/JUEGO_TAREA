using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public float speed = 8.5f;
    public float minY = -3.45f;
    public float maxY = 3.45f;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        float direction = 0f;
        if (Input.GetKey(upKey)) direction += 1f;
        if (Input.GetKey(downKey)) direction -= 1f;
        Vector3 p = transform.position;
        p.y = Mathf.Clamp(p.y + direction * speed * Time.deltaTime, minY, maxY);
        transform.position = p;
    }
}
