using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    [Header("Patrol range (relative to start X)")]
    public float leftOffset = -3f;
    public float rightOffset = 3f;
    [SerializeField] float turnTolerance = 0.02f;

    float startX, leftX, rightX;
    bool movingRight = true;

    void Start()
    {
        startX = transform.position.x;
        // compute world targets once
        leftX = startX + Mathf.Min(leftOffset, rightOffset);
        rightX = startX + Mathf.Max(leftOffset, rightOffset);
    }

    void Update()
    {
        float targetX = movingRight ? rightX : leftX;
        Vector2 targetPos = new Vector2(targetX, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position, targetPos, speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - targetX) <= turnTolerance)
        {
            transform.position = targetPos; // snap
            movingRight = !movingRight;

            // optional face flip
            var s = transform.localScale;
            s.x = movingRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
            transform.localScale = s;
        }
    }
}