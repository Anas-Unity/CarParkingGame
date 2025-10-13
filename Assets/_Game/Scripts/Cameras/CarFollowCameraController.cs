/*using UnityEngine;
using DG.Tweening; // DOTween for smooth transitions

namespace _Game.Cameras
{
    /// <summary>
    /// CarFollowCameraController manages the camera to smoothly follow and look at the target car.
    /// It uses customizable speeds for both the follow and look behaviors.
    /// </summary>
    public class CarFollowCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [Tooltip("Speed at which the camera follows the car.")]
        [Range(1f, 10f)]
        [SerializeField] private float _followSpeed = 2f;

        [Tooltip("Speed at which the camera looks at the car.")]
        [Range(1f, 10f)]
        [SerializeField] private float _lookSpeed = 5f;

        private Transform _carTransform;
        private Vector3 _initialCameraOffset;
        private Vector3 _initialCameraPosition;

        private void Awake()
        {
            // Initialize camera position
            _initialCameraPosition = transform.position;

            ServiceLocator.Register(this);
        }

        private void FixedUpdate()
        {
            if (_carTransform != null)
            {
                FollowCar();
                LookAtCar();
            }
        }

        /// <summary>
        /// Sets the car transform and initializes camera offset.
        /// </summary>
        /// <param name="carTransform">The transform of the car to follow.</param>
        public void SetCarTransform(Transform carTransform)
        {
            _carTransform = carTransform;
            if (_carTransform != null)
            {
                _initialCameraOffset = _initialCameraPosition - _carTransform.position;
                //Debug.Log("CarTransform set and initial camera offset calculated.");
            }
            else
            {
                Debug.LogError("Failed to set CarTransform: Transform is null.");
            }
        }

        /// <summary>
        /// Smoothly follows the car based on the initial camera offset.
        /// Uses Lerp for smooth transitions.
        /// </summary>
        private void FollowCar()
        {
            Vector3 targetPosition = _carTransform.position + _initialCameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smoothly rotates the camera to look at the car.
        /// Uses Lerp for smooth rotation.
        /// </summary>
        private void LookAtCar()
        {
            Vector3 directionToLook = _carTransform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lookSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Optionally uses DOTween for smoother camera transitions.
        /// This function could be called when you want a more cinematic camera movement.
        /// </summary>
        public void SmoothFollowWithDOTween()
        {
            if (_carTransform != null)
            {
                Vector3 targetPosition = _carTransform.position + _initialCameraOffset;
                transform.DOMove(targetPosition, 1f / _followSpeed).SetEase(Ease.InOutQuad);
            }
        }

        public void SmoothLookAtWithDOTween()
        {
            if (_carTransform != null)
            {
                Vector3 directionToLook = _carTransform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
                transform.DORotateQuaternion(targetRotation, 1f / _lookSpeed).SetEase(Ease.InOutQuad);
            }
        }
    }
}*/

using UnityEngine;
using DG.Tweening; // DOTween for smooth transitions

namespace _Game.Cameras
{
    /// <summary>
    /// Manages the camera to smoothly follow and look at the target car using a single, easily adjustable offset.
    /// </summary>
    public class CarFollowCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [Tooltip("Speed at which the camera follows the car.")]
        [Range(1f, 20f)]
        [SerializeField] private float _followSpeed = 8f;

        [Tooltip("Speed at which the camera looks at the car.")]
        [Range(1f, 20f)]
        [SerializeField] private float _lookSpeed = 15f;

        // --- NEW: A single Vector3 to control the camera's position relative to the car ---
        [Header("Camera Angle")]
        [Tooltip("Adjust this in the Inspector to find the best camera angle. Z should be negative to be behind the car.")]
        [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 3, -8);

        private Transform _carTransform;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void FixedUpdate()
        {
            if (_carTransform != null)
            {
                FollowCar();
                LookAtCar();
            }
        }

        /// <summary>
        /// Sets the car transform.
        /// </summary>
        public void SetCarTransform(Transform carTransform)
        {
            _carTransform = carTransform;
        }

        /// <summary>
        /// --- MODIFIED: Smoothly follows the car using the single camera offset ---
        /// </summary>
        private void FollowCar()
        {
            // The offset now rotates with the car for a more dynamic feel
            Vector3 targetPosition = _carTransform.position + (_carTransform.rotation * _cameraOffset);

            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Smoothly rotates the camera to look at the car.
        /// </summary>
        private void LookAtCar()
        {
            Vector3 directionToLook = _carTransform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lookSpeed * Time.deltaTime);
        }
    }
}
