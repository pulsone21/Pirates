using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TerrainTypes", fileName = "New TerrainType")]
public class TerrainTypes : ScriptableObject
{
    public float height;
    public bool sailable, walkable;
    public string terrainName;
    public Color color;

    //TODO need do identify the display form, for more textrues etc...

}
