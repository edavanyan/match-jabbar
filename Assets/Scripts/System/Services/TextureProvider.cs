using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TextureProvider : Service<List<Sprite>>
{
    [FormerlySerializedAs("_spriteList")] [SerializeField]
    private List<Sprite> spriteList;

    public Sprite GetSpriteByName(string name) {
        foreach (var sprite in spriteList)
        {
            if (sprite.name == name) {
                return sprite;
            }
        }
        throw new System.Exception("Sprite with name: " + name + " :does not exist");
    }

    public Sprite GetSpriteByIndex(int index) {
        return spriteList[index];
    }
}
