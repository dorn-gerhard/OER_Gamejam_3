using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CorrectnessIndicator : MonoBehaviour
{
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite sadSprite;
   
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        SetNeutral();
    }

    public void SetHappy()
    {
        image.sprite = happySprite;
    }

    public void SetSad()
    {
        image.sprite = sadSprite;
    }

    public void SetNeutral()
    {
        image.sprite = neutralSprite;
    }
}
