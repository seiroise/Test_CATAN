using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// カタンマップのタイル
/// </summary>
public class CATANMapTile : CATANMapElement {

	public List<CATANMapNode> nodes;
	public List<CATANMapLink> links;

	public CATANMapTile() : base() {
		nodes = new List<CATANMapNode>();
		links = new List<CATANMapLink>();
	}

	public CATANMapTile(bool isBuild, Vector3 pos) : base(isBuild, pos) {
		nodes = new List<CATANMapNode>();
		links = new List<CATANMapLink>();
	}
}