using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnCollision : MonoBehaviour
{
    [SerializeField] AudioSource dropSound;
    private HashSet<int> triggeredObjectIDs = new HashSet<int>(); // Track unique IDs of objects that triggered sound

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the unique ID of the collided ingredient
        int ingredientID = collision.gameObject.GetInstanceID();

        // Check if the sound has already played for this object
        if (!triggeredObjectIDs.Contains(ingredientID))
        {
            // Add the object ID to the set to prevent the sound from playing again
            triggeredObjectIDs.Add(ingredientID);

            // Get the name of the ingredient and play sound based on the name
            string ingredient = collision.gameObject.name;

            if (ingredient == "Strawberry" || ingredient == "Sugar")
            {
                dropSound.Play();
            }
        }
    }
}
