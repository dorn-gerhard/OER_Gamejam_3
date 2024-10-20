using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator[] _animators;

    public void GetAllAnimators()
    {
        // Get all Animator components that are children of this GameObject
        _animators = GetComponentsInChildren<Animator>();
        Debug.Log(_animators.Length);
    }

    public void SetBoolForAll(string parameterName, bool value)
    {
        foreach (Animator animator in _animators)
        {
            animator.SetBool(parameterName, value);
        }
    }
}
