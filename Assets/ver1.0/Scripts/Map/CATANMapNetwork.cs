﻿using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// マップネットワーク
/// </summary>
public class CATANMapNetwork {

	private Dictionary<string, CATANMapTile> tileDic;
	public Dictionary<string, CATANMapTile>.ValueCollection Tiles {
		get { return tileDic.Values; }
	}
	private Dictionary<string, CATANMapNode> nodeDic;
	private Dictionary<string, CATANMapLink> linkDic;

	private float tileRadius;
	private float vertRadius;
	private Vector3[] tileOffsets;
	private Vector3[] vertOffsets;
	private CATANMapNode baseNode = null;
	private CATANMapTile centerTile = null;

	public CATANMapNetwork(float tileRadius, float vertRadius) {
		tileDic = new Dictionary<string, CATANMapTile>();
		nodeDic = new Dictionary<string, CATANMapNode>();
		linkDic = new Dictionary<string, CATANMapLink>();

		this.tileRadius = tileRadius;
		this.vertRadius = vertRadius;

		//60度刻みのタイルの中心からの隣接タイル/頂点オフセットの設定
		tileOffsets = new Vector3[6];
		vertOffsets = new Vector3[6];
		float deltaAngle = 60f;
		float rad;
		for(int i = 0; i < 6; ++i) {
			rad = (deltaAngle * i + 30f) * Mathf.Deg2Rad;
			tileOffsets[i] = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * tileRadius;
			rad = deltaAngle * i * Mathf.Deg2Rad;
			vertOffsets[i] = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * vertRadius;
		}
	}

	#region VirtualFunction

	public override string ToString() {
		StringBuilder sb = new StringBuilder();
		foreach(var n in nodeDic.Keys) {
			sb.AppendLine(n.ToString());
		}
		return sb.ToString();
	}

	#endregion

	#region Function

	/// <summary>
	/// タイルの追加
	/// </summary>
	public void AddTile(GameObject tileObj, CATANUtil.MapTileType tileType) {
		Vector3 tPos = tileObj.transform.position;
		Vector3 pos;
		string posStr;
		int len = vertOffsets.Length;
		//タイルの追加
		CATANMapTile tile = new CATANMapTile(false, tPos, tileType);
		tile.SetBuilding(tileObj);
		tileDic.Add(tPos.ToString(), tile);
		//ノードの追加
		CATANMapNode node;
		for(int i = 0; i < len; ++i) {
			pos = tPos + vertOffsets[i] * vertRadius;
			posStr = pos.ToString();
			if(!nodeDic.ContainsKey(posStr)) {
				node = new CATANMapNode(false, pos);
				node.resetYOffset = 1f;
				nodeDic.Add(posStr, node);
			}
		}
		//タイルと隣接ノードの接続
		for(int i = 0; i < len; ++i) {
			pos = tPos + vertOffsets[i] * vertRadius;
			node = nodeDic[pos.ToString()];
			node.tiles.Add(tile);
			tile.nodes.Add(node);
		}
		//リンクの追加と接続
		Vector3 a, b;
		CATANMapLink link;
		for(int i = 0; i < len; ++i) {
			a = vertOffsets[i];
			b = vertOffsets[(i + 1) % len];
			pos = tPos + ((a + b) / 2f) * vertRadius;
			posStr = pos.ToString();
			if(!linkDic.ContainsKey(posStr)) {
				link = new CATANMapLink(false, pos);
				link.resetYOffset = 1f;
				//aとの接続
				node = nodeDic[(tPos + a).ToString()];
				link.a = node;
				node.links.Add(link);
				//bとの接続
				node = nodeDic[(tPos + b).ToString()];
				link.b = node;
				node.links.Add(link);
				//リンクの追加
				linkDic.Add(posStr, link);
			}
		}
	}

	/// <summary>
	/// タイル同士の接続(最後に)
	/// </summary>
	public void ConnectingTile() {
		Vector3 basePos;
		string posStr;
		CATANMapTile tile;
		foreach(var t in tileDic.Values) {
			basePos = t.pos;
			for(int i = 0; i < tileOffsets.Length; ++i) {
				posStr = (basePos + tileOffsets[i]).ToString();
				if(tileDic.ContainsKey(posStr)) {
					tile = tileDic[posStr];
					t.AddNeighbourTile(i, tile);
				}
			}
		}
		//中心のタイルを取得
		centerTile = GetNearTile(Vector3.zero);
	}

	/// <summary>
	/// サイコロ番号の設定
	/// </summary>
	public void SetDiceNumber(int[] diceNums) {
		//角からせめて行き止まりがあったら曲がる
		int dir = 0;
		int i = 0;
		var tile = GetCornerTile(0);
		tile.diceNumber = diceNums[i];
		var prevTile = tile;
		dir += 2;
		while(tile != centerTile) {
			tile = tile.GetDirTile(dir);
			while(tile == null || tile.diceNumber != 0) {
				dir = (dir + 1) % 6;
				tile = prevTile.GetDirTile(dir);
			}
			if(tile.type != CATANUtil.MapTileType.Desert) {
				tile.diceNumber = diceNums[++i];
			} else {
				tile.diceNumber = -1;
			}
			//Debug.Log(tile.pos);
			//Debug.Log(diceNums[i]);
			prevTile = tile;
		}
	}

	/// <summary>
	/// 角タイルの取得
	/// </summary>
	public CATANMapTile GetCornerTile(int dirIndex = 0) {
		//まずは角タイルを見つける(隣接タイルが3つ)
		if(centerTile == null && (centerTile = GetNearTile(Vector3.zero)) == null) return null;
		var tile = centerTile;
		var prevTile = tile;
		while(true) {
			tile = tile.GetDirTile(dirIndex);
			if(tile == null) {
				return prevTile;
			} else {
				prevTile = tile;
			}
		}
	}

	/// <summary>
	/// 指定した座標に最も近いタイルを返す
	/// </summary>
	public CATANMapTile GetNearTile(Vector3 pos) {
		var tile = tileDic.Values.First();
		if(tile == null) return null;
		var prevTile = tile;
		while(true) {
			//近い方に進んでいく
			tile = tile.GetNearTile(pos);
			if(tile == prevTile) {
				return tile;
			}
			prevTile = tile;
		}
	}

	/// <summary>
	/// 指定した座標に最も近いノードを返す
	/// </summary>
	public CATANMapNode GetNearNode(Vector3 pos) {
		var node = nodeDic.Values.First();
		if(node == null) return null;
		var prevNode = node;
		while(true) {
			//近い方に進んでいく
			node = node.GetNearNode(pos);
			if(node == prevNode) {
				return node;
			}
			prevNode = node;
		}
	}

	/// <summary>
	/// マップ要素の座標を元に戻す
	/// </summary>
	public void ResetElemPosition() {
		foreach(var t in tileDic.Values) t.ResetPosition();
		foreach(var n in nodeDic.Values) n.ResetPosition();
		foreach(var l in linkDic.Values) l.ResetPosition();
	}

	/// <summary>
	/// ノードの座標を取得
	/// </summary>
	public List<Vector3> GetNodePos() {
		return new List<Vector3>(nodeDic.Values.Select(x => x.pos));
	}

	/// <summary>
	/// リンクの座標を取得
	/// </summary>
	public List<Vector3> GetLinkPos() {
		return new List<Vector3>(linkDic.Values.Select(x => x.pos));
	}

	#endregion
}