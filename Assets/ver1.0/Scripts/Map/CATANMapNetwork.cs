using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// マップネットワーク
/// </summary>
public class CATANMapNetwork {

	private Dictionary<string, CATANMapTile> tileDic;
	private Dictionary<string, CATANMapNode> nodeDic;
	private Dictionary<string, CATANMapLink> linkDic;
	private Vector3[] vertOffsets;
	private CATANMapNode baseNode = null;

	public CATANMapNetwork() {
		tileDic = new Dictionary<string, CATANMapTile>();
		nodeDic = new Dictionary<string, CATANMapNode>();
		linkDic = new Dictionary<string, CATANMapLink>();

		//60度刻みのタイルの中心からの頂点オフセット
		vertOffsets = new Vector3[6];
		float deltaAngle = 60f;
		float rad;
		for(int i = 0; i < 6; ++i) {
			rad = deltaAngle * i * Mathf.Deg2Rad;
			vertOffsets[i] = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
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
	public void AddTile(GameObject tileObj, float radius) {
		Vector3 tPos = tileObj.transform.position;
		Vector3 pos;
		string posStr;
		int len = vertOffsets.Length;
		//タイルの追加
		CATANMapTile tile = new CATANMapTile(false, tPos);
		tile.SetBuilding(tileObj);
		tileDic.Add(tPos.ToString(), tile);
		//ノードの追加
		CATANMapNode node;
		for(int i = 0; i < len; ++i) {
			pos = tPos + vertOffsets[i] * radius;
			posStr = pos.ToString();
			if(!nodeDic.ContainsKey(posStr)) {
				node = new CATANMapNode(false, pos);
				node.resetYOffset = 1f;
				nodeDic.Add(posStr, node);
				if(baseNode == null) baseNode = node;
			}
		}
		//リンクの追加と接続
		Vector3 a, b;
		CATANMapLink link;
		for(int i = 0; i < len; ++i) {
			a = vertOffsets[i];
			b = vertOffsets[(i + 1) % len];
			pos = tPos + ((a + b) / 2f) * radius;
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
	/// 指定した座標に最も近いノードを返す
	/// baseNodeがnull(Tileが一つも追加されてない)な場合はnullを返す
	/// </summary>
	public CATANMapNode GetNearNode(Vector3 pos) {
		if(baseNode == null) return null;
		CATANMapNode node = baseNode;
		CATANMapNode prevNode = baseNode;
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