using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeController : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToSetActiveOnFreeze = new List<GameObject>();
    [SerializeField] List<GameObject> objectsToSetUnactiveOnFreeze = new List<GameObject>();

    [SerializeField] float freezeDelay = 5;

    float timeSinceUnfrozen = 0;

    bool isFrozen = false;

    public static FreezeController current;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        timeSinceUnfrozen = 0;  
    }

    void Update()
    {
        //if (isFrozen)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Unfreeze();
        //    }
        //    return;
        //}

        timeSinceUnfrozen += Time.deltaTime;
        if (timeSinceUnfrozen >= freezeDelay)
        {
            Freeze();
        }

        if (PlayerController.current.currentConfidence > 25)
        {

        }
        else if (PlayerController.current.currentConfidence > 50)
        {

        }
        else if (PlayerController.current.currentConfidence > 75)
        {

        }
    }

    public void Freeze()
    {
        isFrozen = true;

        Time.timeScale = 0;

        //PlayerController.current.Freeze();
        //foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        //{
        //    enemy.Freeze();
        //}

        foreach (GameObject obj in objectsToSetActiveOnFreeze)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToSetUnactiveOnFreeze)
        {
            obj.SetActive(false);
        }
    }

    public void Unfreeze()
    {
        isFrozen = false;
        timeSinceUnfrozen = 0;

        Time.timeScale = 1;


        //PlayerController.current.UnFreeze();
        //foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        //{
        //    enemy.UnFreeze();
        //}

        foreach (GameObject obj in objectsToSetActiveOnFreeze)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToSetUnactiveOnFreeze)
        {
            obj.SetActive(true);
        }
    }
}
