using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;


namespace Platformer.Mechanics
{
    /// <summary>
    /// This class contains the data required for implementing token collection mechanics.
    /// It does not perform animation of the token, this is handled in a batch by the 
    /// TokenController in the scene.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Pickup : MonoBehaviour
    {
        public AudioClip tokenCollectAudio;

        //active frame in animation, updated by the controller.
        internal bool collected = false;

        void OnTriggerEnter2D(Collider2D other)
        {
            //only exectue OnPlayerEnter if the player collides with this token.
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null && !collected)
            {
                OnPlayerEnter(player);
            }
        }

        public virtual void OnPlayerEnter(PlayerController player)
        {
            AudioSource.PlayClipAtPoint(tokenCollectAudio, transform.position);

            collected = true;

            Destroy(gameObject);
        }
    }
}