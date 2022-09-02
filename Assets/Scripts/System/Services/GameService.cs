using System;
using System.Collections.Generic;
using UnityEngine;

public class GameService : Service<IService>
{
    public DataService Data {get; private set;}
    public AudioService Audio {get; private set;}

    public TextureProvider Textures {get; set;}

    void Awake()
    {
        InitializeServices();
    }

    private void InitializeServices() {
        Data = GetComponent<DataService>();
        Audio = GetComponent<AudioService>();
        Textures = GetComponent<TextureProvider>();
    }
}
