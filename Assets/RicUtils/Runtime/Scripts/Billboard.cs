using UnityEngine;

namespace RicUtils
{
    public class Billboard : MonoBehaviour
    {
        public Transform lookAtTarget;
        public bool inverseLookAt;

        private static Camera activeCamera;

        private void Update()
        {
            if (activeCamera == null) activeCamera = FindObjectOfType<Camera>();

            Transform target = lookAtTarget;
            if (target == null && activeCamera != null) target = activeCamera.transform;

            if (target == null) return;

            var position = target.position;

            if (inverseLookAt)
            {
                position = transform.position * 2 - target.position;
            }

            transform.LookAt(position);
        }
    }
}
