using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public enum GameState { running,win,passed,lose,pause,quit}
    public List<Enemy> enemies;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player.playerState == Player.States.Die)
        {
            SceneManager.LoadScene("Restart");
        }
    }
}
