using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    public Texture2D cursor;

    //public AudioMixer audioMixer;

    public Animator idleAnim;
    public AudioSource menuAudioSource;

    public AudioClip menuMusic;
    public AudioClip fightMusic;
    public AudioClip corridorMusic;
    public AudioClip bossMusic;

    public GameObject[] childrens;

    public bool pauseGame;
    public bool invertY;

    public float currentSensitivity = 2f;
    [Range(0, 2)] public int currentQualityLevel = 2;

    float normalTimeScale;

    public event System.Action OnPausePress;

    public static MainMenuManager instance;

    private bool gameOver;

    public bool GameOver { get { return gameOver; } set { gameOver = value; } }

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        float xSpot = cursor.width / 2;
        float ySpot = cursor.height / 2;
        Vector2 hotSpot = new Vector2(xSpot, ySpot);

        Cursor.SetCursor(cursor, hotSpot, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //qualityDropdown.value = currentQualityLevel;
        //sensitivitySlider.value = currentSensitivity;

        SceneManager.sceneLoaded += OnSceneLoaded;

        menuAudioSource.clip = menuMusic;
        menuAudioSource.Play();
    }

    
    // private void Update()
    // {
    //     if (SceneManager.GetActiveScene().buildIndex != 0)
    //     {
    //         if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
    //         {
    //             Pause();
    //         }
    //     }
    // }

    public void SetMouseInvert(bool invert)
    {
        invertY = invert;
    }

    // Use this for initialization
    public void StartGame()
    {
        Cursor.visible = false;
        SceneManager.LoadScene(1);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void SetSensitivity(float sensitivity)
    {
        currentSensitivity = sensitivity;
    }

    public void SetVolume(float volume)
    {
        //audioMixer.SetFloat("Volume", volume);
    }

    public void Pause()
    {
        if (OnPausePress != null)
            OnPausePress();

        pauseGame = !pauseGame;
        //pauseScreen.SetActive(pauseGame);

        if (pauseGame)
        {
            normalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = normalTimeScale;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene == SceneManager.GetSceneByBuildIndex(0) )
        {
            if(GameOver)
            {
                foreach (GameObject child in childrens)
                    child.SetActive(true);

                //idleAnim["Idle"].speed = 45f;
                idleAnim = FindObjectOfType<Animator>();
                idleAnim.speed = 20f;

                //menuAudioSource = GetComponent<AudioSource>();
                menuAudioSource.clip = menuMusic;
                menuAudioSource.volume = 0.7f;
                menuAudioSource.pitch = 2f;
                menuAudioSource.Play();

                RoomTrigger.OnPlayerEnterRoom -= ChangeToFightMusic;
                //RoomTrigger.OnPlayerExitRoom -= ChangeToCorridorMusic;
                PlayerHealth.OnPlayerRespawn -= ChangeToCorridorMusic;
                EnemySpawner.OnAllEnemyDeadInRoom -= ChangeToCorridorMusic;
            }
        }
        else if (scene == SceneManager.GetSceneByBuildIndex(1))
        {
            GameOver = false;
            menuAudioSource.pitch = 1f;
            menuAudioSource.volume = 0.2f;
            menuAudioSource.clip = corridorMusic;
            menuAudioSource.Play();

            RoomTrigger.OnPlayerEnterRoom += ChangeToFightMusic;
            //RoomTrigger.OnPlayerExitRoom += ChangeToCorridorMusic;
            PlayerHealth.OnPlayerRespawn += ChangeToCorridorMusic;
            EnemySpawner.OnAllEnemyDeadInRoom += ChangeToCorridorMusic;
        }
        
    }

    void ChangeToFightMusic(RoomIndex index)
    {
        if (menuAudioSource.clip != fightMusic && index != RoomIndex.Fifth)
        {
            menuAudioSource.pitch = 1f;
            menuAudioSource.volume = 0.5f;
            menuAudioSource.clip = fightMusic;
            menuAudioSource.Play();
        }
        else
        {
            menuAudioSource.pitch = 1f;
            menuAudioSource.volume = 0.5f;
            menuAudioSource.clip = bossMusic;
            menuAudioSource.Play();
        }
    }

    void ChangeToCorridorMusic()
    {
        if (menuAudioSource.clip != corridorMusic && !GameManager.instance.GameIsOver)
        {
            menuAudioSource.pitch = 1f;
            menuAudioSource.volume = 0.5f;
            menuAudioSource.clip = corridorMusic;
            menuAudioSource.Play();
        }
    }

    void TransitionBetweenSongs()
    {

    }

}