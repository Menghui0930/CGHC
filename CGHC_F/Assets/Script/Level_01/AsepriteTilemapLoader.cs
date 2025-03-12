#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class AsepriteTilemapLoader : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] tiles;
    public TextAsset jsonFile; // 直接拖.json进 Inspector

    [System.Serializable]
    public class TilemapData
    {
        public List<Layer> layers;
    }

    [System.Serializable]
    public class Layer
    {
        public string name;
        public List<Cel> cels;
    }

    [System.Serializable]
    public class Cel
    {
        public TilemapInfo tilemap;
    }

    [System.Serializable]
    public class TilemapInfo
    {
        public int width;
        public int height;
        public List<int> tiles;
    }

    public void GenerateTilemap()
    {
        if (jsonFile == null)
        {
            Debug.LogError("请拖入 JSON 文件到脚本的 jsonFile 字段！");
            return;
        }

        TilemapData mapData = JsonUtility.FromJson<TilemapData>(jsonFile.text);

        var celTilemap = mapData.layers[0].cels[0].tilemap;

        int width = celTilemap.width;
        int height = celTilemap.height;

        tilemap.ClearAllTiles();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileIndex = celTilemap.tiles[y * width + x];

                if (tileIndex >= 0 && tileIndex < tiles.Length) // 忽略非法索引
                {
                    tilemap.SetTile(new Vector3Int(x, -y, 0), tiles[tileIndex]);
                }
            }
        }

        Debug.Log("Tilemap 生成完成！");

        // 标记场景变更，防止退出编辑模式后丢失 Tilemap 数据
#if UNITY_EDITOR
        EditorUtility.SetDirty(tilemap);
        EditorUtility.SetDirty(tilemap.gameObject);
        Undo.RecordObject(tilemap, "生成 Tilemap");
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AsepriteTilemapLoader))]
public class AsepriteTilemapLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AsepriteTilemapLoader script = (AsepriteTilemapLoader)target;

        if (GUILayout.Button("生成 Tilemap"))
        {
            script.GenerateTilemap();
        }
    }
}
#endif
