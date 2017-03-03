using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// カタンマップのタイル
/// </summary>
public class CATANMapTile : CATANMapElement {

	public int diceNumber;				//ダイスの出目(2-6,8-11)
	public CATANUtil.MapTileType type;  //タイル種類

	private CATANMapTile[] dirTiles;	//隣接方向毎のタイル(0=北東、1=北, 2=北西, ...)

	private List<CATANMapTile> tiles;	//隣接タイル
	public List<CATANMapNode> nodes;	//頂点ノード

	public CATANMapTile() : base() {
		diceNumber = 0;
		type = CATANUtil.MapTileType.NoUse;
		dirTiles = new CATANMapTile[6];
		tiles = new List<CATANMapTile>();
		nodes = new List<CATANMapNode>();
	}

	public CATANMapTile(bool isBuild, Vector3 pos, CATANUtil.MapTileType type) : base(isBuild, pos) {
		diceNumber = 0;
		this.type = type;
		dirTiles = new CATANMapTile[6];
		tiles = new List<CATANMapTile>();
		nodes = new List<CATANMapNode>();
	}

	#region Function

	/// <summary>
	/// 隣接タイルの追加
	/// </summary>
	public void AddNeighbourTile(int dirIndex, CATANMapTile tile) {
		if(dirIndex < 0 || dirTiles.Length <= dirIndex) return;
		dirTiles[dirIndex] = tile;
		tiles.Add(tile);
	}

	/// <summary>
	/// 指定した方向のタイルを取得する
	/// </summary>
	public CATANMapTile GetDirTile(int dirIndex) {
		if(dirIndex < 0 || dirTiles.Length <= dirIndex) return null;
		return dirTiles[dirIndex];
	}

	/// <summary>
	/// 隣接しているタイルの中で指定した座標に最も近いタイルを返す
	/// </summary>
	public CATANMapTile GetNearTile(Vector3 pos) {
		CATANMapTile nearTile = this;
		float dist;
		float minDist = Vector3.Distance(this.pos, pos);
		foreach(var t in tiles) {
			dist = Vector3.Distance(t.pos, pos);
			if(minDist > dist) {
				minDist = dist;
				nearTile = t;
			}
		}
		return nearTile;
	}

	/// <summary>
	/// 隣接タイルの数を返す
	/// </summary>
	public int GetNeighbourTileCount() {
		return tiles.Count;
	}

	#endregion
}