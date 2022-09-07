using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IKPuzzle;
using JsonFx.Json;
// using Newtonsoft.Json;

public class TestSceneController : MonoBehaviour
{
    public Board board;
    // Start is called before the first frame update
    void Start()
    {
        LoadPuzzleBlockMap();
    }

    private void LoadPuzzleBlockMap()
    {
        TextAsset puzzleMap = Resources.Load<TextAsset>("simple_puzzle_map");
        Dictionary<string, object>[] deserializeObjects = JsonReader.Deserialize<Dictionary<string, object>[]>(puzzleMap.text);

        int lastRow = board.rowCount - 1;

        for(int i = deserializeObjects.Length - 1; i >= 0; --i)
        {
            int index = lastRow - i;
            int subIndex = 0;
            foreach(int value in deserializeObjects[i].Values)
            {
                BoardCell cell = board.boardCells[index, subIndex];
                cell.InitializePuzzleBlock((AttributeType)value);
                ++subIndex;
            }
        }

        // foreach(Dictionary<string, object> row in deserializeObjects)
        // {
        //     foreach(object value in row.Values)
        //     {
        //         Debug.Log(value);
        //     }
        // }

        Debug.Log(deserializeObjects.Length);
    }

    public void OnClickTestGetCellPosition()
    {
        board.GetBoardCell();
    }
}
