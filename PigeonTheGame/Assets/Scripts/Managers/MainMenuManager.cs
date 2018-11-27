using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    public Texture2D cursor;

    public AudioMixer audioMixer;

    public Animator idleAnim;
    public AudioSource menuMusic;

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
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
    }

    public void SetMouseInvert(bool invert)
    {
        invertY = invert;
    }

    // Use this for initialization
    public void StartGame()
    {
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
        audioMixer.SetFloat("Volume", volume);
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
        if (scene == SceneManager.GetSceneByBuildIndex(0) && GameOver)
        {
            foreach(GameObject child in childrens)
                child.SetActive(true);

            //idleAnim["Idle"].speed = 45f;
            idleAnim = FindObjectOfType<Animator>();
            idleAnim.speed = 20f;

            menuMusic = Camera.main.GetComponent<AudioSource>();
            menuMusic.pitch = 2f;
        }
        else
            GameOver = false;
    }

}