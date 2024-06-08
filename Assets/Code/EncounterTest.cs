using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Code
{
    public class EncounterTest : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private ScreenRoot _screenRoot;
        
        [SerializeField] private GameObject encounterPrefab;

        private bool canEncounter = true;
        
        private async void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space) && canEncounter)
            {
                canEncounter = false;
                await _diContainer.InstantiatePrefab(encounterPrefab, _screenRoot.transform).GetComponent<EnemyEncounterController>().Init();
                canEncounter = true;
            }
        }
    }
}