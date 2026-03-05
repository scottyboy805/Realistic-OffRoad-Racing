using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace RGSK
{
    [System.Serializable]
    public class BoardItem
    {
        public string name;

        [Header("Cell")]
        public BoardCellType cellType;
        public RaceBoardCellValue cellValue;

        [Header("Header")]
        public string header;

        [Header("Text")]
        public TMP_FontAsset font;
        public float fontSize = 0;
        public TextAlignmentOptions alignment;

        [Header("Layout Element")]
        public float preferredWidth = 100;
        public float preferredHeight = 50;
        public float flexibleWidth = 0;
        public float flexibleHeight = 0;
    }

    [CreateAssetMenu(menuName = "RGSK/UI/Race Board Layout")]
    public class RaceBoardLayout : ScriptableObject
    {
        public string title;
        public List<RaceType> raceTypes = new List<RaceType>();
        public List<BoardItem> items = new List<BoardItem>();
        public List<BoardItem> miniItems = new List<BoardItem>();
    }
}