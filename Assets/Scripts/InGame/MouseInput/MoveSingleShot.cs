using System;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using InGame.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    [Serializable]
    public class MoveSingleShot : BaseMoveShot
    {
        public MoveSingleShot()
        {
            
        }
        
        public MoveSingleShot(Camera cam, MonoCursor cursor) : base(cam, cursor)
        {

        }
        
        public override void OnMouseClick()
        {
            if (!CanShoot) return;
            
            // Check hit enemy, only nearest enemy is hit
            var mousePos = Cam.ScreenToWorldPoint(mousePosition);
            var hits = Physics2D.CircleCastAll(mousePos, WorldUtility.GetWorldSize(Cam, cursor).x, Vector2.zero, 0f, LayerMask.GetMask("Entity"));
            var minDistance = float.MaxValue;
            EnemyEntity nearestEnemy = null;
            foreach (var hit in hits)
            {
                if (hit.collider&& hit.transform.TryGetComponent<EnemyEntity>(out var enemyEntity))
                {
                    var distance = Vector2.Distance(Cam.WorldToScreenPoint(enemyEntity.transform.position),
                        mousePosition);
                    if (distance >= minDistance) continue;
                    minDistance = distance;
                    nearestEnemy = enemyEntity;
                }
            }
            var damage = CalculateDmg();
            if (nearestEnemy)
            {
                nearestEnemy.OnHit(damage);
                CheckElemental(nearestEnemy);
            }

            base.OnMouseClick();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            // Cooldown if player can not shoot
            if (!CanShoot)
            {
                cdCounter -= Time.deltaTime;
                if (cdCounter <= 0)
                    CanShoot = true;
                
                // Update UI
                if (uiCursorCd)
                    uiCursorCd.fillAmount = Mathf.Clamp(cdCounter / Cooldown, 0f, 1f);
            }
        }

        public override void OnDrawGizmos()
        {
            DrawBoxGizmo(Cam.ScreenToWorldPoint(mousePosition), WorldUtility.GetWorldSize(Cam, cursor), 0f, Color.cyan);
        }
        
        private void DrawBoxGizmo(Vector2 center, Vector2 size, float angle, Color color)
        {
            var half = size / 2f;
            
            var corners = new Vector2[4]
            {
                center + new Vector2(-half.x, -half.y),
                center + new Vector2(-half.x,  half.y),
                center + new Vector2( half.x,  half.y),
                center + new Vector2( half.x, -half.y)
            };

            Debug.DrawLine(corners[0], corners[1], color, 1f);
            Debug.DrawLine(corners[1], corners[2], color, 1f);
            Debug.DrawLine(corners[2], corners[3], color, 1f);
            Debug.DrawLine(corners[3], corners[0], color, 1f);
        }
    }
}