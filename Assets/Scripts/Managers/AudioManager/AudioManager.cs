using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameStateMachine gameState;
    private int currentSong;
    private int randomIndex;

    [Header("AudioMixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("AudioSource Mains")]
    [SerializeField] private AudioSource uiSoundSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("AudioSource SFX childs")]
    [SerializeField] private AudioSource runSource;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource airSpinSource;
    [SerializeField] private AudioSource slamDownLandSource;
    [SerializeField] private AudioSource groundSlideSource;
    [SerializeField] private AudioSource keyCollectSource;

    [Header("Menu Music")]
    [SerializeField] AudioClip mainMenuSong;
    
    [Header("UI")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    [Header("Player_SFX")]
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip airSpinSound;
    [SerializeField] private AudioClip[] jumpVoiceSoundList;
    [SerializeField] private AudioClip slamDownLandSound;
    [SerializeField] private AudioClip groundSlideSound;

    [Header("SFX world")]
    [SerializeField] private AudioClip keyCollectSound;

    [Header("Music in game")]
    [SerializeField] private AudioClip[] inGameSongs;

    public static AudioManager Instance;

    // === SUBSCRIBE & UNSUBSCRIBE EVENTS === //
    void OnEnable()
    {
        GameStateMachine.OnStateExit += ExitState;
        GameStateMachine.OnStateEnter += EnterState;
        PlayerScript.OnPlayerDeath += DeathSound;
    }
    
    // ---------------------------- //
    void OnDisable()
    {
        GameStateMachine.OnStateExit -= ExitState;
        GameStateMachine.OnStateEnter -= EnterState;
        PlayerScript.OnPlayerDeath -= DeathSound;
    }

    // === START & AWAKE === //
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    // ---------------------------- //
    void Update()
    {
        MusicUpdate();
    }

    // === EVENT METHODS === //
    public void DeathSound()
    {
        StartCoroutine(PitchFade());
    }

    // === GAMESTATE === //
    void EnterState(GameState newState)
    {
        switch(newState)
        {
            case GameState.Paused:
            case GameState.LevelComplete:
                LowpassFilterMusic(true);
                break;
        }
    }
    
    // ---------------------------- //
    void ExitState(GameState newState)
    {
        switch(newState)
        {
            case GameState.Paused:
            case GameState.LevelComplete:
                LowpassFilterMusic(false);
                break;
        }
    }

    // === MUSIC === //
    void MusicUpdate()
    {
        switch(gameState.currentState)
        {
            case GameState.Playing:
                PickAndPlayRandomSong();
                break;
            case GameState.MainMenu:
                PlayMainMenuMusic();
                break;
        }
    }

    // --- IN GAME RANDOM SONG PICKER --- //
    void PickAndPlayRandomSong()
    {
        if (!musicSource.isPlaying || musicSource.clip == mainMenuSong)
        {
            while(randomIndex == currentSong)
            {
                randomIndex = UnityEngine.Random.Range(0, inGameSongs.Length);  
            }

            currentSong = randomIndex;
            musicSource.clip = inGameSongs[currentSong];
            musicSource.Play();  
        }      
    }

    // --- MAIN MENU MUSIC --- //
    void PlayMainMenuMusic()
    {
        if (musicSource.clip == mainMenuSong && musicSource.isPlaying )
        {
            return;
        } else
        {
            musicSource.clip = mainMenuSong;
            musicSource.Play();          
        }
    }

    // --- SELECT A NEW SONG --- //
    public void SelectNewSong()
    {
        while(randomIndex == currentSong)
        {
            randomIndex = UnityEngine.Random.Range(0, inGameSongs.Length);
        }

        currentSong = randomIndex;
        musicSource.clip = inGameSongs[currentSong];
        musicSource.Play();
    }

    // === UI SOUND METHODS === //

    // --- HOVER --- // 
    public void PlayHoverSound()
    {
        uiSoundSource.PlayOneShot(hoverSound);
    }

    // --- CLICK --- //
    public void PlayClickSound()
    {
        uiSoundSource.PlayOneShot(clickSound);
    }


    // === SFX PLAYER METHODS === //
    public void PlayRunSound()
    {

        int randomIndexFootstep = UnityEngine.Random.Range(0, footstepSounds.Length);
        runSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

        AudioClip footstepSound = footstepSounds[randomIndexFootstep];
        runSource.PlayOneShot(footstepSound);
    }

    // ------------------------------ //
    public void PlayJumpSound()
    {

        int randomJumpVoiceIndex = UnityEngine.Random.Range(0, jumpVoiceSoundList.Length);
        voiceSource.pitch = UnityEngine.Random.Range(1f, 1.1f);

        AudioClip jumpVoice = jumpVoiceSoundList[randomJumpVoiceIndex];
        voiceSource.PlayOneShot(jumpVoice);
    }
    
    // ------------------------------ //
    public void PlayAirSpinSound()
    {
        airSpinSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
        airSpinSource.PlayOneShot(airSpinSound);
    }

    // ------------------------------ //
    public void PlaySlamDownLand()
    {

        slamDownLandSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
        slamDownLandSource.PlayOneShot(slamDownLandSound);
    }

    // ------------------------------ //
    public void PlayGroundSlideLoopSound()
    {

        if(!groundSlideSource.isPlaying)
        {
            groundSlideSource.loop = true;
            groundSlideSource.clip = groundSlideSound;
            groundSlideSource.Play();     
        }
    }

    // ------------------------------ //
    public void StopGroundSlideSound()
    {

        if (groundSlideSource.isPlaying)
        {
            groundSlideSource.Stop();
        }   
    }

    // === sfx for world (only key collect) === //
    public void PlayCollectKeySound()
    {

        keyCollectSource.PlayOneShot(keyCollectSound);
    }

    // === AUDIO MIXER METHODS === //
    void LowpassFilterMusic(bool active)
    {
        float lowpassNumber = active ? 300f : 22000f;
        audioMixer.SetFloat("musicLowpass", lowpassNumber);
    }

    // Ai har laget denne metoden (PitchFade)
    private IEnumerator PitchFade()
        {
            float duration = 1.0f;
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                musicSource.pitch = Mathf.Lerp(1.0f, 0.5f, timer / duration);
                yield return null;
            }
            
            musicSource.pitch = 0.5f;

            timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                musicSource.pitch = Mathf.Lerp(0.5f, 1.0f, timer / duration);
                yield return null;
            }

            musicSource.pitch = 1.0f;
        }   
}
