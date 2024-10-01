using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_SoundsGamePool : MonoBehaviour {

    [SerializeField] private R_AudioController audioController = null;
    [SerializeField] private float masterMusicVolume = 0.5f;
    [SerializeField] private float masterSoundsVolume = 0.5f;

    [Header("Music")]
    [SerializeField] private List<AudioClip> musics = new List<AudioClip>(10);

    [Header("Sounds")]
    [SerializeField] private List<AudioClip> allAudios = new List<AudioClip>(300);

    [Header("Default fallback sounds and some generics")]
    [SerializeField] private AudioFile genericSoundError = null;
    [SerializeField] private AudioFile spawnEnemiesSound = null;

#if UNITY_EDITOR
    [Header("Audio paths")]
    [SerializeField] private string audioPath = null;
#endif

    private List<AudioFile> audioFiles = new List<AudioFile>(200);
    private bool notFoundInPool = false;
    private float clampedVolume = 0f;

    #region init and destroy
    private void OnDestroy() 
    {

        //audioFiles.Clear();
    
    }
    #endregion

    #region public init to be called to prepare everything
    public void InitLoadAllSounds()
    {
        audioController.InitController();

        for(var i = 0; i < allAudios.Count; ++i)
        {
            var sound = new AudioFile(allAudios[i]);
            audioFiles.Add(sound);
        }

        //Imagino que esto es para evitar basura y recuperar memoria
        //de una doble lista
        allAudios.Clear();

        SetId(genericSoundError);
        SetId(spawnEnemiesSound);
    }

    public void SetId(AudioFile anAudio)
    {
        for(var i = 0; i < audioFiles.Count; ++i)
        {
            if (audioFiles[i].Name().Equals(anAudio.Name()))
            {
                anAudio.SetId(audioFiles[i].GetId());
            }
        }
    }

    public void LoadMusic(string aName, bool repeat)
    {
        bool foundMusic = false;

        for(var i = 0; i < audioFiles.Count; ++i)
        {
            if (audioFiles[i].Name().Equals(aName))
            {
                foundMusic = true;
                break;
            }
        }

        if (foundMusic)
        {
            return;
        }

        for(var i = 0; i < musics.Count; ++i)
        {
            if (musics[i].name.Equals(aName))
            {
                var sound = new AudioFile(musics[i]);
                audioController.LoadMusic(sound, masterMusicVolume, repeat);
                audioFiles.Add(sound);
                SetId(sound);
                break;
            }
        }
    }
    #endregion

    private AudioFile GetAudio(AudioFile audio, out bool notFound)
    {
        for(var i = 0; i < audioFiles.Count; ++i)
        {
            if(audioFiles[i].GetId() == audio.GetId())
            {
                notFound = false;
                return audioFiles[i];
            }
        }

        notFound = true;
        return null;
    }

    private AudioFile GetAudio(uint audioId, out bool notFound)
    {
        for(var i = 0; i < audioFiles.Count; ++i)
        {
            if(audioFiles[i].GetId() == audioId)
            {
                notFound = false;
                return audioFiles[i];
            }
        }

        notFound = true;
        return null;
    }

    private AudioFile GetAudio(string nameAudio, out bool notFound)
    {
        for(var i = 0; i < audioFiles.Count; ++i)
        {
            if (audioFiles[i].Name().Equals(nameAudio))
            {
                notFound = false;
                return audioFiles[i];
            }
        }

        notFound = true;
        return null;
    }

    #region enable disable music and sound

    public void EnableMusic()
    {
        audioController.musicActive = true;
    }

    public void DisableMusic()
    {
        audioController.musicActive = false;
        audioController.StopAllMusic();
    }

    public void EnableSounds()
    {
        audioController.soundsActive = true;
    }

    public void DisableSounds()
    {
        audioController.soundsActive = false;
    }

    #endregion

    #region change volume settings

    #endregion

    #region play sounds

    public void PlaySound(AudioFile sound, float volume = 0f, bool repeat = false)
    {
        PlaySound(sound, volume, 1f, repeat);
    }

    public void PlaySound(string name, float volume = 0f)
    {
        var sound = GetAudio(name, out notFoundInPool);

        if (!notFoundInPool)
        {
            var finalVolume = 0f;

            //no volume passed, we use the system defined volume
            if(volume == 0f)
            {
                finalVolume = masterSoundsVolume;
            }
            else
            {
                //divide the number by the max volume (yes, divide by mutiplying)
                //imagino que la multiplicacion es por optimizar
                finalVolume = volume * masterSoundsVolume;
            }

            audioController.PlaySound(sound, finalVolume, 1f);
        }
        else
        {
            Debug.LogError("sound NOT found in the pool " + name);
        }
    }

    public void PlaySound(AudioFile sound, float volume = 0f, float pitch = 1f, bool repeat = false)
    {
        if(sound.GetId() == 0)
        {
            return;
        }

        var soundFound = GetAudio(sound, out notFoundInPool);
        
        audioController.PlaySound(soundFound, volume, 1f);
    }

    #endregion

}
