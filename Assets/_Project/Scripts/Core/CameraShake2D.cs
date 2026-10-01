using System.Collections;
using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    public static CameraShake2D Instance { get; private set; }
    Vector3 baseLocalPosition;
    Coroutine routine;

    void Awake()
    {
        Instance = this;
        baseLocalPosition = transform.localPosition;
    }

    public void Shake(float strength, float duration)
    {
        if (!isActiveAndEnabled || strength <= 0f || duration <= 0f) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShakeRoutine(strength, duration));
    }

    IEnumerator ShakeRoutine(float strength, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float falloff = 1f - Mathf.Clamp01(t / duration);
            Vector2 offset = Random.insideUnitCircle * strength * falloff;
            transform.localPosition = baseLocalPosition + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }
        transform.localPosition = baseLocalPosition;
        routine = null;
    }
}
