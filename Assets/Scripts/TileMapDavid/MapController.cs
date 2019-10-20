using Assets.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class MapController : MonoBehaviour
{
    public List<TextAsset> textAssets;

    public GameObject TilePrefab;

    public int MapWidth = 100;
    public int MapHeight = 50;

    public Vector2 BottomLeftLatLong = new Vector2(24.353800f, -127.505570f);
    public Vector2 TopRightLatLong = new Vector2(49.038010f, -76.808100f);

    public TileController[,] tiles;

    public bool Initialized = false;

    public enum editstate {none, mountains, oceans, resources}

    public editstate curstate = editstate.none;

    private void Awake()
    {
        tiles = new TileController[MapWidth, MapHeight];
        TileController[] ftiles = FindObjectsOfType<TileController>();

        foreach(TileController tile in ftiles)
        {
            //Resource newtile = new Resource(tile.gameObject, newlatlong, true, newobj.GetComponent<TileController>(), 100, 10);
            //newtile.tileController.Change(TileController.tiletype.Resource, 100);

            //tiles[(int)tile.transform.position.x, (int)tile.transform.position.y] = newtile;

            tiles[tile.myTile.xPos, tile.myTile.yPos] = tile;
        }

        Init();
    }

    private List<float> GetData(string text)
    {
        List<float> result = new List<float>();
        string[] values = text.Split(new char[]{ ',' });
        foreach (string value in values)
        {
            float val = 0;
            if (float.TryParse(value, out val))
                result.Add(val);
        }
        return result;
    }

    private void LoadData(int index)
    {
        var text = textAssets[index].text;
        var values = GetData(text);

        for (int i = 0; i < values.Count; ++i)
        {
            var value = values[i];
            var j = i % MapWidth;
            var k = i / MapWidth;

            // sanity check for the data- if we have the right number
            // of values from the file, then this check should never fail.
            if (j < MapWidth && k < MapHeight && tiles[j, k] != null)
            {
                tiles[j, k].Resources = value == 0f ? 1 : value;
            }
        }
    }

    void Init()
    {
        StartCoroutine(CycleSeasons());
    }

    public IEnumerator CycleSeasons()
    {
        for(int i = 0; i < textAssets.Count; ++i)
        {
            LoadData(i);
            yield return new WaitForSeconds(190 / 12);
        }
    }

    public void ChangeTile(GameObject hit)
    {
        TileController newtile = hit.transform.root.GetComponent<TileController>();

        if (newtile == null)
        {
            return;
        }

        Vector2 latlong = newtile.myTile.latlong;

        int xPos = newtile.myTile.xPos;
        int yPos = newtile.myTile.yPos;

        switch (curstate)
        {            
            case editstate.none:
                Debug.Log("Hitloc=" + newtile);
                break;
            case editstate.mountains:
                newtile.myTile = new Tile(latlong, true, 70);
                newtile.Change(TileController.TileType.Mountain);
                break;
            case editstate.oceans:
                
                newtile.myTile = new Tile(latlong, false, 0);
                newtile.Change(TileController.TileType.Ocean);
                break;
            default:
                newtile.myTile = new Resource(latlong, true, 20, 100, 10);
                newtile.Change(TileController.TileType.Resource);
                break;
                
        }
        newtile.myTile.xPos = xPos;
        newtile.myTile.yPos = yPos;

        //tiles[(int)location.x, (int)location.y]
    }

    public void GenerateMap()
    {
        if (Initialized)
        {
            Clean();
        }

        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                Vector2 newlatlong = new Vector2();

                float resulti = (float)i / MapWidth;
                float resultj = (float)j / MapWidth;

                if (i != 0 && j != 0)
                {

                    newlatlong = new Vector2(Mathf.Lerp(BottomLeftLatLong.x, TopRightLatLong.x, resulti), Mathf.Lerp(BottomLeftLatLong.y, TopRightLatLong.y, resultj));
                }
                else if (i == 0 && j == 0)
                {
                    newlatlong = new Vector2(BottomLeftLatLong.x, BottomLeftLatLong.y);
                }
                else if (j == 0)
                {
                    newlatlong = new Vector2(Mathf.Lerp(BottomLeftLatLong.x, TopRightLatLong.x, resulti), BottomLeftLatLong.y);
                }
                else if (i == 0)
                {
                    newlatlong = new Vector2(BottomLeftLatLong.x, Mathf.Lerp(BottomLeftLatLong.y, TopRightLatLong.y, resultj));
                }

                GameObject newobj = Instantiate(TilePrefab, new Vector3(i, 0, j), Quaternion.identity);
                Resource newtile = new Resource(newlatlong, true, 20, 100, 10);
                newtile.xPos = i;
                newtile.yPos = j;

                TileController controller = newobj.GetComponent<TileController>();
                controller.controller = this;
                controller.Change(TileController.TileType.Resource);
                controller.myTile = newtile;
            }
        }

        Initialized = true;
    }

    public void Clean()
    {
        TileController[] ftiles = FindObjectsOfType<TileController>();

        foreach (TileController tile in ftiles)
        {
            DestroyImmediate(tile.gameObject);
        }

        Initialized = false;
    }

    public Vector2 GetTopRight()
    {
        return new Vector2(tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1].myTile.xPos, tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1].myTile.yPos);
    }

    public Vector2 GetBottomRight()
    {
        return new Vector2(tiles[tiles.GetLength(0) - 1, 0].myTile.xPos, tiles[tiles.GetLength(0) - 1, 0].myTile.yPos);
    }

    public Vector2 GetTopLeft()
    {
        return new Vector2(tiles[0, tiles.GetLength(1) - 1].myTile.xPos, tiles[0, tiles.GetLength(1) - 1].myTile.yPos);
    }

    public Vector2 GetBottomLeft()
    {
        return new Vector2(tiles[0, 0].myTile.xPos, tiles[0, 0].myTile.yPos);
    }

    public bool CheckValid(Vector2 location, Guid id = new Guid())
    {
        if ((int)location.x > MapWidth - 1 || (int)location.x < 0 || (int)location.y > MapHeight - 1 || (int)location.y < 0 || tiles[(int)location.x, (int)location.y].myTile.occupied)
        {
            return false;
        }

        if (tiles[(int)location.x, (int)location.y].myTile is City city)
        {
            if (city.Owner != id)
            {
                return false;
            }
        }
        

        return tiles[(int)location.x, (int)location.y].myTile.pathable;
    }

    public bool CheckValid(int x, int y,Guid id)
    {
        return CheckValid(new Vector2(x,y),id);
    }

    public float CheckCost(Vector2 location)
    {
        if ((int)location.x > MapWidth - 1 || (int)location.x < 0 || (int)location.y > MapHeight - 1 || (int)location.y < 0 || tiles[(int)location.x, (int)location.y].myTile.occupied)
        {
            return 0;
        }


        return tiles[(int)location.x, (int)location.y].myTile.travelCost;
    }

    public void Initilizeplayer(Vector2 to)
    {
        tiles[(int)to.x, (int)to.y].myTile.occupied = true;
    }

    public void Changeloc(Vector2 from, Vector2 to)
    {
        tiles[(int)from.x, (int)from.y].myTile.occupied = false;
        tiles[(int)to.x, (int)to.y].myTile.occupied = true;
    }

    public float Harvesttile(Vector2 loc, float requestedvalue)
    {
        if (!CheckValid(loc)) return 0f;
        var tile = tiles[(int)loc.x, (int)loc.y];
        var type = tile.myTile.GetType();

        if (tile.CurrentType == TileController.TileType.Resource)
        {
            if (tile.myTile is Resource resource)
            {
                if (resource.resources >= requestedvalue)
                {
                    resource.resources -= requestedvalue;
                    return requestedvalue;
                }
                else
                {
                    requestedvalue = resource.resources;
                    resource.resources = 0;
                    return requestedvalue;
                }
            }
        }
        return 0;
    }

    internal List<TileController> GetNeighbors(int x, int y, Guid owner)
    {
        var neighbors = new List<TileController>();
        if (CheckValid(x + 1, y, owner))
        {
            neighbors.Add(tiles[x + 1, y]);
        }

        if (CheckValid(x - 1, y, owner))
        {
            neighbors.Add(tiles[x - 1, y]);
        }

        if (CheckValid(x, y + 1, owner))
        {
            neighbors.Add(tiles[x, y + 1]);
        }

        if (CheckValid(x, y - 1, owner))
        {
            neighbors.Add(tiles[x, y - 1]);
        }

        return neighbors;
    }

    internal IResourceProvider BuildOnTile(Vector2 location, Guid characterGuid, Color color)
    {
        City newcity = new City(tiles[(int)location.x, (int)location.y].myTile.latlong, true, 5, 100, 10, characterGuid)
        { xPos = tiles[(int)location.x, (int)location.y].myTile.xPos,
            yPos = tiles[(int)location.x, (int)location.y].myTile.yPos};


        tiles[(int)location.x, (int)location.y].myTile = newcity;
        tiles[(int)location.x, (int)location.y].Change(TileController.TileType.City);



        if (tiles[(int)location.x, (int)location.y].myTile is City city)
        {
            city.Owner = characterGuid;
        }

        return tiles[(int)location.x, (int)location.y];
    }
}
