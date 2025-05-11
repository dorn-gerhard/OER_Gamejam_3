using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkdayReport : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reportText;
    [SerializeField] private GameObject reportGroup;

    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;
    
    [SerializeField] private Sprite fullStar;
    [SerializeField] private Sprite emptyStar;
    
    public void ShowWorkdayReport(Workday.WorkdayReport workdayReport)
    {
        reportText.text =
            $"{workdayReport.CorrectPeople}/{workdayReport.MaximumPeople}";
        reportGroup.SetActive(true);
        UpdateStarImages(workdayReport);
    }

    private void UpdateStarImages(Workday.WorkdayReport workdayReport)
    {
        if (workdayReport.CorrectPeople > 0)
        {
            star1.sprite = fullStar;
        }
        else
        {
            star1.sprite = emptyStar;
        }

        if (workdayReport.CorrectPeople > workdayReport.MaximumPeople / 2)
        {
            star2.sprite = fullStar;
        }
        else
        {
            star2.sprite = emptyStar;
        }
        if (workdayReport.CorrectPeople >= workdayReport.MaximumPeople)
        {
            star3.sprite = fullStar;
        }
        else
        {
            star3.sprite = emptyStar;
        }
    }
}
