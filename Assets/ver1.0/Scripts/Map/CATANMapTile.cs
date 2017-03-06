using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// カタンマップのタイル
/// </summary>
public class CATANMapTile : CATANMapElement {

	private int _diceNumber;            //ダイスの出目(2-6,8-11)
	public int diceNumber {
		get { return _diceNumber; }
		set { _diceNumber = value; }
	}
	private CATANUtil.MapTileType _type;  //タイル種類
	public CATANUtil.MapTileType type { get { return _type; } }

	private CATANMapTile[] dirTiles;    //隣接方向毎のタイル(0=北東、1=北, 2=北西, ...)

	private List<CATANMapTile> tiles;   //隣接タイル
	private List<CATANMapNode> nodes;   //頂点ノード

	public CATANMapTile(Vector3 pos, CATANUtil.MapTileType type) : base(pos) {
		_diceNumber = 0;
		_type = type;
		dirTiles = new CATANMapTile[6];
		tiles = new List<CATANMapTile>();
		nodes = new List<CATANMapNode>();
	}

	#region Function

	/// <summary>
	/// 隣接タイルの追加
	/// </summary>
	public void AddTile(int dirIndex, CATANMapTile tile) {
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
		float minDist = Vector3.Distance(this._position, pos);
		foreach(var t in tiles) {
			dist = Vector3.Distance(t._position, pos);
			if(minDist > dist) {
				minDist = dist;
				nearTile = t;
			}
		}
		return nearTile;
	}

	/// <summary>
	/// 隣接ノードの追加
	/// </summary>
	public void AddNode(CATANMapNode node) {
		nodes.Add(node);
	}

	/// <summary>
	/// 隣接タイルの数を返す
	/// </summary>
	public int GetNeighbourTileCount() {
		return tiles.Count;
	}

	#endregion
}