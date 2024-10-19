using UnityEngine;

namespace Platformer.Mechanics
{
    public class Pickup_Debug : Pickup
    {
        [SerializeField] float confidenceGain = 100f;

        public override void OnPlayerEnter(PlayerController player)
        {
#if UNITY_EDITOR
            player.ChangeConfidence(confidenceGain);
#endif
        }
    }
}