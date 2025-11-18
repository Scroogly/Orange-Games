using UnityEngine;
 
[RequireComponent(typeof(EnemyHealthSimple))]
 public class EnemyController : MonoBehaviour
{
    public float speed = 2f;
    [Header("Patrol range (relative to start X)")]
    public float leftOffset = -3f;
    public float rightOffset = 3f;
    [SerializeField] float turnTolerance = 0.02f;

    public SpriteRenderer sprite;

    float startX, leftX, rightX;
    bool movingRight = true;

    // Static type: declared as the superclass (EffectBase)
    private EffectBase turnEffect;

    // Find sprite if not assigned
    void Awake()
    {
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
    }

    // Setup patrol limits and assign the dynamic type (TurnEffect)
    void Start()
    {
        startX = transform.position.x;
        leftX = startX + Mathf.Min(leftOffset, rightOffset);
        rightX = startX + Mathf.Max(leftOffset, rightOffset);

        // Dynamic type: instance of subclass assigned to superclass variable
        turnEffect = new TurnEffect();
    }

    // Move left and right, flip direction at edges
    void Update()
    {
        float targetX = movingRight ? rightX : leftX;
        Vector2 targetPos = new Vector2(targetX, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position, targetPos, speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - targetX) <= turnTolerance)
        {
            transform.position = targetPos;
            movingRight = !movingRight;

            // Flip the sprite’s facing direction
            var s = transform.localScale;
            s.x = movingRight ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
            transform.localScale = s;

            // Call method through the static type (EffectBase)
            if (sprite != null)
                turnEffect.DoSomething(sprite);
        }
    }
}

// ---------- Super CLASS ----------
// Normal version: just white color, no visible change.
public class EffectBase
{
    // "virtual" means this method can be overridden by subclasses.
    public void DoSomething(SpriteRenderer sr)
    {
        if (sr == null) return;
        sr.color = Color.white; // base: stays white
    }
}

// ---------- SUBCLASS ----------
// Subclass overrides base behavior and changes the color.
public class TurnEffect : EffectBase
{
    // "override" replaces the base version when called through base type.
    public  void DoSomething(SpriteRenderer sr)
    {
        if (sr == null) return;
        sr.color = Color.red; // subclass: turns red when changing direction
    }
}
