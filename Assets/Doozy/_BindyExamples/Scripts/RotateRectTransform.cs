using UnityEngine;
// ReSharper disable CheckNamespace

namespace Doozy.Sandbox.Bindy
{
    [RequireComponent(typeof(RectTransform))]
    public class RotateRectTransform : MonoBehaviour
    {
        /// <summary>
        /// Rotation speed in degrees per second
        /// </summary>
        public float Speed = 90f;

        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (Speed == 0f) return; //no need to rotate if the speed is 0
            m_RectTransform.Rotate(0f, 0f, Speed * Time.deltaTime);
        }
    }
}
