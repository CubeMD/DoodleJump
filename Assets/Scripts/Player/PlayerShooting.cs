using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject playerProjectile;
    public GameObject spawnPosition;
    public float projectileSpeed;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseDir = cam.ScreenToWorldPoint(Input.mousePosition) - spawnPosition.transform.position;
            mouseDir = new Vector3(mouseDir.x, mouseDir.y, 0);

            float upAndMouseAngle = Vector3.SignedAngle(Vector3.up, mouseDir, Vector3.forward);

            if (upAndMouseAngle < 90 && upAndMouseAngle > -90)
            {
                GameObject go = Instantiate(playerProjectile, spawnPosition.transform.position, Quaternion.identity);
                go.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(new Vector3(0, 0, upAndMouseAngle)) * new Vector2(0, projectileSpeed);
                Destroy(go, 3);
            }
        }
    }
}
