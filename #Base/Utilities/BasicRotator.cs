namespace SDE
{
    using UnityEngine;
    public class BasicRotator : MonoBehaviour
    {
        public Vector3 AxisSpeed;

        private void LateUpdate()
        {
            transform.Rotate(AxisSpeed * Time.deltaTime);
        }
    }
}