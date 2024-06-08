using System;
using System.Linq;
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
                await _diContainer.InstantiatePrefab(encounterPrefab, _screenRoot.transform)
                    .GetComponent<EnemyEncounterController>().Init();
                canEncounter = true;
            }

            var m = new MathProblem
            {
                problemType = MathProblemTypes.FRACTIONS, terms = new[]
                {
                    new MathValue { nominator = 2, denominator = 9 },
                    new MathValue { nominator = 6, denominator = 9 },
                    new MathValue { nominator = 3, denominator = 9 },
                }
            };
            var k = m.GetStepwiseExplanation();
            var g = 3;
            
            var m2 = new MathProblem
            {
                problemType = MathProblemTypes.FRACTIONS, terms = new[]
                {
                    new MathValue { nominator = 8, denominator = 8 },
                    new MathValue { nominator = 8, denominator = 8 },
                    new MathValue { nominator = 8, denominator = 8 },
                }
            };
            var k2 = m2.GetStepwiseExplanation();
            var g2 = 3;
        }
    }
}