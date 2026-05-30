using UnityEngine;

namespace HelperFunctions
{
    public static class Collision
    {
        public static bool MoveFree(in Rigidbody rigidbody, in Vector3 move, out RaycastHit info)
        {
            return !rigidbody.SweepTest(move.normalized, out info, move.magnitude, QueryTriggerInteraction.Ignore);
        }
    }
}