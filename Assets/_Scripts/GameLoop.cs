using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
    public static GameLoop instance { get; private set; }

    [SerializeField]
    private SpecificPlant _plant = null;
    [SerializeField]
    private Vector3Int _initialCellForPlant = Vector3Int.zero;
    [SerializeField]
    private List<Vector3Int> _rainCells = new List<Vector3Int>();

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
    private Button _confirmBtn = null;

    [SerializeField]
    private List<Slot> _slots = new List<Slot>();
    private Transformation _currentTransformation;

    public bool Simulating = false;

    void Start()
    {
        _confirmBtn.onClick.AddListener(OnConfirmPressed);
        Init();
    }

    private void Init()
    {
        _plant.SetCell(_initialCellForPlant);
        WeatherManager.instance.InitRainCells(_rainCells);
    }

    private void OnConfirmPressed()
    {
        Simulating = true;
        StartCoroutine(ShowInstructions());
    }

    private IEnumerator ShowInstructions()
    {
        int idx = 0;
        UIController.instance.ToggleCurrentInstructionView();

        while (idx < _slots.Count)
        {
            _currentTransformation = _slots[idx].Item.Transformation;
            UIController.instance.SetCurrentInstructionText(_currentTransformation.ToString());
            Vector3 startPos = _plant.transform.position;
            Vector3 endPos = _currentTransformation.Transform(startPos);
            Vector3 currentPos = startPos;

            float elapsedTime = 0f;
            float secondsPerInstruction = 3f;

            while (elapsedTime < secondsPerInstruction)
            {
                float t = (elapsedTime / secondsPerInstruction);
                switch (_currentTransformation.Type)
                {
                    case TransformationType.Translation:
                        _plant.transform.position = Vector3.Lerp(currentPos, endPos, t);
                        break;
                    case TransformationType.Rotation:
                        var currentRot = _currentTransformation.RotationDegrees * t;
                        _plant.transform.position = Quaternion.Euler(0f, 0f, currentRot) * startPos;
                        break;
                    case TransformationType.None:
                        break;
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            idx++;
            _plant.transform.position = endPos;
            UIController.instance.ToggleCurrentInstructionView();
            yield return new WaitForSeconds(1f);

            elapsedTime = 0f;
            float secondsToReadjust = 0.25f;
            startPos = endPos;
            currentPos = startPos;
            endPos = GridManager.instance.GetCellCenterFromWorldPosition(startPos);
            while (elapsedTime < secondsToReadjust)
            {
                float t = (elapsedTime / secondsToReadjust);
                
                _plant.transform.position = Vector3.Lerp(currentPos, endPos, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _plant.transform.position = endPos;
            yield return new WaitForSeconds(1.5f);
            UIController.instance.ToggleCurrentInstructionView();
        }
        UIController.instance.ToggleCurrentInstructionView();
        yield return EvaluatePlantPositions();
    }

    private IEnumerator EvaluatePlantPositions()
    {
        yield return new WaitForSeconds(1f);
        _plant.QueryCellEffects();
        if(_plant.AreEffectsSatified())
        {
            UIController.instance.SetResultText("The plant survived :)");
        }
        else
        {
            UIController.instance.SetResultText("The plant got severely damaged :(");
        }

        UIController.instance.ToggleResultText();
        yield return new WaitForSeconds(3f);
        UIController.instance.ToggleResultText();
        Simulating = false;
    }
}
