using UnityEngine;

namespace PlaceBase
{
    public class BuildingButton : PlaceButton
    {
        public override void Start()
        {
            base.Start();
            
            SetPlacer(FindObjectOfType<BuildingPlacer>());
        }
    }
}