using TMPro;
using UnityEngine;

public class WorkdayReport : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reportText;
    [SerializeField] private GameObject reportGroup;

    public void ShowWorkdayReport(Workday.WorkdayReport workdayReport)
    {
        reportText.text =
            $"{workdayReport.CorrectPeople}/{workdayReport.MaximumPeople}";
        reportGroup.SetActive(true);
    }
}
