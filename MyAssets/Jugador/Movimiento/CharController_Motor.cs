using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour {

	private float speed = 8f;
	private float sensitivity = 30.0f;
	private float WaterHeight = 15.5f;
	CharacterController character;
	public GameObject cam;
	float moveFB, moveLR;
	float rotX, rotY;
	public bool webGLRightClickRotation = true;
	float gravity = -9.8f;

	private float minY = -90f; // Límite inferior (mirar hacia abajo)
	private float maxY = 90f;  // Límite superior (mirar hacia arriba)
	private float currentRotationX = 0f;
	public int jumpingForce = 20;
	private Camaras camarasScript;
	private Boolean camPlayer = true;


	void Start(){
		//LockCursor ();
		Cursor.visible = false;
		character = GetComponent<CharacterController> ();
		if (Application.isEditor) {
			webGLRightClickRotation = false;
			sensitivity = sensitivity * 1.5f;
		}
		camarasScript = FindObjectOfType<Camaras>();
	}


	void CheckForWaterHeight(){
		if (transform.position.y < WaterHeight) {
			gravity = 0f;			
		} else {
			gravity = -9.8f;
		}
	}



	void Update(){
		moveFB = Input.GetAxis ("Horizontal") * speed;
		moveLR = Input.GetAxis ("Vertical") * speed;

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY = Input.GetAxis ("Mouse Y") * sensitivity;

		//rotX = Input.GetKey (KeyCode.Joystick1Button4);
		//rotY = Input.GetKey (KeyCode.Joystick1Button5);

		CheckForWaterHeight ();


		Vector3 movement = new Vector3 (moveFB, gravity, moveLR);

		// Normalizamos el vector de movimiento en el plano XZ para que no sume velocidad
		if (movement.x != 0 || movement.z != 0)
		{
			movement = movement.normalized * speed;
		}


		if (webGLRightClickRotation) {
			if (Input.GetKey (KeyCode.Mouse0)) {
				CameraRotation (cam, rotX, rotY);
			}
		} else if (!webGLRightClickRotation) {
			CameraRotation (cam, rotX, rotY);
		}

		movement = transform.rotation * movement;
		character.Move (movement * Time.deltaTime);

		if (Input.GetAxis("Jump") != 0)
        {
			Vector3 directionJump = new Vector3(0, 1, 0);
			character.Move(jumpingForce * directionJump * Time.deltaTime);
		}

        if (Input.GetKey(KeyCode.C) && camPlayer) {
            camarasScript.cambiarCamara();
			camPlayer = !camPlayer;
        }

		if (Input.GetKey(KeyCode.C) && !camPlayer) {
            camarasScript.volverMainCam();
			camPlayer = !camPlayer;
        }
	}


	void CameraRotation(GameObject cam, float rotX, float rotY)
	{
		// Rotación horizontal del personaje (rotación en el eje Y)
		transform.Rotate(0, rotX * Time.deltaTime, 0);

		// Acumular la rotación vertical y limitarla
		currentRotationX += -rotY * Time.deltaTime;
		currentRotationX = Mathf.Clamp(currentRotationX, minY, maxY);

		// Aplicar la rotación limitada a la cámara
		cam.transform.localEulerAngles = new Vector3(currentRotationX, cam.transform.localEulerAngles.y, 0);
	}

}
