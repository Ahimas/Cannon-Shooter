using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ControlType
{
    Mouse,
    Arrows
}


public class GameManager : MonoBehaviour
{
    public AudioManager audioManager;

    public int enemiesInGame;
    public ControlType control;
    [SerializeField] private float spawnPause = 0.75f;
    [SerializeField] private float wavePause = 1;

    [SerializeField] private Text waveText;
    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject audioMenu;
    [SerializeField] GameObject gameOverMenu;
    
    [SerializeField] private Enemy enemyPrefab;
    private List<Enemy> enemyPool = new List<Enemy>();

    private int waveCounter;
    private float spawnZRangePos = 14f;
    private float spawnXPos = 6f;
    private float spawnYPos = 0.25f;

    private bool isWaveStarted;
    private bool isPaused;

    public bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        waveCounter = 0;
        enemiesInGame = 0;
        isWaveStarted = false;
        isGameOver = true;
        isPaused = false;
        gameScreen.SetActive(false);
        menuScreen.SetActive(true);
        audioMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if ( !isGameOver )
        {
            if (enemiesInGame < 1 && !isWaveStarted)
            {
                isWaveStarted = true;
                StartCoroutine(StartWave());
                
            }

            if ( Input.GetKeyDown(KeyCode.P) )
            {
                if ( !isPaused )
                {
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;

                } else
                {
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.Confined;
                }
                isPaused = !isPaused;
                AudioListener.pause = isPaused;
                pauseMenu.SetActive(isPaused);
                audioMenu.SetActive(isPaused);
                Cursor.visible = isPaused;

            }
        }
        
    }

    public void StartGame()
    {
        isGameOver = false;
        menuScreen.SetActive(false);
        audioMenu.SetActive(false);
        gameScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("CannonShooterScene");
        Time.timeScale = 1f;
        AudioListener.pause = false;
        audioManager.ChangeAudiVolume(1f);
    }

    void WaveCounterUpdate()
    {
        waveCounter += 1;
        waveText.text = "Wave " + waveCounter;
    }

    Vector3 GenerateSpawnPos()
    {
        float spawnZPos = Random.Range(-spawnZRangePos, spawnZRangePos);

        return new Vector3(spawnXPos, spawnYPos, spawnZPos);
    }

    void CreateEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab);

        enemy.gameObject.SetActive(false);
        enemyPool.Add(enemy);
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(wavePause);

        audioManager.PlayNewWaveSound();
        WaveCounterUpdate();

        yield return new WaitForSeconds(wavePause);

        CreateEnemy();

        foreach ( Enemy enemy in enemyPool )
        {
            enemiesInGame += 1;
            enemy.transform.position = GenerateSpawnPos();
            enemy.gameObject.SetActive(true);

            yield return new WaitForSeconds(spawnPause); 
        }

        isWaveStarted = false;
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverMenu.SetActive(true);
        audioManager.PlayEnemyCelebrating();
        audioManager.PlayGameOverMusic();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

}
