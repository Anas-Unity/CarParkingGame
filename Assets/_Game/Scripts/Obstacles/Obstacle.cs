/*using _Game._Abstracts;
using _Game._helpers;
using _Game._Interfaces;
using DG.Tweening;
using System;
using UnityEngine;

namespace _Game.Obstacles
{
    /// <summary>
    /// Represents an obstacle that can deal damage to other damageable objects upon collision.
    /// Inherits from AbstractDamageableDamagerBase to handle both damage and health management functionalities.
    /// </summary>
    public class Obstacle : AbstractDamageableDamagerBase
    {
        [Header("Damage Settings")]
        [Tooltip("The amount of damage this obstacle inflicts upon collision.")]
        [SerializeField, Range(0f, 100f)]
        private int _damageAmount = 1;

        [Header("Parking Obstacle Params")]
        [SerializeField]
        private Material HitMaterial;
        [SerializeField]
        private Material NormalMaterial;
        [SerializeField]
        private float _hitMaterialChangeInterval = 0.1f;
        [SerializeField]
        private float _damageableDestroyDelay = 1f;

        private Renderer _objectRenderer;

        public float _disposeTime = 0.5f;
        public float _initTime = 0.5f;

        void Awake()
        {
            _objectRenderer = GetComponentInChildren<Renderer>();

            NormalMaterial = _objectRenderer.material;
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);
            ApplyHitEffect();

        }

        void ApplyHitEffect()
        {
            _objectRenderer.material = HitMaterial;

            int time = Convert.ToInt32(_damageableDestroyDelay / (_hitMaterialChangeInterval * 3));

            Sequence hitSequence = DOTween.Sequence();
            for (int i = 0; i < time; i++)
            {
                hitSequence.AppendInterval(_hitMaterialChangeInterval);
                hitSequence.AppendCallback(() => _objectRenderer.material = NormalMaterial);
                hitSequence.AppendInterval(_hitMaterialChangeInterval);
                hitSequence.AppendCallback(() => _objectRenderer.material = HitMaterial);
            }
            hitSequence.AppendInterval(_hitMaterialChangeInterval);
            hitSequence.AppendCallback(() =>
            {
                Dispose();
            });
            hitSequence.Play();
        }

        public void Dispose()
        {
            transform.DOScale(Vector3.zero, _disposeTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public void Init()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _initTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collided object implements IDamageable
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                // Deal damage to the damageable object
                DealDamage(damageable, _damageAmount);
                TakeDamage(_health);
            }
        }
    }
}
*/

using _Game._Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _Game.Obstacles
{
    /// <summary>
    /// Represents an indestructible obstacle that deals damage and shows a visual hit effect.
    /// </summary>
    public class Obstacle : MonoBehaviour // No longer needs to be damageable itself
    {
        [Header("Damage Settings")]
        [Tooltip("The amount of damage this obstacle inflicts upon collision.")]
        [SerializeField, Range(0, 100)]
        private int _damageAmount = 1;

        [Header("Visual Effect Settings")]
        [Tooltip("The material to show when the obstacle is hit.")]
        [SerializeField]
        private Material HitMaterial;
        [Tooltip("How long the hit material should be visible (in seconds).")]
        [SerializeField]
        private float _hitEffectDuration = 0.5f;

        private Renderer _objectRenderer;
        private Material _originalMaterial;
        private bool _isHit = false; // Prevents multiple hit effects at once

        void Awake()
        {
            _objectRenderer = GetComponentInChildren<Renderer>();
            if (_objectRenderer != null)
            {
                // Store the original material
                _originalMaterial = _objectRenderer.material;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if we've already been hit recently to avoid spamming effects
            if (_isHit) return;

            // Check if the collided object can take damage (the car)
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                // 1. Deal damage to the car
                damageable.TakeDamage(_damageAmount);

                // 2. Play the visual hit effect on this obstacle
                ApplyHitEffect();
            }
        }

        void ApplyHitEffect()
        {
            _isHit = true;
            _objectRenderer.material = HitMaterial;

            // Use DOTween to create a delayed call to revert the material
            DOVirtual.DelayedCall(_hitEffectDuration, () =>
            {
                _objectRenderer.material = _originalMaterial;
                _isHit = false; // Allow the effect to be triggered again
            });
        }
    }
}