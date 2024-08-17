using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController m_Instance;

    public static GameController GetInstance() { return m_Instance; }

    private void Awake()
    {
        if(m_Instance == null)
        {
            m_Instance = this;
        }
        else if(m_Instance != this)
        {
            Destroy(this);
            Debug.LogWarning(gameObject.name + " had an extra GameController, which was deleted");
        }
    }
}
