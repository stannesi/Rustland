using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nappycat.Vehicles.Hover
{
	public class HoverboardAudio : MonoBehaviour
	{

		// public AudioSource engineSound;
		// private float Soundpitch;

		public enum EngineSoundType
		{
			// simple style audio
			Simple,
			// four channel audio
			FourChannel
		}

		// set the default audio options to be four channel
		public EngineSoundType engineSoundType = EngineSoundType.FourChannel;
		// audio clip for low acceleration
		public AudioClip lowAccelClip;
		// audio clip for low deceleration
		public AudioClip lowDecelClip;
		// audio clip for high acceleration
		public AudioClip highAccelClip;
		// audio clip for high deceleration
		public AudioClip highDecelClip;


		// used for altering the pitch of audio clips
		private const float pitchMultiplier = 1f;
		// the lowest possible pitch for the low sounds
		public const float lowPitchMin = 0.3f;
		// the highest possible pitch for the low sounds
		public const float lowPitchMax = 0.8f;
		// used of altering the pitch of high sounds
		private const float highPitchMultiplier = 0.25f;

		// the maximum distance where rollof starts to take place
		public float maxRollofDistance = 500f;

		// the mount of doppler effect used in the audio
		public float dopplerLevel = 1f;
		// togglr for using doppler
		public bool useDoppler = true;
	

		// flag for knowing if we have started sounds
		private bool m_startSound;


		// audio sources

		// source for the low acceleration sounds
		private AudioSource m_lowAccel;
		// source for the low deceleration sounds
		private AudioSource m_lowDecel;
		// source for the high acceleration sounds
		private AudioSource m_highAccel;
		// source for the high acceleration sounds
		private AudioSource m_highDecel;

		// reference to hover controller
		private HoverboardController m_controller;


		void Awake()
		{
			// get the hover controller (this will not be null as we have require component)
			m_controller = GetComponent<HoverboardController> ();
		}


		private void StartSound()
		{

			// setup the simple audio source
			m_highAccel = SetupEngineAudioSource (highAccelClip);

			// if we have four channel audio setup the four audio sources
			if (engineSoundType == EngineSoundType.FourChannel)
			{
				m_lowAccel = SetupEngineAudioSource (lowAccelClip);
				m_lowDecel = SetupEngineAudioSource (lowDecelClip);
			
				m_highDecel = SetupEngineAudioSource (highDecelClip);
			}

			// flag that we started the sounds playing
			m_startSound = true;
		}


		private void StopSound()
		{
			// destroy all audio sources on this object
			foreach (var source in GetComponents<AudioSource>())
			{
				Destroy (source);
			}

			// flag the we have stopped the sounds playing
			m_startSound = false;
		}


		private void Update()
		{
			// get the distance to main camera
			float cameraDist = (Camera.main.transform.position - transform.position).sqrMagnitude;


			if (m_controller.engineOn)
			{
				// stop sound if the object is beyond the maximum roll of distance
				if ((m_startSound && cameraDist > maxRollofDistance * maxRollofDistance)) 
				{
					StopSound ();
				}
				
				// start the sound if not playing and it is never than the maximum distance
				if ((!m_startSound && cameraDist < maxRollofDistance * maxRollofDistance))
				{
					StartSound ();
				}
			} else {
				StopSound ();
			}

			if (m_startSound) {
				// the pitch is interpolated between the min and max values, according to the car's revs.
				// float pitch = Mathf.Lerp(lowPitchMin, Mathf.Clamp(Mathf.Abs(m_controller.gasInput) + Mathf.Lerp(0f, 0.25f, m_controller.speed / m_controller.maxSpeed), .45f, 1.5f), Time.deltaTime * 2f);

				float pitch = ULerp (lowPitchMin, lowPitchMax, Mathf.Clamp (Mathf.Abs (m_controller.gasInput) + Mathf.Lerp (0f, 0.25f, m_controller.speed / m_controller.maxSpeed), 0.45f, 1.5f));
				pitch = Mathf.Min (lowPitchMax, pitch);


				// simple engine
				if (engineSoundType == EngineSoundType.Simple) {
					m_highAccel.pitch = pitch * pitchMultiplier * pitchMultiplier;
					m_highAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
					m_highAccel.volume = Mathf.Lerp (m_highAccel.volume, 1f, Time.deltaTime * 0.5f);
				} else {
					// 4 channel engine sound

					// adjustr the pitches based on the multipliers
					m_lowAccel.pitch = pitch * pitchMultiplier;
					m_lowDecel.pitch = pitch * pitchMultiplier;
					m_highAccel.pitch = pitch * pitchMultiplier * pitchMultiplier;
					m_highDecel.pitch = pitch * pitchMultiplier * pitchMultiplier;

					// get values for fading the sounds based on the acceleration
					float accFade = Mathf.Abs (Mathf.Clamp (m_controller.gasInput, 0f, 1f));
					float decFade = 1 - accFade;

					// get the high fade value based on the cars revs
					float highFade = Mathf.InverseLerp(0.2f, 0.8f, Mathf.Clamp (m_controller.gasInput, 0f, 1f));
					float lowFade = 1 - highFade;

					// adjust the values to be more realistic
					highFade = 1 - ((1 - highFade) * (1 - highFade));
					lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
					accFade = 1 - ((1 - accFade) * (1 - accFade));
					decFade = 1 - ((1 - decFade) * (1 - decFade));

					// adjus the source bolumes based on the fade values
					m_lowAccel.volume = lowFade * accFade;
					m_lowDecel.volume = lowFade * decFade;


					m_highAccel.volume = highFade * accFade;
					m_highDecel.volume = highFade * decFade;

					// adjust the doppler levels
					m_highAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
					m_lowAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;

					m_highDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
					m_lowDecel.dopplerLevel = useDoppler ? dopplerLevel : 0;
				}
			}
		}


		// sets up and adds new audio source to the game object
		private AudioSource SetupEngineAudioSource(AudioClip clip, bool loop = true)
		{
			// create the new audio source component on the game object and setup its properties
			AudioSource source = gameObject.AddComponent<AudioSource>();
			source.clip = clip;
			source.volume = 0;
			source.loop = loop;

			// start the clip from a random point
			source.time = Random.Range(0f, clip.length);
			source.Play ();
			source.minDistance = 5;
			source.maxDistance = maxRollofDistance;
			source.dopplerLevel = 0;

			return source;
		}


		// unclamped versions of Lerp and inverse Lerp, to allow value to exceed the from-to range
		private static float ULerp (float from, float to, float value)
		{
			return (1.0f - value) * from - value * to;
		}
	}
}