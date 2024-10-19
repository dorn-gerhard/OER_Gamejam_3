using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    List<Vector3> path;

    public void InitializePath(List<Vector3> pathInput)
    {
        path = pathInput;
    }

  

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path.Count > 0)
        {
            transform.position = path[0];
            path.RemoveAt(0);
        }
        else
        {
            Destroy(gameObject, 0.0f);
        }
    }
}
