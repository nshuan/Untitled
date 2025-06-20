using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace InGame
{
    public class EnemyEntity : MonoBehaviour
    {
        public Transform Target { get; set; }
        public EnemyBehaviour config;
        private float MaxHealth { get; set; }
        private float CurrentHealth { get; set; }
        public bool IsDead => CurrentHealth <= 0;
        public Action OnDead { get; set; }
        public EnemyType Type { get; set; }
        public EnemyState State { get; set; }
        
        // Elemental effect
        public bool IsInLightning { get; set; }
        
        [Space, Header("Visual")] 
        [SerializeField] private Transform uiHealth;
        
        private bool inAttackRange;
        private Coroutine attackCoroutine;

        private Vector2 attackPosition;
        
        #region Initialize

        public void Init(Transform target, EnemyType type, float hpMultiplier)
        {
            // Set target and attack position
            Target = target;
            Type = type;
            attackPosition = (Vector2)((Quaternion.Euler(0f, 0f, Random.Range(-75f, 75f)) *
                                      (Vector2)(transform.position - target.position).normalized).normalized * (0.9f * config.attackRange)
                            + target.position);
            
            MaxHealth = config.hp * hpMultiplier;
            CurrentHealth = MaxHealth;
            State = EnemyState.Spawn;
            config.Init(transform);
            
            // Update health ui
            UIUpdateHealth();
        }

        #endregion

        #region Core function

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
        
        public void Activate()
        {
            config.Spawn(transform, () =>
            {
                StartAttackCoroutine();
                State = EnemyState.Move;
            });
        }

        #endregion

        private void Update()
        {
            if (!Target) return;
            if (State != EnemyState.Move) return;
            MoveTo(Target);
        }

        private void MoveTo(Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) < config.attackRange)
                inAttackRange = true;
            else
                config.moveBehaviour.Move(transform, attackPosition, config.attackRange, config.moveSpeed);
        }

        private void StartAttackCoroutine()
        {
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(IEAttack());
        }

        private IEnumerator IEAttack()
        {
            while (true)
            {
                if (inAttackRange)
                {
                    Attack();
                    yield return new WaitForSeconds(1 / config.attackSpeed);
                }
                else
                    yield return new WaitUntil(() => inAttackRange);
            }
        }

        private void Attack()
        {
            Debug.Log($"Enemy {name} Attack!!!");
        }

        public void OnHit(float damage)
        {
            if (IsDead) return;
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                OnDie();
            }
            else
            {
                UIUpdateHealth();
            }
        }
        
        private void OnDie()
        {
            IsInLightning = false;
            OnDead?.Invoke();
            OnDead = null;
            EnemyPool.Instance.Release(Type, this);
        }
        
        private void UIUpdateHealth()
        {
            DOTween.Complete(this);
            var seq = DOTween.Sequence(this);
            seq.Append(transform.DOPunchScale(0.5f * Vector3.one, 0.2f))
                .Join(uiHealth.DOScale(Mathf.Clamp(CurrentHealth, 0f, MaxHealth) / MaxHealth, 0.2f));
            seq.Play();
        }
    }
}