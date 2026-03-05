using UnityEngine;

namespace RGSK
{
    public class ScoreCollectible : RaceCollectible
    {
        [SerializeField] float value = 10;

        protected override void Collect(RGSKEntity entity)
        {
            RaceManager.Instance?.AddCompetitorScore(entity.Competitor, value);
        }
    }
}