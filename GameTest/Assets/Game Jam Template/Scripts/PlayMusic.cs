using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum MusicChoice { titleMusic, middleburg, BanditForest, Easton, Swamp, BeforeBog, Bog, Boss, Credits, None, Null }

public class PlayMusic : MonoBehaviour {


	public AudioClip titleMusic;					//Assign Audioclip for title music loop
	public AudioClip middleburg;					//Assign Audioclip for middleburg area; 'Lital Village'
    public AudioClip BanditForest;                  // 'Bad Zone'
    public AudioClip Easton;                        // 'Biiga Town'
    public AudioClip Swamp;                         // 'Sickly Forest'
    public AudioClip BeforeBog;                     // 'Humming in the Rain'
    public AudioClip Bog;                           // 'Ominous Bog'
    public AudioClip Boss;                          // 'Fight Me'
    public AudioClip Credits;                       // 'Where Credit Is Due'
	public AudioMixerSnapshot volumeDown;			//Reference to Audio mixer snapshot in which the master volume of main mixer is turned down
	public AudioMixerSnapshot volumeUp;				//Reference to Audio mixer snapshot in which the master volume of main mixer is turned up


	private AudioSource musicSource;				//Reference to the AudioSource which plays music
	private float resetTime = .01f;					//Very short time used to fade in near instantly without a click


	void Awake () 
	{
		//Get a component reference to the AudioSource attached to the UI game object
		musicSource = GetComponent<AudioSource> ();
		//Call the PlayLevelMusic function to start playing music
	}


	public void PlayLevelMusic()
	{
		//This switch looks at the last loadedLevel number using the scene index in build settings to decide which music clip to play.
		switch (SceneManager.GetActiveScene().buildIndex)
		{
			//If scene index is 0 (usually title scene) assign the clip titleMusic to musicSource
			case 0:
				musicSource.clip = titleMusic;
				break;
			//If scene index is 1 (usually main scene) assign the clip middleburg to musicSource
			case 1:
				musicSource.clip = middleburg;
				break;
		}
		//Fade up the volume very quickly, over resetTime seconds (.01 by default)
		FadeUp (resetTime);
		//Play the assigned music clip in musicSource
		musicSource.Play ();
	}

    //Used if running the game in a single scene, takes an integer music source allowing you to choose a clip by number and play.
    // public void PlaySelectedMusic(int musicChoice)
    public void PlaySelectedMusic(MusicChoice theChoice)
    {
        
        if (theChoice == MusicChoice.None) {
            musicSource.Stop();
            return;
        }

        
        // MusicChoice theChoice = (MusicChoice) musicChoice;

		//This switch looks at the integer parameter musicChoice to decide which music clip to play.
		switch (theChoice) 
		{
            //if musicChoice is 0 assigns titleMusic to audio source
            case MusicChoice.titleMusic:
                musicSource.clip = titleMusic;
                break;
            //if musicChoice is 1 assigns middleburg to audio source
            case MusicChoice.middleburg:
                musicSource.clip = middleburg;
                break;
            case MusicChoice.BanditForest:
                musicSource.clip = BanditForest;
                break;
            case MusicChoice.Easton:
                musicSource.clip = Easton;
                break;
            case MusicChoice.Swamp:
                musicSource.clip = Swamp;
                break;
            case MusicChoice.BeforeBog:
                musicSource.clip = BeforeBog;
                break;
            case MusicChoice.Bog:
                musicSource.clip = Bog;
                break;
            case MusicChoice.Boss:
                musicSource.clip = Boss;
                break;
            case MusicChoice.Credits:
                musicSource.clip = Credits;
                break;
		}
		//Play the selected clip
		musicSource.Play ();
	}

	//Call this function to very quickly fade up the volume of master mixer
	public void FadeUp(float fadeTime)
	{
		//call the TransitionTo function of the audioMixerSnapshot volumeUp;
		volumeUp.TransitionTo (fadeTime);
	}

	//Call this function to fade the volume to silence over the length of fadeTime
	public void FadeDown(float fadeTime)
	{
		//call the TransitionTo function of the audioMixerSnapshot volumeDown;
		volumeDown.TransitionTo (fadeTime);
	}
}
