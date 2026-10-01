using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    public float startSpeed = 8f;
    public float maxSpeed = 12f;
    private Rigidbody2D rb;
    private bool activeBall;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetBall()
    {
        activeBall = false;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void Launch()
    {
        activeBall = true;
        float x = Random.value > 0.5f ? 1f : -1f;
        float y = Random.Range(-0.55f, 0.55f);
        Vector2 direction = new Vector2(x, y).normalized;
        rb.linearVelocity = direction * startSpeed;
    }

    void FixedUpdate()
    {
        if (!activeBall || Time.timeScale == 0f) return;
        float speed = rb.linearVelocity.magnitude;
        if (speed < startSpeed * 0.9f && speed > 0.1f)
            rb.linearVelocity = rb.linearVelocity.normalized * startSpeed;
        if (speed > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string n = collision.gameObject.name;
        if (n.Contains("Paddle"))
        {
            float half = collision.collider.bounds.extents.y;
            float offset = half > 0.01f ? (transform.position.y - collision.transform.position.y) / half : 0f;
            float x = collision.transform.position.x < 0f ? 1f : -1f;
            float newSpeed = Mathf.Min(maxSpeed, Mathf.Max(startSpeed, rb.linearVelocity.magnitude + 0.25f));
            rb.linearVelocity = new Vector2(x, Mathf.Clamp(offset, -0.9f, 0.9f)).normalized * newSpeed;
            AudioManager.Instance?.PlayBounce();
        }
        else
        {
            AudioManager.Instance?.PlayWall();
        }
    }
}
