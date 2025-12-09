/********************************************
 * File: PlayerPowerUp.cs
 * Author: Safal Shrestha
 * Description: Demonstrates DYNAMIC and STATIC binding
 *              using helper classes that return different
 *              power-up stat values.
 ********************************************/

using System.Collections;
using UnityEngine;

/// Power-up that temporarily boosts health and speed.
/// Demonstrates dynamic binding using helper effect classes.
public class PlayerPowerUp : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer[] _tintTargets;
    [SerializeField] private Color _activeTintColor = Color.yellow;

    private PlayerHealth _health;
    private PlayerMovementController _movement;

    private Color[] _cachedColors;
    private Coroutine _timer;
    private bool _isActive = false;

    // Static type: declared as base class (PowerStatsBase)
    // Dynamic type: assigned as subclass (PowerStatsEnhanced) in Start()
    private PowerStatsBase _powerStats;

    /// Initializes component references
    void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _movement = GetComponent<PlayerMovementController>();

        Debug.Assert(_health != null, "PlayerHealth missing!");
        Debug.Assert(_movement != null, "PlayerMovementController missing!");
    }

    /// Sets up the power stats using dynamic binding
    void Start()
    {
        // Dynamic type: instance of subclass assigned to superclass variable
        _powerStats = new PowerStatsEnhanced();
    }

    /// Handles input for power activation
    void Update()
    {
        // Press P to activate power-up
        if (Input.GetKeyDown(KeyCode.P)) {
            ActivatePower();
        }
    }

    /// Activates the power, applying boosts and starting the timer
    public void ActivatePower(float? overrideDurationSeconds = null)
    {
        if (_isActive) {
            return;
        }

        _isActive = true;

        CacheTints();
        ApplyTint(true);

        // Call methods through the static type (PowerStatsBase)
        int bonusHealth = _powerStats.GetBonusHealth();
        float speedMult = _powerStats.GetSpeedMultiplier();
        float duration = overrideDurationSeconds ?? _powerStats.GetDuration();

        _health.SetMaxHealthPreserveCurrent(_health.MaxHealth + bonusHealth);
        _movement.SetSpeedMultiplier(_movement.CurrentSpeedMultiplier * speedMult);

        Debug.Log($"Power activated - Health: +{bonusHealth}, Speed: x{speedMult}, Duration: {duration}s");

        _timer = StartCoroutine(Timer(duration));
    }

    /// Deactivates the power and restores stats
    public void DeactivatePower()
    {
        if (!_isActive) {
            return;
        }

        _isActive = false;
        ApplyTint(false);

        // Use the same stats object to restore values
        int bonusHealth = _powerStats.GetBonusHealth();

        _health.SetMaxHealthPreserveCurrent(_health.MaxHealth - bonusHealth);
        _movement.ResetSpeedMultiplier();

        Debug.Log("Power deactivated.");

        if (_timer != null) {
            StopCoroutine(_timer);
        }
    }

    /// Coroutine that handles power duration timing
    private IEnumerator Timer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DeactivatePower();
    }

    /// Stores original sprite colors before tinting
    private void CacheTints()
    {
        if (_tintTargets == null) return;

        _cachedColors = new Color[_tintTargets.Length];
        for (int i = 0; i < _tintTargets.Length; i++) {
            _cachedColors[i] = _tintTargets[i].color;
        }
    }

    /// Applies or removes tint from target sprites
    private void ApplyTint(bool active)
    {
        if (_tintTargets == null) return;

        for (int i = 0; i < _tintTargets.Length; i++) {
            _tintTargets[i].color = active ? _activeTintColor : _cachedColors[i];
        }
    }
}

// ---------- SUPER CLASS ----------
// Base class provides default power-up stats.
// STATIC BINDING: These values are used (Health: 100, Speed: 5, Duration: 60)
public class PowerStatsBase
{
    // "virtual" enables DYNAMIC BINDING
    // Remove "virtual" for STATIC BINDING
    public virtual int GetBonusHealth()
    {
        return 100; // Base version: higher health bonus
    }

    public virtual float GetSpeedMultiplier()
    {
        return 5f; // Base version: higher speed multiplier
    }

    public virtual float GetDuration()
    {
        return 60f; // Base version: longer duration
    }
}

// ---------- SUBCLASS ----------
// Subclass provides enhanced power-up stats.
// DYNAMIC BINDING: These values are used (Health: 50, Speed: 3, Duration: 10)
public class PowerStatsEnhanced : PowerStatsBase
{
    // "override" enables DYNAMIC BINDING
    // Replace "override" with "new" for STATIC BINDING
    public override int GetBonusHealth()
    {
        return 50;
    }

    public override float GetSpeedMultiplier()
    {
        return 3f;
    }

    public override float GetDuration()
    {
        return 10f; 
    }
}