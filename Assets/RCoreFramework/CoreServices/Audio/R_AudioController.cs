using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class AudioFile
{	
	public AudioSource source { get; private set; }

	[SerializeField] private AudioClip clip = null;
	[SerializeField] [Range(0, 100)] private int volume = 0;
	private uint id = 0;

	public AudioFile() { }

	public AudioFile(AudioSource source)
	{
		this.source = source;
		this.source.playOnAwake = false;
		this.source.Stop();
	}

	public AudioFile(AudioSource source, uint id)
	{
		this.source = source;
		this.source.playOnAwake = false;
		this.source.Stop();
		this.id = id;
	}

	public AudioFile(AudioClip clip)
	{
		this.clip = clip;
		//Aqui Alex tiene su Idgenerator
		//this.id = IdGenerator.GetNewAudioId();
		this.id = 0;
	}

	public AudioFile(AudioSource source, AudioClip clip, float volume, float pitch, bool repeat, uint id)
	{
		this.source = source;
		this.source.playOnAwake = false;
		this.source.volume = volume;
		this.source.pitch = pitch;
		this.source.clip = clip;
		this.clip = clip;
		this.source.loop = repeat;
		this.source.Stop();
		this.id = id;
	}

	public void ConfigAudioFile(AudioSource source, AudioClip clip, float volume, float pitch, bool repeat)
	{
		this.source = source;
		this.source.playOnAwake = false;
		this.source.volume = volume;
		this.source.pitch = pitch;
		this.source.clip = clip;
		this.clip = clip;
		this.source.loop = repeat;
		this.source.Stop();
	}

	public void ConfigAudioFile(AudioSource source, AudioClip clip, float volume, float pitch, bool repeat, uint id)
	{
		this.source = source;
		this.source.playOnAwake = false;
		this.source.volume = volume;
		this.source.pitch = pitch;
		this.source.clip = clip;
		this.clip = clip;
		this.source.loop = repeat;
		this.source.Stop();
		this.id = id;
	}

	public void Play()
	{
		source.Play();
	}

	public void Stop()
	{
		source.volume = 0f;
		source.Stop();
	}

	public void Pause()
	{
		source.Pause();
	}

	public void Resume()
	{
		source.UnPause();
	}

	public void Mute()
	{
		source.mute = true;
	}

	public void UnMute()
	{
		source.mute = false;
	}

	public AudioClip GetClip()
	{
		return clip;
	}

	public string Name()
	{
		//get { return clip != null ? clip.name : string.Empty; }
		return clip != null ? clip.name : string.Empty;

	}

	public bool IsPlaying()
	{
		return source.isPlaying ? true : false;
	}

	public bool IsValid()
	{
		return source != null ? true : false;
	}

	public float GetVolume()
	{
		return volume / 100f;
	}

	public float GetVolumeClip()
	{
		return source.volume;
	}

	public void SetId(uint id)
	{
		this.id = id;
	}

	public uint GetId()
	{
		return id;
	}

	public float GetDuration()
	{
		return clip != null ? clip.length : 0f;
	}
}

public class R_AudioController : MonoBehaviour {

	private List<AudioFile> listAudioFile = new List<AudioFile>();
	private List<AudioFile> listMusicFile = new List<AudioFile>();

	public int maximunSoundsInPool = 6;
	public bool soundsActive = false;
	public bool musicActive = false;

	private GameObject goHierarchySounds = null; //to store all the sounds
	private int totalSounds = 0;

	/*
	private uint timerFadeMusicOutId = 0;
	private uint timerFadeMusicInId = 0;
	private uint timerFadeSoundInId = 0;
	private uint timerFadeSoundOutId = 0;
	private uint timerFadeMusicToVolumeId = 0;
	private AudioFile tmpMusicFadeIn = null;
	private AudioFile tmpMusicFadeOut = null;
	private AudioFile tmpSoundFadeIn = null;
	private AudioFile tmpSoundFadeOut = null;
	*/

    #region init
	private void OnDestroy()
	{
		//Aqui deberiamos pensar si queremos limpiar algo
	}
    #endregion

    #region load sounds and music

	public void InitController()
	{
		goHierarchySounds = new GameObject("Sounds2D");
		goHierarchySounds.transform.position = Vector3.zero;
		DontDestroyOnLoad(goHierarchySounds);

		for(var i = 0; i < maximunSoundsInPool; ++i)
		{
			CreateNewAudioFile();
		}
	}

	internal void StopAllMusic()
	{
		Debug.Log("te falta este metodo");
		throw new NotImplementedException();
	}

	private AudioFile CreateNewAudioFile()
	{
		var sound = new AudioFile(goHierarchySounds.AddComponent<AudioSource>());
		listAudioFile.Add(sound);
		totalSounds = listAudioFile.Count;
		return sound;
	}

	private AudioFile CreateNewAudioFile(uint id)
	{
		var sound = new AudioFile(goHierarchySounds.AddComponent<AudioSource>(), id);
		listAudioFile.Add(sound);
		totalSounds = listAudioFile.Count;
		return sound;
	}

	private void LoadSound(AudioFile clip, float volume, float pitch, bool repeat)
	{
		var audio = CreateNewAudioFile(clip.GetId());
		audio.ConfigAudioFile(audio.source, clip.GetClip(), volume, pitch, repeat);
		audio.Play();
	}

	public void LoadMusic(AudioFile clip, float volume, bool repeat)
	{
		LoadMusic(clip, volume, 1.0f, repeat);
	}

	private void LoadMusic(AudioFile clip, float volume, float pitch, bool repeat)
	{
		var go = new GameObject(string.Format("Music_{0}", clip.Name()));
		go.transform.position = Vector3.zero;
		DontDestroyOnLoad(go);

		//No estoy seguro de que esto funcione como debe
		var sound = new AudioFile((AudioSource) go.AddComponent(typeof(AudioSource)), clip.GetClip(), volume,
			pitch,
			repeat, clip.GetId());
			
		listMusicFile.Add(sound);

	}

	internal void PlaySound(AudioFile sound, float finalVolume, float v)
	{
		
		PlaySound(sound, finalVolume, v, false);
	}

	#endregion

	#region play sounds/music

	public void PlaySound(AudioFile clip)
	{
		PlaySound(clip, 1f, 1f, false);
	}

	private void PlaySound(AudioFile clip, float volume, float pitch, bool repeat)
	{
		if (!soundsActive)
			return;

		var sourceFound = false;

		//Buscamos un slot vacio
		for(var i = 0; i < totalSounds; i++)
		{
			if (listAudioFile[i].source.isPlaying) continue;

			listAudioFile[i].ConfigAudioFile(listAudioFile[i].source, clip.GetClip(), volume, pitch, repeat, clip.GetId());
			listAudioFile[i].Play();
			Debug.Log("Sonando......");
			sourceFound = true;
			break;
		}

		//Si no tenemos slots vacios, somos retrasados y no hemos calculado bien el tamaño de la pool,
		//pero no nos podemos quedar sin reproducir sonido, asi que hemos de crear uno y pagar el precio
		//en memoria
		if (!sourceFound)
		{
			LoadSound(clip, volume, pitch, repeat);
		}
	}

	public void PlayMusic(AudioFile anAudio)
	{
		if (!musicActive) return;

		for(int i = 0, max = listMusicFile.Count; i < max; ++i)
		{
			var tmpAudio = listMusicFile[i];

			if(tmpAudio.GetId() == anAudio.GetId())
			{
				tmpAudio.Play();
				break;
			}
		}
	}

	public void PlayMusic(AudioFile anAudio, float aVolume)
	{
		if (!musicActive) return;

		for (int i = 0, max = listMusicFile.Count; i < max; ++i)
		{
			var tmpAudio = listMusicFile[i];

			if (tmpAudio.GetId() == anAudio.GetId())
			{
				tmpAudio.source.volume = aVolume;
				tmpAudio.Play();
				break;
			}
		}
	}

    #endregion

    #region pause and resume music

	public void PauseMusic(AudioFile anAudio)
	{
		for(int i = 0, max = listMusicFile.Count; i < max; i++)
		{
			var tmpAudio = listMusicFile[i];

			if(tmpAudio.GetId() == anAudio.GetId())
			{
				tmpAudio.Pause();
				break;
			}
		}
	}

	public void ResumeMusic(AudioFile anAudio)
	{
		for (int i = 0, max = listMusicFile.Count; i < max; i++)
		{
			var tmpAudio = listMusicFile[i];

			if (tmpAudio.GetId() == anAudio.GetId())
			{
				tmpAudio.Resume();
				break;
			}
		}
	}

    #endregion


}
