using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnRelease : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR// && !DEVELOPMENT_BUILD
        gameObject.SetActive(false);
#endif
    }
}
