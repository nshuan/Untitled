using System.Collections.Generic;
using UnityEngine;

namespace Boids.Behaviours
{
    [CreateAssetMenu(menuName = "Boids/Behaviour/Stay In Radius")]
    public class StayInRadiusBehaviour : FlockBehaviour
    {
        public Vector2 center;
        public float radius = 15f;
        
        public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
        {
            var centerOffset = center - (Vector2)agent.transform.position;
            var t = centerOffset.magnitude / radius;
            if (t < 0.9f) return Vector2.zero;

            return centerOffset * (t * t);
        }
    }
}