using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public static GameController Instance {get; private set;}

    [FormerlySerializedAs("_gameService")] [SerializeField]
    private GameService gameService;

    public GameService GameService { get { return gameService; }}

    [FormerlySerializedAs("_gameBoard")] [SerializeField]
    private GameBoard gameBoard;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;

        } else {
            Destroy(gameObject);
        }        
    }

    void Start() 
    {
        InitializeGame();

        gameBoard.CreateGameBoard();
    }

    private void InitializeGame() {
        var boardConfig = new BoardConfig();
        var configList = new List<IConfig>();
        configList.Add(boardConfig);
        GameService.Data.SetFromConfig(configList);
    }

    private void OnDestroy() {
        Instance = null;
    }
}
