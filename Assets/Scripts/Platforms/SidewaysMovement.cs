using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewaysMovement : MonoBehaviour
{
    public float speed;
    private float initialXPosition;
    private float Distance;
    public float maxDistance;
    private bool isMovingLeft;

    void Start()
    {
        initialXPosition = transform.position.x;
    }

    void FixedUpdate()
    {
        if (isMovingLeft)
        {
            Distance -= speed * Time.fixedDeltaTime;
        }
        else
        {
            Distance += speed * Time.fixedDeltaTime;
        }

        if (Distance > maxDistance || Distance < -maxDistance)
        {
            isMovingLeft = !isMovingLeft;
        }

        transform.position = new Vector3(initialXPosition + Distance, transform.position.y, transform.position.z);
    }
}
