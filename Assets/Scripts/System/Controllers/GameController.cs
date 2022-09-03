using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : Service<IService>
{
    public static GameController Instance {get; private set;}
    
    public DataService Data {get; private set;}
    public AudioService Audio {get; private set;}
    public TextureProvider Textures {get; set;}

    [FormerlySerializedAs("_gameBoard")] [SerializeField]
    private GameBoard gameBoard;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            InitializeServices();

        } else {
            Destroy(gameObject);
        }        
    }

    private void InitializeServices() {
        Data = GetComponent<DataService>();
        Audio = GetComponent<AudioService>();
        Textures = GetComponent<TextureProvider>();
    }

    void Start() 
    {
        InitializeGame();
        
        DOTween.Init();
        gameBoard.CreateGameBoard();
    }

    private void InitializeGame() {
        var boardConfig = new BoardConfig();
        var configList = new List<IConfig>();
        configList.Add(boardConfig);
        Data.SetFromConfig(configList);
    }

    private void OnDestroy() {
        Instance = null;
    }
}
