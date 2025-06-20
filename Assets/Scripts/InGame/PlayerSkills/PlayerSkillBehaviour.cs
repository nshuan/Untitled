using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public abstract class PlayerSkillBehaviour : ScriptableObject
    {
        public ShotCursorType cursorType;
        public ProjectileEntity projectilePrefab;

        public abstract void Shoot(Vector2 spawnPos, Vector2 target);
    }
}