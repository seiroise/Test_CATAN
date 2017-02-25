using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マップタイル配置
/// </summary>
public class CATANMapTileLocater : MonoBehaviour {

	/// <summary>
	/// 配置種類
	/// </summary>
	public enum LocateMode {
		Normal,     //通常のカタン
		Large       //6人用カタン
	}

	[SerializeField]
	private Transform tileParent;
	[SerializeField, Range(0.01f, 5f)]
	private float radius = 2f;
	[SerializeField]
	private LocateMode mode = LocateMode.Normal;
	[SerializeField]
	private GameObject[] locateObjs;

	[Header("Debug")]
	public bool isDebug = false;
	public GameObject nodeObj;
	public GameObject linkObj;

	private CATANMapNetwork network;
	public CATANMapNetwork Network { get { return network; } }

	private int[] normalTiles = {
		0,				//砂漠
		1, 1, 1, 1,		//森
		2, 2, 2, 2,		//草原
		3, 3, 3, 3,		//麦
		4, 4, 4,		//土
		5, 5, 5			//岩
	};
	private int[] largeTiles = {
		0, 0,
		1, 1, 1, 1, 1, 1,
		2, 2, 2, 2, 2, 2,
		3, 3, 3, 3, 3, 3,
		4, 4, 4, 4, 4,
		5, 5, 5, 5, 5
	};

	#region UnityEvent

	private void Start() {
		Locate();
	}

	#endregion

	#region Function

	/// <summary>
	/// 配置
	/// </summary>
	public void Locate() {
		CATANMapNetwork network = null;
		switch(mode) {
			case LocateMode.Normal:
			//LocateForLines(normalTiles, 5, 3);
			network = LocateForLines(normalTiles, 5, 3);
			break;
			case LocateMode.Large:
			//LocateForLines(largeTiles, 6, 3);
			network = LocateForLines(largeTiles, 6, 3);
			break;
		}
		if(isDebug && network != null) {
			foreach(var v in network.GetNodePos()) {
				Instantiate(nodeObj, v, Quaternion.identity);
			}
			foreach(var v in network.GetLinkPos()) {
				Instantiate(linkObj, v, Quaternion.identity);
			}
		}
		this.network = network;
	}

	/// <summary>
	/// 円形状に配置
	/// </summary>
	public void LocateForCircle() {
		Transform parent = tileParent;
		if(!parent) parent = transform;

		List<int> list = new List<int>(normalTiles);
		Stack<int> locateStack = new Stack<int>();
		int rIndex;
		while(list.Count > 0) {
			rIndex = Random.Range(0, list.Count);
			locateStack.Push(list[rIndex]);
			list.RemoveAt(rIndex);
		}

		//配置
		int num;
		float rad, deltaAngle;

		Instantiate(locateObjs[locateStack.Pop()], Vector3.zero, Quaternion.identity);

		num = 6;
		deltaAngle = 360f / num;
		for(int i = 0; i < num; ++i) {
			rad = (deltaAngle * i) * Mathf.Deg2Rad;
			Instantiate(locateObjs[locateStack.Pop()], new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius, Quaternion.identity);
		}

		num = 12;
		deltaAngle = 360f / num;
		for(int i = 0; i < num; ++i) {
			rad = (deltaAngle * i) * Mathf.Deg2Rad;
			Instantiate(locateObjs[locateStack.Pop()], new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius * 2f, Quaternion.identity);
		}
	}

	/// <summary>
	/// 線状に配置(複数)
	/// </summary>
	public CATANMapNetwork LocateForLines(int[] tiles, int max, int min) {
		var locateStack = new Stack<int>(GetShuffleArray<int>(tiles));
		Vector3 baseOffset, offsetR, offsetL;
		float rad;

		//オフセットの計算
		baseOffset = new Vector3(0f, 0f, -(max / 2) * radius);
		rad = 30f * Mathf.Deg2Rad;
		offsetR = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
		rad = 150f * Mathf.Deg2Rad;
		offsetL = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;

		//配置
		var network = new CATANMapNetwork();
		LocateForLine(locateStack, max, baseOffset, network);
		for(int i = max - 1, j = 1; i >= min; --i, ++j) {
			LocateForLine(locateStack, i, offsetL * j + baseOffset, network);
			LocateForLine(locateStack, i, offsetR * j + baseOffset, network);
		}
		return network;
	}

	/// <summary>
	/// 線状に配置
	/// </summary>
	private void LocateForLine(Stack<int> locateStack, int locateNum, Vector3 offset, CATANMapNetwork network) {
		for(int i = 0; i < locateNum; ++i) {
			var tileObj = (GameObject)Instantiate(locateObjs[locateStack.Pop()], new Vector3(0f, 0f, i * radius) + offset, Quaternion.identity);
			tileObj.transform.SetParent(tileParent);
			network.AddTile(tileObj, 1f);	//タイルの頂点は中心から1の距離にある(radiusは使わない)
		}
	}

	/// <summary>
	/// 配列の内容をシャッフルした新しい配列を返す
	/// </summary>
	private T[] GetShuffleArray<T>(T[] src) {
		List<T> srcList = new List<T>(src);
		T[] dst = new T[src.Length];
		int rIndex;
		for(int i = 0; i < dst.Length; ++i) {
			rIndex = Random.Range(0, srcList.Count);
			dst[i] = srcList[rIndex];
			srcList.RemoveAt(rIndex);
		}
		return dst;
	}

	#endregion
}