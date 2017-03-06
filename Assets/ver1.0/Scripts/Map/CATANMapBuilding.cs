using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// マップ上の建物
/// </summary>
public class CATANMapBuilding<Element> where Element : CATANMapElement {

	public Element elem;	//接地している要素

	public CATANMapBuilding(Element elem) {
		this.elem = elem;
	}
}