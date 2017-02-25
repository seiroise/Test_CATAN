using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// カタンマップの要素
/// </summary>
public class CATANMapElement {
	
	public bool isBuild;
	public GameObject building;
	public Vector3 pos;
	public float resetYOffset;

	public CATANMapElement() {
		isBuild = false;
		pos = Vector3.zero;
		resetYOffset = 0f;
	}

	public CATANMapElement(bool isBuild, Vector3 pos) {
		this.isBuild = isBuild;
		this.pos = pos;
		this.resetYOffset = 0f;
	}

	#region Function

	/// <summary>
	/// 建物の設定
	/// </summary>
	public void SetBuilding(GameObject building) {
		if(isBuild) return;
		this.building = building;
		isBuild = true;
	}

	/// <summary>
	/// 建物の座標を元に戻す
	/// </summary>
	public void ResetPosition() {
		if(!isBuild) return;
		Vector3 rPos = pos;
		rPos.y += resetYOffset;
		building.transform.position = rPos;
		building.transform.eulerAngles = Vector3.zero;
	}

	#endregion
}