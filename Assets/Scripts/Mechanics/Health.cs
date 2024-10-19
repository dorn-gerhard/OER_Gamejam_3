using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        public FunctionType functionType;

        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 1;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;

        int currentHP;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment()
        {
            currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement(FunctionType fT)
        {
            if (fT != functionType) return;

            currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);

            if (currentHP == 0)
            {
                OnNoHealth();
            }
        }

        private void OnNoHealth()
        {
            if (GetComponent<PlayerController>())
            {
                //GetComponent<PlayerController>().Death();
            }
            else if (GetComponent<EnemyController>())
            {
                GetComponent<EnemyController>().Death();
            }
        }

        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>
        public void Die()
        {
            currentHP = 0;
            OnNoHealth();
        }

        void Awake()
        {
            currentHP = maxHP;
        }
    }
}
