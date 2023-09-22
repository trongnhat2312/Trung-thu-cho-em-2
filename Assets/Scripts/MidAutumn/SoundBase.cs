using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundBase : MonoBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SoundBase.Instance = this;
	}

	private void Update()
	{
	}

	public static SoundBase Instance;

	public AudioClip click;

	public AudioClip checkIn;

	public AudioClip pieceDisappear;

	public AudioClip starPartsCombine;

	public AudioClip ChiecDenOngSaoPiano;
}
