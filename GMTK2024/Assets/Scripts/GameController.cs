using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 gameAreaSize;

    [Header("Important Stuff")]
    [SerializeField] private GameObject mainEnemy;
    
    public GameObject MainEnemy { get { return mainEnemy; } }

    private static GameController instance;
    public static GameController Instance { get { return instance; } }

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