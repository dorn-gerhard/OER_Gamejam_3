using TMPro;
using UnityEngine;

public class WorkdayReport : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reportText;
    [SerializeField] private GameObject reportGroup;

    public void ShowWorkdayReport(Workday.WorkdayReport workdayReport)
    {
        reportText.text =
            $"You succesfully identified {workdayReport.CorrectPeople} out of {workdayReport.MaximumPeople} function people.";
        reportGroup.SetActive(true);
    }
}
