﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody rigidBody;

	//transform data
	private float xrotation;
	private float yrotation;
	private static readonly float deg = 180f / (float)Mathf.PI;

	private bool canJump = true;
	private bool grounded = true;

	void Start() {
		rigidBody = GetComponent <Rigidbody> ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	void Update () {
		if (!grounded && !canJump && Mathf.Abs (rigidBody.velocity.y) < 1) {
			rigidBody.useGravity = false;
			Vector3 v = rigidBody.velocity;
			v.y = 0;
			rigidBody.velocity = v;
		}

		float xrotate = Input.GetAxis ("Mouse X") * 0.1f;
		xrotation += xrotate;

		float yrotate = Input.GetAxis ("Mouse Y") * -0.1f;
		yrotation += yrotate;

		transform.rotation = Quaternion.identity;
		transform.RotateAround (transform.position, new Vector3 (1, 0, 0), yrotation * deg);
		transform.RotateAround (transform.position, new Vector3 (0, 1, 0), xrotation * deg);

		float vy = 0;
		if (Input.GetButton ("Jump") && canJump) {
			grounded = false;
			vy = 1;
		} else if (!grounded) {
			canJump = false;
		}

		Vector3 force = Quaternion.Euler (0, xrotation * deg, 0) * new Vector3 (
			Input.GetAxis ("Horizontal"),
			vy,
			Input.GetAxis ("Vertical")
		) * 10;
		rigidBody.AddForce (force);
	}

	void OnCollisionEnter(Collision colInfo) {
		rigidBody.useGravity = true;
		grounded = true;
		canJump = true;
	}

}