using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BallController : MonoBehaviour
{
    [Header("Speed")]
    [Min(1f)] public float startSpeed = 10.6f;
    [Min(1f)] public float speedGainPerPaddle = 1.42f;
    [Min(1f)] public float maxSpeed = 12.8f;
    [Range(0.15f, 0.75f)] public float minimumHorizontalRatio = 0.38f;

    [Header("Control / Feel")]
    [Range(0f, 1f)] public float impactInfluence = 0.74f;
    [Range(0f, 0.35f)] public float paddleVelocityInfluence = 0.11f;

    public bool IsActive { get; private set; }
    public float CurrentSpeed => rb == null ? 0f : rb.linearVelocity.magnitude;

    Rigidbody2D rb;
    Vector3 initialScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        initialScale = transform.localScale;
    }

    public void ResetBall()
    {
        IsActive = false;
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = Vector2.zero;
        transform.localScale = initialScale;
    }

    public void Launch(int horizontalDirection = 0)
    {
        IsActive = true;
        int xSign = horizontalDirection == 0 ? (Random.value < 0.5f ? -1 : 1) : (horizontalDirection < 0 ? -1 : 1);
        float y = Random.Range(-0.52f, 0.52f);
        Vector2 dir = new Vector2(xSign, y).normalized;
        rb.linearVelocity = dir * startSpeed;
    }

    public void StopBall()
    {
        IsActive = false;
        rb.linearVelocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (!IsActive || rb.linearVelocity.sqrMagnitude < 0.001f) return;

        float speed = Mathf.Clamp(rb.linearVelocity.magnitude, startSpeed, maxSpeed);
        Vector2 dir = rb.linearVelocity.normalized;

        // Avoid boring near-vertical loops.
        if (Mathf.Abs(dir.x) < minimumHorizontalRatio)
        {
            float sign = Mathf.Sign(dir.x == 0f ? (Random.value < 0.5f ? -1f : 1f) : dir.x);
            dir.x = sign * minimumHorizontalRatio;
            dir = dir.normalized;
        }

        rb.linearVelocity = dir * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PaddleController paddle = collision.collider.GetComponent<PaddleController>();
        if (paddle != null)
        {
            float halfHeight = Mathf.Max(0.01f, collision.collider.bounds.extents.y);
            float impact = Mathf.Clamp((transform.position.y - paddle.transform.position.y) / halfHeight, -1f, 1f);
            float x = paddle.transform.position.x < 0f ? 1f : -1f;
            float y = impact * impactInfluence + paddle.CurrentVelocityY * paddleVelocityInfluence;

            float nextSpeed = Mathf.Min(maxSpeed, Mathf.Max(startSpeed, CurrentSpeed + speedGainPerPaddle));
            Vector2 direction = new Vector2(x, Mathf.Clamp(y, -0.95f, 0.95f)).normalized;
            rb.linearVelocity = direction * nextSpeed;

            AudioManager.Instance?.PlayBounce(nextSpeed / maxSpeed);
            CameraShake2D.Instance?.Shake(0.045f, 0.055f);
        }
        else
        {
            AudioManager.Instance?.PlayWall();
            CameraShake2D.Instance?.Shake(0.018f, 0.035f);
        }
    }
}
