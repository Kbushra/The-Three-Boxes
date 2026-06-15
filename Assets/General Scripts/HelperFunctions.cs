using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace HelperFunctions
{
    public static class Collisions
    {
        public static bool MoveFree(in Rigidbody rigidbodyComponent, in Vector3 move, out RaycastHit info, float padding = 0.1f)
        {
            info = new RaycastHit(); info.distance = 0;
            if (!rigidbodyComponent.gameObject.activeSelf) { return true; }

            bool success = !rigidbodyComponent.SweepTest(move.normalized, out info, move.magnitude, QueryTriggerInteraction.Ignore);

            info.distance = Math.Clamp(info.distance - padding, 0, move.magnitude);
            return success;
        }

        public static bool CapsuleFree(in CapsuleCollider capsuleComponent, in Vector3 move, out RaycastHit info, float padding = 0.1f, string layerName = "Solids")
        {
            info = new RaycastHit(); info.distance = 0;
            if (!capsuleComponent.enabled) { return true; }

            float scale = capsuleComponent.transform.lossyScale.y;
            float height = capsuleComponent.height * scale;
            float radius = capsuleComponent.radius * scale;
            Vector3 center = capsuleComponent.transform.TransformPoint(capsuleComponent.center);
            Vector3 centerToPoint = capsuleComponent.transform.up * (height/2 - radius);
            bool success = !Physics.CapsuleCast(center + centerToPoint, center - centerToPoint, radius, move.normalized, out info,
                move.magnitude, LayerMask.GetMask(layerName), QueryTriggerInteraction.Ignore);
            
            info.distance = Math.Clamp(info.distance - padding, 0, move.magnitude);
            return success;
        }

        public static bool BoxFree(in BoxCollider boxComponent, in Vector3 move, out RaycastHit info, float padding = 0.1f, string layerName = "Solids")
        {
            info = new RaycastHit(); info.distance = 0;
            if (!boxComponent.enabled) { return true; }

            Vector3 center = boxComponent.transform.TransformPoint(boxComponent.center);
            Vector3 halfExtents = Vector3.Scale(boxComponent.size * 0.5f, boxComponent.transform.lossyScale);
            bool success = !Physics.BoxCast(center, halfExtents, move.normalized, out info, boxComponent.transform.rotation,
                move.magnitude, LayerMask.GetMask(layerName), QueryTriggerInteraction.Ignore);
            
            info.distance = Math.Clamp(info.distance - padding, 0, move.magnitude);
            return success;
        }

        public static Collider[] BoxCollisions(in BoxCollider boxComponent)
        {
            if (!boxComponent.enabled) { return new Collider[0]; }
            
            Vector3 center = boxComponent.transform.TransformPoint(boxComponent.center);
            Vector3 halfExtents = Vector3.Scale(boxComponent.size * 0.5f, boxComponent.transform.lossyScale);
            return Physics.OverlapBox(center, halfExtents, boxComponent.transform.rotation);
        }
    }

    public static class Maths
    {
        public static bool NearEquals(float a, float b, float precision = 0.1f)
        {
            return Mathf.Abs(a - b) <= precision;
        }

        public static float LerpDelta(float a, float b, float t)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Pow(1 - t, Time.deltaTime));
        }

        public static float LerpAngleDelta(float a, float b, float t)
        {
            return Mathf.LerpAngle(a, b, 1 - Mathf.Pow(1 - t, Time.deltaTime));
        }

        public static float RoundNearest(float n, float toNearest)
        {
            return Mathf.Round(n / toNearest) * toNearest;
        }

        public static Vector3 DivideVectors(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3 RepeatNum(float n)
        {
            return new Vector3(n, n, n);
        }

        public static Vector3 MeshSize(Transform transformComponent)
        {
            return transformComponent.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        }

        public static Vector3 BoundsSize(Transform transformComponent)
        {
            return transformComponent.GetComponent<Renderer>().bounds.size;
        }

        //Origin between (-1, -1, -1) and (1, 1, 1)
        public static Vector3 OriginWorldPosition(Transform transformComponent, Vector3 origin)
        {
            Bounds bounds = transformComponent.GetComponent<Renderer>().bounds;
            return bounds.center + transformComponent.rotation * Vector3.Scale(origin, bounds.size / 2);
        }

        public static void OriginScale(Transform transformComponent, Vector3 scale, Vector3 origin)
        {
            origin = OriginWorldPosition(transformComponent, origin);
            
            Vector3 prevScale = transformComponent.localScale;
            transformComponent.localScale = scale;

            Vector3 toOrigin = transformComponent.position - origin;
            transformComponent.position = origin + Vector3.Scale(toOrigin, DivideVectors(scale, prevScale));
        }
    }
}