using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Vector3 cameraOffset;

    public float speed;
    Vector3 velocity;
    public float turnSpeed;

    public Camera cam;

    void Start() {
        speed = 100;
        turnSpeed = 2;
        cameraOffset = new Vector3(0, 0, 0);

        transform.position = Vector3.zero;
        cam.transform.position = transform.position + cameraOffset;
    }

    void FixedUpdate()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 direction = input.normalized;

        velocity = direction * speed * Time.deltaTime;
        transform.position += velocity;

        if(Input.GetKey(KeyCode.Space)) {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.LeftShift)) {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.Q)) {
            transform.Rotate(0, -turnSpeed, 0, Space.World);
            cam.transform.Rotate(0, -turnSpeed, 0, Space.World);
        }
        if(Input.GetKey(KeyCode.E)) {
            transform.Rotate(0, turnSpeed, 0, Space.World);
            cam.transform.Rotate(0, turnSpeed, 0, Space.World);
        }

        cam.transform.position = transform.position + cameraOffset;
    }
}
