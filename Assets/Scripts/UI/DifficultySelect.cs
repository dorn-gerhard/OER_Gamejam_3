using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySelect : MonoBehaviour
{

    [SerializeField] UnityEngine.UI.Image easyButton;
    [SerializeField] UnityEngine.UI.Image middleButton;
    [SerializeField] UnityEngine.UI.Image hardButton;

    [SerializeField] Sprite selectedButtonSprite;
    [SerializeField] Sprite unselectedButtonSprite;

    private void Start()
    {
        easyButton.sprite = selectedButtonSprite;
    }
    public void onDifficultySelected(int difficulty_num)
    {
        switch (difficulty_num)
        {
            case 0:
                easyButton.sprite = selectedButtonSprite;
                middleButton.sprite = unselectedButtonSprite;
                hardButton.sprite = unselectedButtonSprite;
                ComplexityManager.selectedDifficulty = ComplexityManager.Difficulty.EASY;
                break;
            case 1:
                easyButton.sprite = unselectedButtonSprite;
                middleButton.sprite = selectedButtonSprite;
                hardButton.sprite = unselectedButtonSprite;
                ComplexityManager.selectedDifficulty = ComplexityManager.Difficulty.MEDIUM;
                break;
            case 2:
                easyButton.sprite = unselectedButtonSprite;
                middleButton.sprite = unselectedButtonSprite;
                hardButton.sprite = selectedButtonSprite;
                ComplexityManager.selectedDifficulty = ComplexityManager.Difficulty.HARD;
                break;
        }
        
    }
}
