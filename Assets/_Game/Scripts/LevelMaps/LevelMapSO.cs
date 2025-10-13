//using System.Collections.Generic;
//using UnityEngine;
//using _Game.LevelSystem;

//namespace _Game.Data
//{
//    /// <summary>
//    /// A ScriptableObject that represents a single map in the game.
//    /// It contains a list of levels and the number of stars required to unlock this map.
//    /// This allows for easy creation of new maps with their own set of levels and progression requirements.
//    /// </summary>
//    [CreateAssetMenu(fileName = "LevelMapSO", menuName = "Game Maps/LevelMapSO")]
//    public class LevelMapSO : ScriptableObject
//    {
//        [Tooltip("The unique ID of this map, used for saving and loading progress.")]
//        [SerializeField] private string _mapId;

//        [Tooltip("The list of levels contained within this map.")]
//        [SerializeField] private List<Level> _levels;

//        [Tooltip("The number of stars required from previous maps to unlock this map.")]
//        [SerializeField] private int _requiredStarsToUnlock;

//        /// <summary>
//        /// Gets the unique ID of the map.
//        /// </summary>
//        public string MapId => _mapId;

//        /// <summary>
//        /// Gets the list of levels for this map.
//        /// </summary>
//        public List<Level> Levels => _levels;

//        /// <summary>
//        /// Gets the number of stars required to unlock this map.
//        /// </summary>
//        public int RequiredStarsToUnlock => _requiredStarsToUnlock;
//    }
//}
