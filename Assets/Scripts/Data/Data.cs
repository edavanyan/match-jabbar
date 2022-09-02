using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IData {
    void SetFromConfig(IConfig config);
}

public abstract class Data<T> : IData where T : IConfig {
    public abstract void SetFromConfig(T config);

    public void SetFromConfig(IConfig config)
    {
        this.SetFromConfig((T)config);
    }
}
