using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustlandSignboard : MonoBehaviour {

	public float forceAmount = 10f;

	private Rigidbody rb;


	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
	{
		rb.AddForce (-transform.forward * forceAmount, ForceMode.Acceleration);
		rb.useGravity = true;
	}
}
