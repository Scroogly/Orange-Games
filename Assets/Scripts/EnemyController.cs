using UnityEngine;

// This script controls my enemy movement, patrol behavior, and keeps the enemy upright.
[RequireComponent(typeof(EnemyHealthSimple))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    // How fast the enemy moves left and right.
    public float speed = 2f;

    [Header("Patrol range (relative to start X)")]
    // How far left from the start position the enemy is allowed to go.
    public float leftOffset = -3f;
    // How far right from the start position the enemy is allowed to go.
    public float rightOffset = 3f;
    // How close to the edge we have to be before we say "we reached it".
    [SerializeField] float turnTolerance = 0.02f;

    [Header("Visuals")]
    // Reference to the enemy's SpriteRenderer so I can flip the sprite.
    public SpriteRenderer sprite;

    // These are cached patrol limits so I don't recalculate them every frame.
    float startX, leftX, rightX;
    // I lock the Y position here so the enemy always stays on top of the platform.
    float startY;
    // This flag tells me if the enemy is currently moving to the right or to the left.
    bool movingRight = true;

    // Awake is called first when the object is created/enabled.
    // I use it to make sure I have a SpriteRenderer reference.
    public void Awake()
    {
        // If nobody set the sprite in the Inspector, I try to auto-find it.
        if (sprite == null)
        {
            // First check on this GameObject, then check children.
            sprite = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Start is called once before the first frame Update.
    // I use it to record the starting position and compute patrol limits.
    public void Start()
    {
        // Save the starting X and Y so I know where the patrol is centered.
        startX = transform.position.x;
        startY = transform.position.y;   // This is the Y height I keep the enemy locked to.

        // Calculate the world-space left and right patrol edges.
        // I use Min/Max so it works even if someone swaps the offsets.
        leftX = startX + Mathf.Min(leftOffset, rightOffset);
        rightX = startX + Mathf.Max(leftOffset, rightOffset);
    }

    // Update is called once per frame.
    // This is where I actually move the enemy back and forth.
    public void Update()
    {
        // Decide which patrol edge I am currently moving towards.
        float targetX = movingRight ? rightX : leftX;

        // I always use the locked startY so the enemy doesn't drift up or down.
        Vector2 targetPos = new Vector2(targetX, startY);

        // Move the enemy a little bit closer to the target position this frame.
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        // Check if I am close enough to the edge to count as "reached".
        if (Mathf.Abs(transform.position.x - targetX) <= turnTolerance)
        {
            // Snap exactly to the edge so I don’t hover slightly past it.
            transform.position = targetPos;

            // Flip direction: if I was moving right, now I move left, and vice versa.
            movingRight = !movingRight;

            // Flip the sprite visually so it faces the direction of travel.
            if (sprite != null)
            {
                Vector3 s = transform.localScale;
                // Positive scale.x means facing right, negative means facing left.
                s.x = movingRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
                transform.localScale = s;
            }
        }
    }

    // LateUpdate is called after all Update calls finish for the frame.
    // I use it to correct the rotation so enemies never tip over or spin.
    public void LateUpdate()
    {
        // Force the enemy back to "upright" every frame.
        transform.rotation = Quaternion.identity;
    }
}
