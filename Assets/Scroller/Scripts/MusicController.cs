﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

	public AudioClip levelSong, fightSong, bossSong, levelClearSong;

	private AudioSource audioS;

	// Use this for initialization
	void Start () {

		audioS = GetComponent<AudioSource>();
		PlaySong(levelSong);

	}
	
	

	public void PlaySong(AudioClip clip)
	{
		audioS.clip = clip;
		audioS.volume = (float)0.5;
		audioS.Play();
	}
}
