//Team Lead 1 - Aiden Weaver
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthPickup2D : MonoBehaviour
{
    public int healAmount = 25;
    public string playerTag = "Player";
    public AudioClip pickupSound;
    public ParticleSystem pickupParticles;
    public int minPlayerHealth = 0;

    private Collider2D _collider;
    private SpriteRenderer[] _renderers;
    private AudioSource _audioSource;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        if (_collider != null && !_collider.isTrigger)
            _collider.isTrigger = true;

        _renderers = GetComponentsInChildren<SpriteRenderer>(true);

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null && pickupSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) return;

        if (ph.CurrentHealth < minPlayerHealth) return;

        int healed = ph.Heal(healAmount);
        if (healed <= 0) return;

        if (pickupSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(pickupSound);
        }

        
            float delay = (pickupSound != null && _audioSource != null) ? pickupSound.length : 0f;
            Destroy(gameObject, delay);
        
    }

}
