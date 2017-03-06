using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// マップの要素
/// </summary>
public class CATANMapElement {

	protected Vector3 _position;
	public Vector3 position { get { return _position; } }
	protected GameObject _building;
	protected bool _isBuild;
	public bool isBuild { get { return _isBuild; } }

	public CATANMapElement(Vector3 position) {
		this._position = position;
		this._building = null;
		this._isBuild = false;
	}

	#region Function

	/// <summary>
	/// 建物の設定
	/// </summary>
	public virtual void SetBuilding(GameObject building) {
		if(_isBuild) return;
		this._building = building;
		this._isBuild = true;
	}

	#endregion
}