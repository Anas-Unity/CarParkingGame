/*using _Game.UI;
using UnityEngine;

namespace _Game.Inputs
{
    /// <summary>
    /// Handles player input for both keyboard and touch controls and updates the PlayerInput ScriptableObject.
    /// Ensures that the input system is modular and easy to extend.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Player Input Configuration")]
        [Tooltip("ScriptableObject to store the player's input states.")]
        [SerializeField] private PlayerInputSO playerInput;

        [Header("Touch Controls Setup")]
        [Tooltip("Indicates if touch controls are configured.")]
        [SerializeField] private bool _touchControlsSetup = false;

        [Tooltip("Button used for accelerating.")]
        [SerializeField] private CustomButton _throttleButton;

        [Tooltip("Button used for reversing.")]
        [SerializeField] private CustomButton _reverseButton;

        [Tooltip("Button used for turning left.")]
        [SerializeField] private CustomButton _turnLeftButton;

        [Tooltip("Button used for turning right.")]
        [SerializeField] private CustomButton _turnRightButton;

        [Tooltip("Button used for handbraking.")]
        [SerializeField] private CustomButton _handbrakeButton;

        private void Start()
        {
            _touchControlsSetup = AreTouchControlsConfigured();
        }

        private void Update()
        {
            UpdateInputStates();
        }

        /// <summary>
        /// Verifies if all touch controls are properly configured.
        /// </summary>
        /// <returns>True if all touch controls are assigned; otherwise, false.</returns>
        private bool AreTouchControlsConfigured()
        {
            return _throttleButton != null &&
                   _reverseButton != null &&
                   _turnLeftButton != null &&
                   _turnRightButton != null &&
                   _handbrakeButton != null;
        }

        /// <summary>
        /// Updates the player's input states by checking both keyboard and touch inputs.
        /// </summary>
        private void UpdateInputStates()
        {
            playerInput.IsAccelerating = GetInput(KeyCode.W, _throttleButton);
            playerInput.IsReversing = GetInput(KeyCode.S, _reverseButton);
            playerInput.IsTurningLeft = GetInput(KeyCode.A, _turnLeftButton);
            playerInput.IsTurningRight = GetInput(KeyCode.D, _turnRightButton);
            playerInput.IsHandbraking = GetInput(KeyCode.Space, _handbrakeButton);
        }

        /// <summary>
        /// Checks if a specific keyboard key is pressed or if a touch control button is being pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <param name="button">The corresponding touch control button.</param>
        /// <returns>True if the key or button is pressed; otherwise, false.</returns>
        private bool GetInput(KeyCode key, CustomButton button)
        {
            return Input.GetKey(key) || (_touchControlsSetup && button != null && button.ButtonPressed);
        }
    }
}*/

using _Game.UI;
using UnityEngine;

namespace _Game.Inputs
{
    /// <summary>
    /// Handles player input for both keyboard and touch controls and updates the PlayerInput ScriptableObject.
    /// Keyboard controls are now customizable in the Inspector.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Player Input Configuration")]
        [Tooltip("ScriptableObject to store the player's input states.")]
        [SerializeField] private PlayerInputSO playerInput;

        // --- NEW: Editable KeyCode fields for keyboard controls ---
        [Header("Keyboard Controls")]
        [Tooltip("The key used to accelerate the car.")]
        [SerializeField] private KeyCode _accelerateKey = KeyCode.UpArrow;
        [Tooltip("The key used to reverse the car.")]
        [SerializeField] private KeyCode _reverseKey = KeyCode.DownArrow;
        [Tooltip("The key used to turn the car left.")]
        [SerializeField] private KeyCode _turnLeftKey = KeyCode.LeftArrow;
        [Tooltip("The key used to turn the car right.")]
        [SerializeField] private KeyCode _turnRightKey = KeyCode.RightArrow;
        [Tooltip("The key used for the handbrake.")]
        [SerializeField] private KeyCode _handbrakeKey = KeyCode.Space;

        [Header("Touch Controls Setup")]
        [Tooltip("Indicates if touch controls are configured.")]
        [SerializeField] private bool _touchControlsSetup = false;
        [SerializeField] private CustomButton _throttleButton;
        [SerializeField] private CustomButton _reverseButton;
        [SerializeField] private CustomButton _turnLeftButton;
        [SerializeField] private CustomButton _turnRightButton;
        [SerializeField] private CustomButton _handbrakeButton;

        private void Start()
        {
            _touchControlsSetup = AreTouchControlsConfigured();
        }

        private void Update()
        {
            UpdateInputStates();
        }

        private bool AreTouchControlsConfigured()
        {
            return _throttleButton != null &&
                   _reverseButton != null &&
                   _turnLeftButton != null &&
                   _turnRightButton != null &&
                   _handbrakeButton != null;
        }

        /// <summary>
        /// --- MODIFIED: Updates input states using the customizable KeyCode variables ---
        /// </summary>
        private void UpdateInputStates()
        {
            playerInput.IsAccelerating = GetInput(_accelerateKey, _throttleButton);
            playerInput.IsReversing = GetInput(_reverseKey, _reverseButton);
            playerInput.IsTurningLeft = GetInput(_turnLeftKey, _turnLeftButton);
            playerInput.IsTurningRight = GetInput(_turnRightKey, _turnRightButton);
            playerInput.IsHandbraking = GetInput(_handbrakeKey, _handbrakeButton);
        }

        private bool GetInput(KeyCode key, CustomButton button)
        {
            return Input.GetKey(key) || (_touchControlsSetup && button != null && button.ButtonPressed);
        }
    }
}
