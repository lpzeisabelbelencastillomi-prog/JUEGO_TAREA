using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PaddleController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;

    [Header("Movement")]
    [Min(1f)] public float maxSpeed = 9.5f;
    [Min(1f)] public float acceleration = 44f;
    [Min(1f)] public float deceleration = 58f;
    public float minY = -3.55f;
    public float maxY = 3.55f;

    public float CurrentVelocityY { get; private set; }

    Rigidbody2D rb;
    float input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        input = 0f;
        if (Input.GetKey(upKey)) input += 1f;
        if (Input.GetKey(downKey)) input -= 1f;
    }

    void FixedUpdate()
    {
        float target = input * maxSpeed;
        float rate = Mathf.Abs(input) > 0.01f ? acceleration : deceleration;
        CurrentVelocityY = Mathf.MoveTowards(CurrentVelocityY, target, rate * Time.fixedDeltaTime);

        Vector2 next = rb.position + Vector2.up * CurrentVelocityY * Time.fixedDeltaTime;
        next.y = Mathf.Clamp(next.y, minY, maxY);
        rb.MovePosition(next);

        if ((next.y <= minY + 0.001f && CurrentVelocityY < 0f) ||
            (next.y >= maxY - 0.001f && CurrentVelocityY > 0f))
            CurrentVelocityY = 0f;
    }
}
