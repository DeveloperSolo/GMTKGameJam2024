using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    None,
    Title,
    Play,
    Win,
    Lose
}

public class GameController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float timeLimit;
    [SerializeField] private Vector2 gameAreaSize;

    [Header("Important Stuff")]
    [SerializeField] private CameraScript cameraScript;
    [SerializeField] private GameObject mainEnemy;
    [SerializeField] private Transform environmentRoot;

    [Header("UI")]
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [SerializeField] private TextMeshProUGUI timeText;

    public GameObject MainEnemy { get { return mainEnemy; } }

    private static GameController instance;
    public static GameController Instance { get { return instance; } }

    private GameState currentState = GameState.None;
    private float playTimeElapsed = 0.0f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
            Debug.LogWarning(gameObject.name + " had an extra GameController, which was deleted");
        }
    }

    private void Start()
    {
        currentState = GameState.Play;
        SetState(GameState.Title);
    }

    private void Update()
    {
        float elapsed = Time.deltaTime;
        switch(currentState)
        {
            case GameState.Play:

                UpdatePlayTimeElapsed(playTimeElapsed + elapsed);
                if(playTimeElapsed >= timeLimit)
                {
                    SetState(GameState.Lose);
                }
                break;
        }
    }

    public void SetStateFromButton(string stateString)
    {
        GameState newState = GameState.None;
        if(System.Enum.TryParse(stateString, true, out newState))
        {
            SetState(newState);
            return;
        }
        Debug.LogError(stateString + " NOT VALID STATE");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetState(GameState newState)
    {
        // exit
        switch(currentState)
        {
            case GameState.Title:
                titleScreen.SetActive(false);
                break;

            case GameState.Play:
                HUD.SetActive(false);
                cameraScript.enabled = false;
                for (int i = 0; i < environmentRoot.childCount - 1; ++i)
                {
                    environmentRoot.GetChild(i).gameObject.SetActive(false);
                }
                break;

            case GameState.Win:
                winScreen.SetActive(false);
                break;
            case GameState.Lose:
                loseScreen.SetActive(false);
                break;
        }
        // enter
        switch(newState)
        {
            case GameState.Title:
                titleScreen.SetActive(true);
                break;

            case GameState.Play:
                HUD.SetActive(true);
                cameraScript.enabled = true;
                for(int i = 0; i < environmentRoot.childCount - 1; ++i)
                {
                    environmentRoot.GetChild(i).gameObject.SetActive(true);
                }
                UpdatePlayTimeElapsed(0.0f);
                break;

            case GameState.Win:
                winScreen.SetActive(true);

                Transform timeToSpareText = winScreen.transform.Find("Time_text");
                if(timeToSpareText != null)
                {
                    timeToSpareText.GetComponent<TextMeshProUGUI>().text = "with " + GetTimeText(timeLimit - playTimeElapsed) + " to spare!";
                }
                break;
            case GameState.Lose:
                loseScreen.SetActive(true);
                break;
        }
        currentState = newState;
    }

    private void UpdatePlayTimeElapsed(float newTime)
    {
        playTimeElapsed = newTime;
        timeText.text = GetTimeText(timeLimit - playTimeElapsed) + " left";
    }

    private string GetTimeText(float time)
    {
        int min = Mathf.FloorToInt(time / 60.0f);
        float seconds = time - (min * 60);
        string text = seconds.ToString("F1") + "s";
        if (min > 0)
        {
            text = min + " min, " + text;
        }
        return text;
    }

    public Vector3 ClampToGameArea(Vector3 position, Vector2 size)
    {
        Vector2 limits = (gameAreaSize - size) / 2.0f;

        position.x = Mathf.Clamp(position.x, -limits.x, limits.x);
        position.y = Mathf.Clamp(position.y, -limits.y, limits.y);

        return position;
    }

    public bool IsOutOfGameArea(Vector3 position)
    {
        Vector2 limits = gameAreaSize / 2.0f;
        return position.x < -limits.x || position.x > limits.x
            || position.y < -limits.y || position.y > limits.y;
    }

    public Vector2 GetRandomPositionInGameArea(Vector2 size)
    {
        Vector2 limits = (gameAreaSize - size) / 2.0f;
        return new Vector2(Random.Range(-limits.x, limits.x), Random.Range(-limits.y, limits.y));
    }

    public static bool GetMouseButtonDown(MouseButton button)
    {
        return Input.GetMouseButtonDown((int)button);
    }
    public static bool GetMouseButton(MouseButton button)
    {
        return Input.GetMouseButton((int)button);
    }
    public static bool GetMouseButtonUp(MouseButton button)
    {
        return Input.GetMouseButtonUp((int)button);
    }
}

public enum MouseButton
{
    Left = 0,
    Right = 1,
    Middle = 2
}