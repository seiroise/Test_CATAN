using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// マウスの位置にキャラクタを配置
/// </summary>
public class CATANMouseLocater : MonoBehaviour {

	[SerializeField]
	private Camera cam;
	[SerializeField]
	private Text selectText;
	[SerializeField]
	private CATANMapTileLocater locater;
	[SerializeField]
	private GameObject[] locateObjects;
	private int selectIndex = 0;
	private string wheelAxis = "Mouse ScrollWheel";

	#region UnityEvent

	private void Start() {
		if(selectText) {
			selectText.text = locateObjects[selectIndex].name;
		}
	}

	private void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Locate();
		} else if(Input.GetMouseButtonDown(1)) {
			Debug.Log("ResetPosition");
			locater.Network.ResetElemPosition();
		}
		SelectWheel();
	}

	#endregion

	#region Function

	/// <summary>
	/// マウスの位置にキャラクタを配置
	/// </summary>
	private void Locate() {
		if(!locater) return;
		if(!cam) cam = Camera.main;
		var mRay = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if(Physics.Raycast(mRay, out hitInfo, 100f)) {
			var node = locater.Network.GetNearNode(hitInfo.point);
			if(node.isBuild) return;
			Vector3 pos = node.pos;
			pos.y = 10f;
			var obj = (GameObject)Instantiate(locateObjects[selectIndex], pos, Quaternion.identity);
			node.SetBuilding(obj);
		}
	}

	/// <summary>
	/// ホイールの処理
	/// </summary>
	private void SelectWheel() {
		var wheel = Input.GetAxis(wheelAxis);
		if(wheel > 0) {
			selectIndex = (selectIndex + 1) % locateObjects.Length;
			if(selectText) {
				selectText.text = locateObjects[selectIndex].name;
			}
		} else if(wheel < 0){
			selectIndex = (selectIndex + locateObjects.Length - 1) % locateObjects.Length;
			if(selectText) {
				selectText.text = locateObjects[selectIndex].name;
			}
		}
	}

	#endregion
}