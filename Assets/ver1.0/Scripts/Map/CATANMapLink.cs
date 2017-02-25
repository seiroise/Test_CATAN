using UnityEngine;
using System.Collections;

/// <summary>
/// カタンマップノード(道とか建てられる場所)
/// </summary>
public class CATANMapLink : CATANMapElement {

	public CATANMapNode a, b;

	public CATANMapLink() : base() {
		a = b = null;
	}

	public CATANMapLink(bool isBuild, Vector3 pos) : base(isBuild, pos) {
		a = b = null;
	}

	#region Function

	/// <summary>
	/// 反対側のノードを返す
	/// nodeがaでもbでもない場合はnullを返す
	/// </summary>
	public CATANMapNode GetOppsiteNode(CATANMapNode node) {
		if(node == a) {
			return b;
		} else if(node == b) {
			return a;
		}
		return null;
	}

	#endregion
}