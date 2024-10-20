using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public abstract class PlantBase : MonoBehaviour
{
    [SerializeField]
    private Vector3 translation = Vector3.zero;
    [SerializeField]
    private int rotation = 0;

    [SerializeField]
    protected List<CellEffect> desiredEffects = new List<CellEffect>() { CellEffect.None };
    protected List<CellEffect> appliedEffects = new List<CellEffect>();

    public List<CellEffect> DesiredEffects => desiredEffects;

    public Vector3Int Cell => GridManager.instance.GetCellFromWorldPosition(transform.position);

    public void SetCell(Vector3Int cell)
    {
        transform.position = GridManager.instance.GetCellCenter(cell);
    }

    [Button]
    public void ApplyTranslation()
    {
        transform.Translate(translation);
    }

    [Button]
    public void ApplyRotation()
    {
        transform.position = Quaternion.Euler(0, 0, rotation) * transform.position;
    }

    public void QueryCellEffects()
    {
        appliedEffects = GridManager.instance.GetCellEffects(Cell);
    }

    public void ClearAppliedEffects()
    {
        appliedEffects.Clear();
    }

    public bool AreEffectsSatified()
    {
        return !appliedEffects.Except(desiredEffects).Any() && !desiredEffects.Except(appliedEffects).Any();
    }
}
