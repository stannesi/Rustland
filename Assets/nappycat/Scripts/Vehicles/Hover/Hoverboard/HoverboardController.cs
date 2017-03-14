using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nappycat.Vehicles.Hover
{

	// controller types (keyboard, mobile)

	internal enum ControllerType 
	{
		KeyboardController,
		MobileUIController,
		MobileGestureController
	}

	internal enum SpeedType
	{
		MPH,
		KPH
	}


	[RequireComponent (typeof(Rigidbody))]
	public class HoverboardController : MonoBehaviour
	{		
		private Rigidbody rb;
		public Transform COM;

		// can receive play input
		public bool controlOn;

		// engine on/off
		public bool engineOn;

		// infinite runner mode
		public bool runningMode;


		[SerializeField] private ControllerType m_controllerType;
		[SerializeField] private SpeedType m_speedType;


		// stabilizer thrusters
		public List<Transform> stabilizerThruster = new List<Transform>();


		// stabilizers
		//public float stabilizerForce = 0f;
		//public float stabilizerDamperForce = 0f;

		public float stabilizerConstant = 10000f;
		public float stabilizerDamperConstant = 1000f;


		// 	torques
		public float motorTorque = 20000f;
		public float steerTorque = 5000f;


		// engine torque curve depends on craft speed
		public AnimationCurve engineTorqueCurve;


		// speeds
		//private float speed = 0f;
		public float speed = 0f;
		public float maxSpeed = 250f;


		// inputs
		[HideInInspector] public float gasInput = 0f;
		[HideInInspector] public float steerInput = 0f;


		// stabilizers variablesmotor
		private float stability = 0.5f;
		private float reflection = 100f;
		public float hoverHeight = 3f;
		public float maxAngularvelocity = 2f;


		// sounds
		// public AudioClip engineSound;
		// private AudioSource engineSoundSource;
		public AudioClip[] crashSounds;
		private AudioSource crashSoundSource;


		// particles
		private List<ParticleSystem> thrumotormotorsterSmoke = new List<ParticleSystem>();


		// contact particles


		// lights
		public Light[] headLights;
		public Light[] particleLights;


		// damage
		public bool useDamage = true;


		void Start()
		{
			// rigibody component
			rb = GetComponent<Rigidbody> ();
			rb.centerOfMass = COM.localPosition;
			rb.maxAngularVelocity = maxAngularvelocity;
		}


		// Input controls
		void Inputs ()
		{
			if (m_controllerType == ControllerType.KeyboardController)
			{
				if (controlOn)
				{
					
					if (!runningMode) {
						gasInput = Input.GetAxis ("Vertical");
					} else {
						gasInput = 1.0f;
					}

					steerInput = Input.GetAxis ("Horizontal");
				} else {

					if (!runningMode) {
						gasInput = 0.0f;
					} else {
						gasInput = 1.0f;
					}
				}
			}
		}


		// Update is called once per frame
		void Update ()
		{
		
		}


		// Physics FixedUpdate is called once per frame
		void FixedUpdate ()
		{
			Inputs ();

			if (!engineOn)
				return;

			Engines ();
			Stabilizers();
		}

		void HoverSpeed()
		{
			speed = rb.velocity.magnitude;

			switch(m_speedType)
			{
				case SpeedType.MPH:
					speed *= 2.23693629f;
					break;
				case SpeedType.KPH:
					speed *= 3.6f;
					break;
			}
		}

		// engines
		void Engines()
		{
			if (!runningMode) {
				
				HoverSpeed ();
				// speed = rb.velocity.magnitude * 3.6f;


				if (speed < maxSpeed)
					rb.AddForceAtPosition(transform.forward * ((motorTorque * Mathf.Clamp (gasInput, -0.5f, 1f)) * engineTorqueCurve.Evaluate (speed)), COM.position, ForceMode.Force);

				rb.AddRelativeTorque (Vector3.up * ((steerTorque * Mathf.Lerp (1f, 0.25f, speed / maxSpeed)) * steerInput), ForceMode.Force);
				rb.AddRelativeTorque (Vector3.forward * ((-steerTorque / 1f) * steerInput), ForceMode.Force);

			} else {
				HoverSpeed ();
				//rb.constraints = RigidbodyConstraints.FreezeRotationX;
				//rb.constraints = RigidbodyConstraints.FreezeRotationY;
			
				rb.AddForceAtPosition(transform.forward * ((motorTorque * 1f) * engineTorqueCurve.Evaluate (speed)), COM.position, ForceMode.Force);
	
				// strafe left and right
				rb.transform.Translate (Vector3.right * steerInput);


				//rb.AddRelativeTorque (Vector3.up * ((steerTorque * Mathf.Lerp (1f, 0.25f, speed / maxSpeed)) * steerInput), ForceMode.Force);
				rb.AddRelativeTorque (Vector3.forward * ((-steerTorque / 1f) * steerInput), ForceMode.Force);
			}

			Vector3 localVelocity = transform.InverseTransformDirection (rb.velocity);
			localVelocity = new Vector3 (Mathf.Lerp (localVelocity.x, 0f, Time.fixedDeltaTime * 1f), localVelocity.y, localVelocity.z);
			rb.velocity = transform.TransformDirection (localVelocity);
			rb.angularVelocity = Vector3.Lerp (rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 1f);
		
		}

		// stabilizers
		void Stabilizers ()
		{
			Vector3 predictedUp = Quaternion.AngleAxis (rb.velocity.magnitude * Mathf.Rad2Deg * stability / reflection, rb.angularVelocity) * transform.up;
			Vector3 torqueVector = Vector3.Cross (predictedUp, Vector3.up);

			rb.AddTorque (torqueVector * reflection * reflection);
		}

		// draw scene view gizmo
		void OnDrawGizmos()
		{
			RaycastHit hit;

			for (int i = 0; i < stabilizerThruster.Count; i++)
			{
				if (!stabilizerThruster[i].GetComponent<HoverboardThruster>())
					stabilizerThruster[i].gameObject.AddComponent<HoverboardThruster>();

				if (Physics.Raycast (stabilizerThruster [i].position, stabilizerThruster [i].forward, out hit, hoverHeight * 2f)
					&& hit.transform.root != transform)
				{
					Debug.DrawRay (stabilizerThruster [i].position, stabilizerThruster [i].forward * hit.distance, new Color (Mathf.Lerp (1f, 0f, hit.distance / (hoverHeight * 1.5f)), 0f, 1f));
					Gizmos.color = new Color (Mathf.Lerp (1f, 0f, hit.distance / (hoverHeight * 1.5f)), Mathf.Lerp (0f, 1f, hit.distance / (hoverHeight * 1.5f)), 0f, 1f);
					Gizmos.DrawSphere (hit.point, 0.5f);
				}

				Matrix4x4 temp = Gizmos.matrix;
				Gizmos.matrix = Matrix4x4.TRS (stabilizerThruster [i].position, stabilizerThruster [i].rotation, Vector3.one);
				Gizmos.DrawFrustum(Vector3.zero, 30f, hoverHeight, 0.1f, 1f);
				Gizmos.matrix = temp;
			}
		}
	}
}