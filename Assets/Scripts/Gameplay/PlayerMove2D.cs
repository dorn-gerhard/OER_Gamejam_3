using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    public float moveSpeed = 5;
    public float currentMoveSpeed = 5;
    public bool isMovementFrozen = false;

    public FixedJoystick fixedJoystick;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    void Update()
    {
        currentMoveSpeed = moveSpeed;
        //if (Input.GetKey(KeyCode.LeftShift))
        //{ 
        //    currentMoveSpeed = moveSpeed * runMultiplier;
        //}



        //if (Input.GetKey(KeyCode.Space))
        //{
        //    if (youWin)
        //    {
        //        Time.timeScale = 1f;
        //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //    }
        //    else
        //    {
        //        lineFunction.Shoot();
        //    }
        //}


        HandleMovement();
    }

    void HandleMovement()
    {
        if (isMovementFrozen) return;

        Vector3 moveDirection = Vector3.zero;

        // Check input and set movement direction
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.up; // Move up
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right; // Move right
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.down; // Move down
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left; // Move left
        }

        // Normalize the movement direction to ensure consistent speed in all directions
        moveDirection.Normalize();

        if (fixedJoystick is not null & fixedJoystick.Direction.normalized.magnitude > 0.2)
            moveDirection = fixedJoystick.Direction.normalized;

        // Move the spaceship
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }
}
