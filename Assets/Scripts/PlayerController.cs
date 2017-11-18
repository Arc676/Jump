using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private Rigidbody rigidBody;

	//transform data
	private float xrotation;
	private float yrotation;
	private static readonly float deg = 180f / (float)Mathf.PI;

	private bool isJumping = false;
	private bool canJump = true;
	private bool grounded = true;

	private int ballsObtained = 0;

	private bool canWakeUp = false;
	private float flightTime = 0;
	[SerializeField] private Text wakeUpLabel;

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

		if (!rigidBody.useGravity) {
			flightTime += Time.deltaTime;
			if (flightTime > 20) {
				wakeUpLabel.gameObject.SetActive (true);
				canWakeUp = true;
			}
		}

		if (canWakeUp && Input.GetKeyDown (KeyCode.X)) {
			canWakeUp = false;
			rigidBody.useGravity = true;
			wakeUpLabel.gameObject.SetActive (false);
			transform.position = new Vector3 (-22.6f, 26, 0);
			rigidBody.velocity = Vector3.zero;
			flightTime = 0;
			grounded = true;
		}

		bool changed = false;
		Vector3 pos = transform.position;
		if (Mathf.Abs (pos.x + rigidBody.velocity.x) > 750) {
			pos.x *= -1;
			changed = true;
		}
		if (Mathf.Abs (pos.z + rigidBody.velocity.z) > 750) {
			changed = true;
			pos.z *= -1;
		}
		if (changed) {
			transform.position = pos;
		}

		float xrotate = Input.GetAxis ("Mouse X") * 0.1f;
		xrotation += xrotate;

		float yrotate = Input.GetAxis ("Mouse Y") * -0.1f;
		yrotation += yrotate;

		transform.rotation = Quaternion.identity;
		transform.RotateAround (transform.position, new Vector3 (1, 0, 0), yrotation * deg);
		transform.RotateAround (transform.position, new Vector3 (0, 1, 0), xrotation * deg);

		float fy = 0;
		if (Input.GetButton ("Jump") && canJump) {
			isJumping = true;
			grounded = false;
			fy = 1;
		} else if (!grounded) {
			isJumping = false;
			canJump = false;
		}

		float fx = Input.GetAxis ("Horizontal") * (grounded ? 1 : 0.3f);
		float fz = Input.GetAxis ("Vertical") * (grounded ? 1 : 0.3f);
		Vector3 force = Quaternion.Euler (0, xrotation * deg, 0) * new Vector3 (fx, fy, fz) * 10;
		rigidBody.AddForce (force);
	}

	void OnCollisionEnter(Collision colInfo) {
		rigidBody.useGravity = true;
		flightTime = 0;
		grounded = true;
		canJump = true;
	}

	void OnCollisionExit(Collision colInfo) {
		canJump = isJumping;
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("ScoreBall")) {
			ballsObtained++;
			Destroy (other.gameObject);
		}
	}

}