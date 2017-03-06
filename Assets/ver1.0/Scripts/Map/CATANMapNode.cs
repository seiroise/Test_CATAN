using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マップノード(家とか建てられる場所)
/// </summary>
public class CATANMapNode : CATANMapElement {

	private List<CATANMapTile> _tiles;  //隣接タイル
	private List<CATANMapLink> _links;  //隣接リンク

	private float _nearTileDiceWeight = 1f; //周辺タイルのサイコロ番号の重み
	public float nearTileDiceWeight { get { return _nearTileDiceWeight; } }

	private bool _isBuildNearNode;  //周辺の距離1にあるノードの何れかに建物が建築されているか
	public bool isBuildNearNode { get { return _isBuildNearNode; } set { _isBuildNearNode = value; } }

	public CATANMapNode(Vector3 pos) : base(pos) {
		_tiles = new List<CATANMapTile>();
		_links = new List<CATANMapLink>();
	}

	#region VirtualFunction

	/// <summary>
	/// 建物の設定
	/// </summary>
	public override void SetBuilding(GameObject building) {
		if(_isBuild || _isBuildNearNode) return;
		base.SetBuilding(building);
		//隣接するノードへのフラグ立て
		foreach(var l in _links) {
			l.GetOppsiteNode(this).isBuildNearNode = true;
		}
	}

	#endregion

	#region Function

	/// <summary>
	/// 隣接タイルの追加
	/// </summary>
	public void AddNeighbourTile(CATANMapTile tile) {
		_tiles.Add(tile);
	}

	/// <summary>
	/// 隣接リンクの追加
	/// </summary>
	public void AddNeighbourLink(CATANMapLink link) {
		_links.Add(link);
	}

	/// <summary>
	/// 隣接しているノードの中で指定した座標に最も近いノードを返す(自身を含む)
	/// </summary>
	public CATANMapNode GetNearNode(Vector3 pos) {
		CATANMapNode nearNode = this;
		CATANMapNode node;
		float dist;
		float minDist = Vector3.Distance(this._position, pos);
		foreach(var l in _links) {
			node = l.GetOppsiteNode(this);
			dist = Vector3.Distance(node.position, pos);
			if(minDist > dist) {
				minDist = dist;
				nearNode = node;
			}
		}
		return nearNode;
	}

	/// <summary>
	/// 隣接しているリンクの中で指定した座標に最も近いリンクを返す
	/// </summary>
	public CATANMapLink GetNearLink(Vector3 pos) {
		CATANMapLink nearLink = _links.First();
		float dist;
		float minDist = Vector3.Distance(nearLink.position, pos);
		foreach(var l in _links) {
			dist = Vector3.Distance(l.position, pos);
			if(minDist > dist) {
				minDist = dist;
				nearLink = l;
			}
		}
		return nearLink;
	}

	/// <summary>
	/// 重み付け
	/// </summary>
	public void SetWeight() {
		float sum = 0f;
		int i = 3;
		foreach(var t in _tiles) {
			sum += Mathf.Abs(t.diceNumber - 7);
			i--;
		}
		if(i > 0) sum += i * 7;
		_nearTileDiceWeight = sum / 3f;
	}

	#endregion
}