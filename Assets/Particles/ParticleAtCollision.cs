using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Particles
{
    public class ParticleAtCollision : MonoBehaviour
    {
        public ParticleSystem collisionParticles;
        [CanBeNull] public string collisionTag = null;
        public AudioSource audioSource;
   
        private void OnCollisionEnter(Collision other)
        {
            if (collisionTag is not null && !other.gameObject.CompareTag(collisionTag))
            {
                return; // skip if the tag does not match the configured
            }

            List<ContactPoint> contactPoints = new();
            other.GetContacts(contactPoints);
            contactPoints.ForEach(cp =>
            {
                var ps = Instantiate(collisionParticles, cp.point, Quaternion.identity);
                ps.gameObject.transform.localScale *= gameObject.transform.localScale.magnitude;
                ps.gameObject.SetActive(true);
                Destroy(ps.gameObject, ps.main.duration);
            });
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }
    }
}
