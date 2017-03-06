using UnityEngine;
using System.Collections;

/// <summary>
/// マップリンク(道とか建てられる場所)
/// </summary>
public class CATANMapLink : CATANMapElement {

	private CATANMapNode _a, _b;

	public CATANMapLink(Vector3 pos) : base(pos) {
		_a = _b = null;
	}

	#region Function

	/// <summary>
	/// Aノードの設定
	/// </summary>
	public void SetNodeA(CATANMapNode node) {
		_a = node;
	}

	/// <summary>
	/// Bノードの設定
	/// </summary>
	public void SetNodeB(CATANMapNode node) {
		_b = node;
	}

	/// <summary>
	/// 反対側のノードを返す
	/// nodeがaでもbでもない場合はnullを返す
	/// </summary>
	public CATANMapNode GetOppsiteNode(CATANMapNode node) {
		if(node == _a) {
			return _b;
		} else if(node == _b) {
			return _a;
		}
		return null;
	}



	#endregion
}