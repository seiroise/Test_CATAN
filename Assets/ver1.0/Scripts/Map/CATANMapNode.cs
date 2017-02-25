using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// カタンマップのノード(家とか建てられる場所)
/// </summary>
public class CATANMapNode : CATANMapElement {

	public List<CATANMapLink> links;

	public CATANMapNode() : base() {
		links = new List<CATANMapLink>();
	}

	public CATANMapNode(bool isBuild, Vector3 pos) : base(isBuild, pos) {
		links = new List<CATANMapLink>();
	}

	#region Function

	/// <summary>
	/// 隣接しているノードの中で指定した座標に最も近いノードを返す(自身を含む)
	/// </summary>
	public CATANMapNode GetNearNode(Vector3 pos) {
		CATANMapNode nearNode = this;
		CATANMapNode node;
		float dist;
		float minDist = Vector3.Distance(this.pos, pos);
		foreach(var l in links) {
			node = l.GetOppsiteNode(this);
			dist = Vector3.Distance(node.pos, pos);
			if(minDist > dist) {
				minDist = dist;
				nearNode = node;
			}
		}
		return nearNode;
	}

	#endregion
}