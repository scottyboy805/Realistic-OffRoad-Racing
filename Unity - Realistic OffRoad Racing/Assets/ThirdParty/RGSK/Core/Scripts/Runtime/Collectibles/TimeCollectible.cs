using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class TimeCollectible : RaceCollectible
    {
        [SerializeField] float value = 10;

        protected override void Collect(RGSKEntity entity)
        {
            RaceManager.Instance?.ExtendCompetitorTimer(entity.Competitor, value);
        }
    }
}