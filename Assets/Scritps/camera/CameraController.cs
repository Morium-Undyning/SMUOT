using System;
using UnityEngine;

namespace camera
{
    public class CameraController : MonoBehaviour
    {
        public float followSpeed = 5f;
        public float maxOffset = 5f;

        public float zoomSpeed = 10f;
        public float minHeight = 5f;
        public float maxHeight = 20f;
        public float smoothTime = 5f;

        private Camera cam;
        private float targetHeight;
        private Vector3 startPosition;
        private float startHeight;

        void Start()
        {
            startPosition = transform.position;
            startHeight = transform.position.y;
            targetHeight = startHeight;
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            HandleZoom();
            HandleFollow();
        }

        void HandleFollow()
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            Vector3 mouseViewportPos = cam.ScreenToViewportPoint(mouseScreenPos);

            Vector3 offsetFromCenter = new Vector3(
                mouseViewportPos.x - 0.5f,
                0f,
                mouseViewportPos.y - 0.5f
            );

            offsetFromCenter *= followSpeed;

            offsetFromCenter.x = Mathf.Clamp(offsetFromCenter.x, -maxOffset, maxOffset);
            offsetFromCenter.z = Mathf.Clamp(offsetFromCenter.z, -maxOffset, maxOffset);

            Vector3 targetPosition = startPosition + new Vector3(offsetFromCenter.x, 0f, offsetFromCenter.z);

            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(targetPosition.x, transform.position.y, targetPosition.z),
                smoothTime * Time.deltaTime
            );
        }

        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                targetHeight -= scroll * zoomSpeed;
                targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
            }

            float newY = Mathf.Lerp(transform.position.y, targetHeight, smoothTime * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}