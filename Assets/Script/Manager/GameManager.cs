using CatCupTool;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] string levelToLoad;


    void Start()
    {
        LoadLevel(levelToLoad);
    }


    public void LoadLevel(string fileName)
    {
        LevelData levelData = CatCup.LoadJsonFile(fileName);

        GameObject loadPrefab = Resources.Load<GameObject>(CatCup.BASE_LEVEL_SETUP_PATH);

        GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(loadPrefab);
        Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Prefab");
        Selection.activeObject = spawnedObject;

        List<List<ZoneNode>> nodeLst = levelData.nodeLst;
        List<IDData> idDatas = levelData.idDatas;

        Vector2 startAt = new Vector2(CatCup.MIN_X, CatCup.MAX_Y) + (Vector2)spawnedObject.transform.position;
        float stepY = (CatCup.MAX_Y - CatCup.MIN_Y) / (nodeLst.Count - 1);
        float stepX = (CatCup.MAX_X - CatCup.MIN_X) / (nodeLst[0].Count - 1);
        for (int i = 0; i < idDatas.Count; i++)
        {
            ZoneNode[] couple = levelData.FindCouple(idDatas[i]);
            if (couple.Length < 2 || idDatas[i].id == "")
                continue;

            Vector2 pos0 = new Vector2(startAt.x + couple[0].gridX * stepX
                                        , startAt.y - couple[0].gridY * stepY);
            Vector2 pos1 = new Vector2(startAt.x + couple[1].gridX * stepX
                                        , startAt.y - couple[1].gridY * stepY);


            Vector2 spawnPos = (pos0 + pos1) / 2f;
            float distance = Vector2.Distance(pos0, pos1);
            Vector2 direction = pos1 - pos0;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject prefab = CatCup.GetPrefab(idDatas[i].type);
            if (prefab == null)
                continue;
            GameObject spawn = Instantiate(prefab, spawnPos, Quaternion.identity, spawnedObject.transform);
            spawn.GetComponent<IZoneNode>().Init(idDatas[i], distance, angle);

        }
    }
}
