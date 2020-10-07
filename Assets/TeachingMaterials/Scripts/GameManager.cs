using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //Player
    public PlayerManager _Player;
    //WaveManager
    //GameUI
    //public GameMenu _GameUI;
    //SceneObjManager(reset game)
    //ScoreManager?

    public bool _HasStarted = false;

    private void Awake()
    {
        if (!_instance)
            _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        _HasStarted = true;
    }
}
