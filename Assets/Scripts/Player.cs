using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	
	void Update () {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            MoveUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            MoveLeft();
        }

    }

    void MoveUp() {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1.0f)) {
            transform.position += Vector3.forward;
        }
        
    }

    void MoveDown() {
        Ray ray = new Ray(transform.position, Vector3.back);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1.0f)) {
            transform.position += Vector3.back;
        }
        
    }

    void MoveRight() {
        Ray ray = new Ray(transform.position, Vector3.right);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1.0f)) {
            transform.position += Vector3.right;
        }
        
    }

    void MoveLeft() {
        Ray ray = new Ray(transform.position, Vector3.left);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1.0f)) {
            transform.position += Vector3.left;
        }
        
    }
}
