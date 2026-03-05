using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK
{
    public class AutoFinishScoreAttack : MonoBehaviour
    {
        List<ScoreCollectible> _collectibles = new List<ScoreCollectible>();

        void OnEnable()
        {
            RGSKEvents.OnScoreAdded.AddListener(OnScoreAdded);
        }

        void OnDisable()
        {
            RGSKEvents.OnScoreAdded.RemoveListener(OnScoreAdded);
        }

        void Start()
        {
            _collectibles = FindObjectsByType<ScoreCollectible>(FindObjectsSortMode.None).ToList();
        }

        void OnScoreAdded(Competitor c, float value)
        {
            if (_collectibles.All(x => x.Collected))
            {
                RaceManager.Instance.ForceFinishRace(false);
            }
        }
    }
}