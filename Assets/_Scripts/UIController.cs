using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField]
    private GameObject _instructionView = null;
    [SerializeField]
    private GameObject _currentInstructionView = null;
    [SerializeField]
    private TMP_Text _currentInstructionText = null;
    [SerializeField]
    private GameObject _helpText = null;
    [SerializeField]
    private Button _confirmBtn = null;
    [SerializeField]
    private TMP_Text _resultText = null;
    [SerializeField]
    private GameObject _resultTextParent = null;

    private void Start()
    {
        _helpText.SetActive(true);
        _instructionView.SetActive(false);
        _resultTextParent.gameObject.SetActive(false);
        _currentInstructionView.SetActive(false);

        _confirmBtn.onClick.AddListener(() =>
            {
                _instructionView.SetActive(false);

            });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!GameLoop.instance.Simulating)
                _instructionView.SetActive(!_instructionView.activeSelf);
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            _helpText.SetActive(!_helpText.activeSelf);
        }
    }

    public void SetResultText(string text)
    {
        _resultText.SetText(text);
    }

    public void ToggleResultText()
    {
        _resultTextParent.SetActive(!_resultTextParent.activeSelf);
    }

    public void SetCurrentInstructionText(string text)
    {
        _currentInstructionText.SetText(text);
    }

    public void ToggleCurrentInstructionView()
    {
        _currentInstructionView.SetActive(!_currentInstructionView.activeSelf);
    }
}
