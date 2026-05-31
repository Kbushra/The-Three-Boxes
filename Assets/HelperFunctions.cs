using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace HelperFunctions
{
    public static class Collisions
    {
        public static bool MoveFree(in Rigidbody rigidbodyComponent, in Vector3 move, out RaycastHit info, float padding = 0.1f)
        {
            info = new RaycastHit();
            bool success = !rigidbodyComponent.SweepTest(move.normalized, out info, move.magnitude, QueryTriggerInteraction.Ignore);

            info.distance = Math.Clamp(info.distance - padding, 0, move.magnitude);
            return success;
        }

        public static bool CapsuleFree(in CapsuleCollider capsuleComponent, in Vector3 move, out RaycastHit info, float padding = 0.1f)
        {
            info = new RaycastHit();
            float scale = capsuleComponent.transform.lossyScale.y;
            float height = capsuleComponent.height * scale;
            float radius = capsuleComponent.radius * scale;
            Vector3 center = capsuleComponent.transform.TransformPoint(capsuleComponent.center);
            Vector3 centerToPoint = capsuleComponent.transform.up * (height/2 - radius);
            bool success = !Physics.CapsuleCast(center + centerToPoint, center - centerToPoint, radius, move.normalized, out info,
                move.magnitude, LayerMask.GetMask("Solids"), QueryTriggerInteraction.Ignore);
            
            info.distance = Math.Clamp(info.distance - padding, 0, move.magnitude);
            return success;
        }

        public static bool BoxFree(in BoxCollider boxComponent, in Vector3 move, out RaycastHit info, float padding = 0.1f)
        {
            info = new RaycastHit();
            Vector3 center = boxComponent.transform.TransformPoint(boxComponent.center);
            Vector3 halfExtents = Vector3.Scale(boxComponent.size * 0.5f, boxComponent.transform.lossyScale);
            bool success = !Physics.BoxCast(center, halfExtents, move.normalized, out info, boxComponent.transform.rotation,
                move.magnitude, LayerMask.GetMask("Solids"), QueryTriggerInteraction.Ignore);
            
            info.distance = Math.Clamp(info.distance - padding, 0, move.magnitude);
            return success;
        }
    }

    public static class Maths
    {
        public static float LerpDelta(float a, float b, float t)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Pow(1 - t, Time.deltaTime));
        }

        public static float RoundNearest(float n, float toNearest)
        {
            return Mathf.Round(n / toNearest) * toNearest;
        }

        public static Vector3 RepeatNum(float n)
        {
            return new Vector3(n, n, n);
        }
    }
}