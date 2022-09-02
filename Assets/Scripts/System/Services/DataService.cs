using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataService : Service<IData> {

    public BoardData BoardData {get; private set;}

    public void SetFromConfig(List<IConfig> dataConfigList) {
        foreach (var dataConfig in dataConfigList)
        {
            GetServiceOfType(dataConfig.GetType()).SetFromConfig(dataConfig);
        }
    }

    private void Awake() {
        BoardData = new BoardData();
        MapToType(typeof(BoardConfig), BoardData);
    }
}
