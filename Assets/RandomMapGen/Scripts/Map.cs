using UnityEngine;
using System.Collections;
using System.Linq;

public enum TileType{
	Empty = -1,
	Grass = 15,
	Tree = 16,
	Hills = 17,
	Mountains = 18,
	Towns = 19,
	Castle = 20,
	Monster = 21,
  Skull = 22,
  Ruins = 23,
  Tower = 24,
  Well = 25,
  Evo = 26
}

public class Map {

	public Tile[] tiles;
	public int columns;
	public int rows;

	public Tile[] coastTiles{
		get{ return tiles.Where (t => t.autotileID < (int)TileType.Grass).ToArray (); }
	}

	public Tile[] landTiles{
		get { return tiles.Where(t => t.autotileID == (int)TileType.Grass).ToArray(); }
	}

	public Tile castleTile{
		get{ return tiles.FirstOrDefault (t => t.autotileID == (int)TileType.Castle);	}
	}

  public Tile[] townTiles
  {
    get { return tiles.Where(t => t.autotileID == (int)TileType.Towns).ToArray(); }
  }

  public Tile[] ruinsTiles
  {
    get { return tiles.Where(t => t.autotileID == (int)TileType.Ruins).ToArray(); }
  }

  public Tile[] monsterTiles
  {
    get { return tiles.Where(t => t.autotileID == (int)TileType.Monster).ToArray(); }
  }

  public Tile[] skullTiles
  {
    get { return tiles.Where(t => t.autotileID == (int)TileType.Skull).ToArray(); }
  }

  public Tile towerTile
  {
    get { return tiles.FirstOrDefault(t => t.autotileID == (int)TileType.Tower); }
  }

  public Tile evoTile
  {
    get { return tiles.FirstOrDefault(t => t.autotileID == (int)TileType.Evo); }
  }

  public int percentVisited
  {
    get
    {
      float visited = tiles.Where(t => t.visited == true && t.autotileID != (int)TileType.Empty).ToArray().Length;
      float mapTiles = tiles.Where(t => t.autotileID != (int)TileType.Empty).ToArray().Length;
      return Mathf.RoundToInt((visited / mapTiles) * 100);
    }
  }

  public void NewMap(int width, int height){
		columns = width;
		rows = height;

		tiles = new Tile[columns * rows];

		CreateTiles ();
	}

	public void CreateIsland(
		float erodePercent,
		int erodeIterations,
		float treePercent,
		float hillPercent,
		float mountainPercent,
		float townPercent,
		float monsterPercent,
		float lakePercent,
    int fixedMonsterCount = -1
	){
		DecorateTiles (landTiles, lakePercent, TileType.Empty);

		for (var i = 0; i < erodeIterations; i++) {
			DecorateTiles (coastTiles, erodePercent, TileType.Empty);
		}

		var openTiles = landTiles;
		RandomizeTileArray (openTiles);
		openTiles[0].autotileID = (int)TileType.Castle;

        if (PasswordManager.Instance.CheckPassword("betray"))
        {
          openTiles[openTiles.Length - 1].autotileID = (int)TileType.Tower;
        }

        if (PasswordManager.Instance.CheckPassword("baxter"))
        {
            openTiles[openTiles.Length - 2].autotileID = (int)TileType.Well;
        }


        DecorateTiles (landTiles, treePercent, TileType.Tree);
		DecorateTiles (landTiles, hillPercent, TileType.Hills);
		DecorateTiles (landTiles, mountainPercent, TileType.Mountains);

    if (PasswordManager.Instance.CheckPassword("berlin"))
    {
      DecorateTiles(landTiles, townPercent, TileType.Towns);
    }
    else
    {
      DecorateTiles(landTiles, townPercent, TileType.Ruins);
    }

    if (fixedMonsterCount < 0)
    {
		  DecorateTiles (landTiles, monsterPercent, TileType.Monster);
    }
    else
    {
      DecorateTiles(landTiles, monsterPercent, TileType.Monster, fixedMonsterCount);
    }
	}

	private void CreateTiles(){
		var total = tiles.Length;

		for (var i = 0; i < total; i++) {
			var tile = new Tile ();
			tile.id = i;
			tiles [i] = tile;
		}

		FindNeighbors ();
	}

	private void FindNeighbors(){

		for (var r = 0; r < rows; r++) {

			for (var c = 0; c < columns; c++) {

				var tile = tiles [columns * r + c];

				if (r < rows - 1) {
					tile.AddNeighbor (Sides.Bottom, tiles [columns * (r + 1) + c]);
				}

				if (c < columns - 1) {
					tile.AddNeighbor (Sides.Right, tiles [columns * r + c + 1]);
				}

				if (c > 0) {
					tile.AddNeighbor (Sides.Left, tiles [columns * r + c - 1]);
				}

				if (r > 0) {
					tile.AddNeighbor (Sides.Top, tiles [columns * (r - 1) + c]);
				}
			}
		}
	}

	public void DecorateTiles(Tile[] tiles, float percent, TileType type, int fixedTotal = -1)
  {
    int total;
    if (fixedTotal > -1)
    {
      total = fixedTotal;
    }
    else
    {
      total = Mathf.FloorToInt (tiles.Length * percent);
    }

		RandomizeTileArray (tiles);

		for (var i = 0; i < total; i++) {

			var tile = tiles [i];

			if (type == TileType.Empty)
				tile.ClearNeighbors ();

			tile.autotileID = (int)type;
		}
	}

	public void RandomizeTileArray(Tile[] tiles){

		for (var i = 0; i < tiles.Length; i++) {
			var tmp = tiles [i];
			var r = Random.Range (i, tiles.Length);
			tiles [i] = tiles [r];
			tiles [r] = tmp;
		}
	}
}