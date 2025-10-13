using _Game._helpers.Audios;
using _Game.Car;
using _Game.Management;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Game.UI
{
    /// <summary>
    /// Manages the UI elements including health, speed display, result screens, and their associated animations.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Result Screen Elements")]
        [Tooltip("Result screen that appears after level completion or failure.")]
        [SerializeField] private GameObject _resultScreen;
        [Tooltip("Text field to display the result message.")]
        [SerializeField] private TextMeshProUGUI _resultText;
        [Tooltip("Menu button.")]
        [SerializeField] private CustomButton _menuButton;
        [Tooltip("Restart button.")]
        [SerializeField] private CustomButton _restartButton;

        [Header("Control Buttons Elements")]
        [Tooltip("Throttle Button UI element.")]
        [SerializeField] private GameObject _throttleButton;
        [Tooltip("Reverse Button UI element.")]
        [SerializeField] private GameObject _reverseButton;
        [Tooltip("Turn Left Button UI element.")]
        [SerializeField] private GameObject _turnLeftButton;
        [Tooltip("Turn Right Button UI element.")]
        [SerializeField] private GameObject _turnRightButton;
        [Tooltip("Handbrake Button UI element.")]
        [SerializeField] private GameObject _handbrakeButton;

        [Header("Game UI Elements")]
        [Tooltip("Text field to display the current level index.")]
        [SerializeField] private TextMeshProUGUI _levelIndexText;


        [Tooltip("Text field to display the level timer.")]
        [SerializeField] private TextMeshProUGUI _timerText;

        [Tooltip("Car health displayer UI element.")]
        [SerializeField] private GameObject _carHealthDisplayer;
        [Tooltip("Control buttons UI element.")]
        [SerializeField] private GameObject _controlButtons;
        [Tooltip("Speed displayer UI element.")]
        [SerializeField] private GameObject _speedDisplayer;

        [Header("Messages")]
        [Tooltip("Message to display when the level is completed.")]
        [SerializeField] private string _levelCompleteMessage = "Level Complete!";
        [Tooltip("Message to display when the level fails.")]
        [SerializeField] private string _levelFailMessage = "Level Failed";

        [Header("DOTween Animation Settings")]
        [Tooltip("Duration for element animations.")]
        [SerializeField, Range(0.1f, 2f)] private float _elementAnimationDuration = 0.5f;
        [SerializeField, Range(0.1f, 2f)] private float _elementAnimationInterval = 0.1f;
        [Tooltip("Ease type for opening UI elements.")]
        [SerializeField] private Ease _openElementAnimationEase = Ease.OutBack;
        [Tooltip("Ease type for closing UI elements.")]
        [SerializeField] private Ease _closeElementAnimationEase = Ease.InBack;

        [Header("Result Screen Delay")]
        [Tooltip("Delay before showing the result screen.")]
        [SerializeField, Range(0.1f, 5f)] private float _resultScreenDelay = 1f;

        [Header("Speed Display")]
        [Tooltip("Text component to display the vehicle's speed.")]
        [SerializeField] private TextMeshProUGUI _speedText;

        [Header("Coin Display")]
        [Tooltip("Text component to display the coin's earned on level.")]
        [SerializeField] private TextMeshProUGUI _coinCountText;
        [SerializeField] private GameObject[] _stars;

        [Header("Health Display")]
        [Tooltip("Prefab for health segments.")]
        [SerializeField] private GameObject _healthSegmentPrefab;
        [Tooltip("Container for health segments.")]
        [SerializeField] private Transform _healthContainer;

        [Header("Effects")]
        [Header("Audio Effects")]
        [Tooltip("The sound play when spawn an obstacle.")]
        [SerializeField] private string _uiSoundKey = "ui";

        private List<GameObject> _healthSegments = new List<GameObject>();
        private float _currentSpeed;
        private float _speedVelocity = 0.0f;

        private CarDamageHandler _carDamageHandler;
        private LevelManager _levelManager;
        private AudioManager _audioManager;

        private void Awake()
        {
            ServiceLocator.Register(this);
            StartCoroutine(InitializeDependencies());
        }

        private void Start()
        {
            _audioManager = ServiceLocator.Get<AudioManager>();

            InitializeUI();
            SetButtonEvents();
        }

        /// <summary>
        /// Coroutine that waits for dependencies to be registered before proceeding.
        /// </summary>
        private IEnumerator InitializeDependencies()
        {
            while (_carDamageHandler == null || _levelManager == null)
            {
                _carDamageHandler = ServiceLocator.Get<CarDamageHandler>();
                _levelManager = ServiceLocator.Get<LevelManager>();

                if (_carDamageHandler != null)
                {
                    _carDamageHandler.OnHealthChanged += UpdateHealthBar;
                    InitializeHealthSegments(_carDamageHandler.MaxHealth);
                }

                if (_levelManager != null)
                {
                    _levelManager.OnLevelStart += OpenGameUIElements;
                    _levelManager.OnLevelFail += ShowFailResultScreen;
                    _levelManager.OnLevelComplete += ShowSuccessResultScreen;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Sets up button event handlers.
        /// </summary>
        private void SetButtonEvents()
        {
            _menuButton.onButtonDown.AddListener(HideResultScreen);
            _menuButton.onButtonDown.AddListener(MenuScene);
            _restartButton.onButtonDown.AddListener(RestartLevel);
        }

        private void Update()
        {
            UpdateCarSpeedText();
        }

        // --- NEW: Method to update the timer text ---
        /// <summary>
        /// Updates the timer display with the given time. Called by Level.cs.
        /// </summary>
        public void UpdateTime(float timeInSeconds)
        {
            if (_timerText != null)
            {
                _timerText.text = string.Format("{0:00}:{1:00}",
                    Mathf.FloorToInt(timeInSeconds / 60),
                    Mathf.FloorToInt(timeInSeconds % 60));
            }
        }

        // --- NEW: Method to show or hide the timer ---
        /// <summary>
        /// Shows or hides the timer UI element.
        /// </summary>
        public void ToggleTimerDisplay(bool show)
        {
            if (_timerText != null)
            {
                _timerText.gameObject.SetActive(show);
            }
        }

        private void UpdateCarSpeedText()
        {
            var carController = ServiceLocator.Get<CarController>();

            if (carController != null)
            {
                float targetSpeed = carController.CarSpeed;
                _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedVelocity, 0.1f);
                _speedText.text = $"{_currentSpeed:F0}";
            }
            else
            {
                Debug.Log("Car Controller is null");
            }
        }

        /// <summary>
        /// Initializes the health segments based on the car's maximum health.
        /// </summary>
        private void InitializeHealthSegments(int maxHealth)
        {
            foreach (Transform child in _healthContainer)
            {
                Destroy(child.gameObject);
            }

            _healthSegments.Clear();

            for (int i = 0; i < maxHealth; i++)
            {
                var segment = Instantiate(_healthSegmentPrefab, _healthContainer);
                _healthSegments.Add(segment);
            }
        }

        /// <summary>
        /// Updates the health bar UI based on the car's current health.
        /// </summary>
        private void UpdateHealthBar(int health, int maxHealth)
        {
            for (int i = 0; i < _healthSegments.Count; i++)
            {
                _healthSegments[i].SetActive(i < health);
            }
        }

        /// <summary>
        /// Opens game UI elements with animations.
        /// </summary>
        private void OpenGameUIElements()
        {
            ToggleUIElements(new[] {
                _levelIndexText.gameObject,
                _carHealthDisplayer,
                _controlButtons,
                _speedDisplayer,
                _throttleButton,
                _reverseButton,
                _turnLeftButton,
                _turnRightButton,
                _handbrakeButton  }, true, true);
        }

        /// <summary>
        /// Closes game UI elements with animations.
        /// </summary>
        private void CloseGameUIElements()
        {
            ToggleUIElements(new[] { 
                _levelIndexText.gameObject, 
                _carHealthDisplayer, 
                _controlButtons, 
                _speedDisplayer, 
                _throttleButton,
                _reverseButton, 
                _turnLeftButton,
                _turnRightButton,
                _handbrakeButton }, false, true);
        }

        /// <summary>
        /// Hides the result screen and shows game UI elements again.
        /// </summary>
        private void HideResultScreen()
        {
            ToggleUIElements(new[] { _resultText.gameObject, _menuButton.gameObject, _restartButton.gameObject, _resultScreen }, false, true);
        }

        /// <summary>
        /// Displays the fail result screen with a message and hides game UI elements.
        /// </summary>
        private void ShowFailResultScreen()
        {
            DOVirtual.DelayedCall(_resultScreenDelay, () =>
            {
                int earnedCoins = _levelManager.CurrentReward;
                int starInLevel = _levelManager.CurrentStars;

                _coinCountText.text = $"{earnedCoins}";

                for (int i = 0; i < _stars.Length; i++)
                {
                    _stars[i].SetActive(i < starInLevel);
                }

                _restartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Restart";
                GameObject[] elemets = new[] { _resultScreen, _resultText.gameObject, _menuButton.gameObject, _restartButton.gameObject };
                DisplayResultScreen(elemets, false);

            });
        }

        /// <summary>
        /// Displays the success result screen with a message and hides game UI elements.
        /// </summary>
        private void ShowSuccessResultScreen()
        {
            DOVirtual.DelayedCall(_resultScreenDelay, () =>
            {
                int earnedCoins = _levelManager.CurrentReward;
                int starInLevel = _levelManager.CurrentStars;

                _coinCountText.text = $"{earnedCoins}";

                for (int i = 0; i < _stars.Length; i++)
                {
                    _stars[i].SetActive(i < starInLevel);
                }

                _restartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
                GameObject[] elemets = new[] { _resultScreen, _resultText.gameObject, _menuButton.gameObject, _restartButton.gameObject };
                DisplayResultScreen(elemets, true);

            });
        }

        /// <summary>
        /// Displays the result screen with the given message and hides game UI elements.
        /// </summary>
        private void DisplayResultScreen(GameObject[] elements, bool success)
        {
            string message = success ? _levelCompleteMessage : _levelFailMessage;
            _resultText.text = message;

            CloseGameUIElements();

            ToggleUIElements(elements, true, true);
        }

        /// <summary>
        /// Toggles UI elements' active state with scale animations, either sequentially or simultaneously.
        /// </summary>
        private void ToggleUIElements(GameObject[] elements, bool isEnabled, bool sequential = false)
        {
            if (sequential)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    var element = elements[i];
                    float delay = i * _elementAnimationInterval;

                    if (isEnabled)
                    {
                        element.SetActive(true);
                        element.transform.DOScale(1, _elementAnimationDuration)
                            .SetEase(_openElementAnimationEase)
                            .SetDelay(delay);

                        DOTween.Sequence()
                            .AppendInterval(delay)
                            .AppendCallback(() => _audioManager.PlaySound(_uiSoundKey));
                    }
                    else
                    {
                        element.transform.DOScale(0, _elementAnimationDuration)
                            .SetEase(_closeElementAnimationEase)
                            .SetDelay(delay)
                            .OnComplete(() => element.SetActive(false));

                        DOTween.Sequence()
                            .AppendInterval(delay)
                            .AppendCallback(() => _audioManager.PlaySound(_uiSoundKey));
                    }
                }
            }
            else
            {
                foreach (var element in elements)
                {
                    if (isEnabled)
                    {
                        element.SetActive(true);
                        element.transform.DOScale(1, _elementAnimationDuration)
                            .SetEase(_openElementAnimationEase);
                    }
                    else
                    {
                        element.transform.DOScale(0, _elementAnimationDuration)
                            .SetEase(_closeElementAnimationEase)
                            .OnComplete(() => element.SetActive(false));
                    }

                    _audioManager.PlaySound(_uiSoundKey);
                }
            }
        }

        /// <summary>
        /// Initializes the UI by hiding all game elements and result screen.
        /// </summary>
        private void InitializeUI()
        {
            _resultScreen.SetActive(false);
            _resultText.gameObject.SetActive(false);
            _menuButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);

            _resultScreen.transform.localScale = Vector3.zero;
            _resultText.transform.localScale = Vector3.zero;
            _menuButton.transform.localScale = Vector3.zero;
            _restartButton.transform.localScale = Vector3.zero;

            _levelIndexText.transform.localScale = Vector3.zero;
            _carHealthDisplayer.transform.localScale = Vector3.zero;
            _controlButtons.transform.localScale = Vector3.zero;
            _speedDisplayer.transform.localScale = Vector3.zero;

            _throttleButton.transform.localScale = Vector3.zero;
            _reverseButton.transform.localScale = Vector3.zero;
            _turnLeftButton.transform.localScale = Vector3.zero;
            _turnRightButton.transform.localScale = Vector3.zero;
            _handbrakeButton.transform.localScale = Vector3.zero;

            _levelIndexText.gameObject.SetActive(true);
            _carHealthDisplayer.gameObject.SetActive(true);
            _controlButtons.gameObject.SetActive(true);
            _speedDisplayer.gameObject.SetActive(true);

            _throttleButton.gameObject.SetActive(true);
            _reverseButton.gameObject.SetActive(true);
            _turnLeftButton.gameObject.SetActive(true);
            _turnRightButton.gameObject.SetActive(true);
            _handbrakeButton.gameObject.SetActive(true);

            DisplayLevelIndex();
        }

        private void DisplayLevelIndex()
        {
            _levelIndexText.text = (_levelManager.LevelIndex + 1).ToString();
        }

        /// <summary>
        /// Restarts the current level (button action).
        /// </summary>
        private void RestartLevel()
        {
            _levelManager.HandleRestartButtonPressed();
            HideResultScreen();
        }

        /// <summary>
        /// Loads the menu scene (button action).
        /// </summary>
        private void MenuScene()
        {
            _levelManager.HandleMenuButtonPressed();
            HideResultScreen();
        }
    }
}
