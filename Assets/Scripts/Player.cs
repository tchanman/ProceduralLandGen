using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Camera cam;
    Vector3 camOffset = new Vector3(0, 1.5f, -5);

    public float speed = 10;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 direction = input.normalized;

        velocity = direction * speed * Time.deltaTime;
        transform.position += velocity;

        cam.transform.position = transform.position + camOffset;

        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Jump");
        }
    }
}
