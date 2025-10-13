//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using _Game.Management;
//using _Game.Data;
//using DG.Tweening; // To be used for animations, if desired.

///// <summary>
///// Manages the UI for the level selection screen.
///// Dynamically creates level buttons, updates their state (locked/unlocked), and handles button clicks to load levels.
///// </summary>
//public class LevelSelectionUI : MonoBehaviour
//{
//    [Header("UI Elements")]
//    [Tooltip("The parent transform where level buttons will be instantiated.")]
//    [SerializeField] private RectTransform _buttonContainer;

//    [Tooltip("The prefab for a single level button.")]
//    [SerializeField] private GameObject _levelButtonPrefab;

//    [Tooltip("The Sprite to use for locked levels.")]
//    [SerializeField] private Sprite _lockedSprite;

//    [Tooltip("The Sprite to use for unlocked levels.")]
//    [SerializeField] private Sprite _unlockedSprite;

//    [Header("Dependencies")]
//    [Tooltip("Reference to the LevelManager.")]
//    private LevelManager _levelManager;

//    [Tooltip("Reference to the GameData ScriptableObject.")]
//    private GameData _gameData;

//    private void Awake()
//    {
//        // Use the ServiceLocator to get references to key managers.
//        _levelManager = ServiceLocator.Get<LevelManager>();
//        _gameData = ServiceLocator.Get<GameData>();
//    }

//    private void Start()
//    {
//        InitializeLevelButtons();
//    }

//    /// <summary>
//    /// Initializes and displays the level selection buttons based on game data.
//    /// </summary>
//    private void InitializeLevelButtons()
//    {
//        // Clear any existing buttons to prevent duplicates on scene reload.
//        foreach (Transform child in _buttonContainer)
//        {
//            Destroy(child.gameObject);
//        }

//        // Get the index of the last completed level to determine which levels are unlocked.
//        int currentLevelIndex = _gameData.CurrentLevelIndex;
//        int totalLevels = _gameData.LevelList.Count;

//        for (int i = 0; i < totalLevels; i++)
//        {
//            int levelIndex = i;
//            GameObject buttonInstance = Instantiate(_levelButtonPrefab, _buttonContainer);
//            Button buttonComponent = buttonInstance.GetComponent<Button>();
//            Image buttonImage = buttonInstance.GetComponent<Image>();
//            TextMeshProUGUI buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();

//            // Set the button text to the level number.
//            buttonText.text = (levelIndex + 1).ToString();

//            // Check if the level is unlocked.
//            bool isUnlocked = levelIndex <= currentLevelIndex;

//            if (isUnlocked)
//            {
//                // Unlocked level: Enable the button and set its visual state.
//                buttonImage.sprite = _unlockedSprite;
//                buttonComponent.interactable = true;
//                buttonComponent.onClick.AddListener(() => OnLevelButtonClick(levelIndex));
//            }
//            else
//            {
//                // Locked level: Disable the button and change its visual state.
//                buttonImage.sprite = _lockedSprite;
//                buttonComponent.interactable = false;
//                // Optional: You could add a tooltip or other visual flair to indicate it's locked.
//            }

//            // Animate the button creation for a dynamic effect.
//            buttonInstance.transform.localScale = Vector3.zero;
//            buttonInstance.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(i * 0.1f);
//        }
//    }

//    /// <summary>
//    /// Called when an unlocked level button is clicked.
//    /// </summary>
//    /// <param name="levelIndex">The index of the level to load.</param>
//    private void OnLevelButtonClick(int levelIndex)
//    {
//        Debug.Log($"Loading level {levelIndex + 1}...");

//        // Set the current level index in GameData before loading the scene.
//        _gameData.CurrentLevelIndex = levelIndex;

//        // Use the LevelManager to handle the scene transition.
//        // NOTE: The LevelManager.cs provided does not have a public method to select a specific level.
//        //_levelManager.LoadGameScene();
//    }
//}
