using UnityEngine;
using System.Collections.Generic;
using System; 

[Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3Data(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class QuaternionData
{
    public float x;
    public float y;
    public float z;
    public float w;

    public QuaternionData(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}


[Serializable]
public class SpawnPointData
{
    public Vector3Data position;
    public QuaternionData rotation;

    public SpawnPointData(Transform spawnTransform)
    {
        position = new Vector3Data(spawnTransform.position);
        rotation = new QuaternionData(spawnTransform.rotation);
    }
}

[Serializable]
public class ObstacleData
{
    public string prefabName; 
    public Vector3Data position;
    public QuaternionData rotation;

    public ObstacleData(GameObject obstacleObject, string nameIdentifier)
    {
        prefabName = nameIdentifier; 
        position = new Vector3Data(obstacleObject.transform.position);
        rotation = new QuaternionData(obstacleObject.transform.rotation);
    }
}

[Serializable]
public class LevelData
{
    public string levelName; 
    public SpawnPointData spawnPoint; 
    public List<ObstacleData> obstacles = new List<ObstacleData>();

    public float maxPlayerHealth; 
    public int winScore; 

    public LevelData(string name)
    {
        levelName = name;
    }
}

[System.Serializable]
public struct PrefabNameMapping
{
    public string name;
    public GameObject prefab; 
}