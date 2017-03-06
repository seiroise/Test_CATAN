using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マップ作成と管理(多分押し付けられる)
/// </summary>
public class CATANMapCreater : MonoBehaviour {

	/// <summary>
	/// 配置種類
	/// </summary>
	public enum CreateMode {
		Normal,     //通常のカタン
		Large       //6人用カタン
	}

	[Header("Locate Setting")]
	[SerializeField]
	private Transform tileParent;
	[SerializeField, Range(0.01f, 5f)]
	private float radius = 2f;
	[SerializeField]
	private CreateMode mode = CreateMode.Normal;
	[SerializeField]
	private GameObject[] locateTiles;

	[Header("Building")]
	[SerializeField]
	private GameObject homePref;	//拠点
	[SerializeField]
	private GameObject cityPref;	//都市
	[SerializeField]
	private GameObject roadPref;	//街道
	[SerializeField]
	private GameObject portPref;	//港

	[Header("Build Setting")]
	[SerializeField, Range(0f, 10f)]
	private float yOffset = 0.5f;

	[Header("Dice Num")]
	[SerializeField]
	private Transform diceNumParent;	//ダイス番号テキストの親
	[SerializeField]
	private Text diceNumText;           //ダイス番号テキスト
	[SerializeField]
	private float textY = 0.15f;		//テキストのy座標

	[Header("Option")]
	public bool startWithCreate;

	[Header("Debug")]
	public bool isDebug = false;
	public GameObject nodeObj;
	public GameObject linkObj;

	private CATANMapNetwork _network;
	public CATANMapNetwork network { get { return _network; } }

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
	private int[] normalDices = {
		5, 2, 6, 3, 8, 10, 9, 12, 11, 4, 8, 10, 9, 4, 5, 6, 3, 11
	};

	#region UnityEvent

	private void Start() {
		if(startWithCreate)	Create();
	}

	#endregion

	#region Function

	/// <summary>
	/// 作成
	/// </summary>
	public CATANMapNetwork Create() {
		CATANMapNetwork network = null;
		switch(mode) {
			case CreateMode.Normal:
			//LocateForLines(normalTiles, 5, 3);
			network = LocateForLines(normalTiles, 5, 3);
			break;
			case CreateMode.Large:
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
		if(network != null) {
			//タイル同士の接続
			network.ConnectingTile();
			//ダイス番号の設定
			network.SetDiceNumber(normalDices);
			//ダイス番号の表示
			Vector3 pos;
			foreach(var t in network.tiles) {
				if(t.type == CATANUtil.MapTileType.Desert) continue;
				pos = t.position;
				pos.y = textY;
				var obj = (Text)Instantiate(diceNumText, pos, diceNumText.transform.rotation);
				obj.transform.SetParent(diceNumParent);
				obj.text = t.diceNumber.ToString();
			}
		}
		return this._network = network;
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

		Instantiate(locateTiles[locateStack.Pop()], Vector3.zero, Quaternion.identity);

		num = 6;
		deltaAngle = 360f / num;
		for(int i = 0; i < num; ++i) {
			rad = (deltaAngle * i) * Mathf.Deg2Rad;
			Instantiate(locateTiles[locateStack.Pop()], new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius, Quaternion.identity);
		}

		num = 12;
		deltaAngle = 360f / num;
		for(int i = 0; i < num; ++i) {
			rad = (deltaAngle * i) * Mathf.Deg2Rad;
			Instantiate(locateTiles[locateStack.Pop()], new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius * 2f, Quaternion.identity);
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
		var network = new CATANMapNetwork(radius, 1f);
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
		int type;
		for(int i = 0; i < locateNum; ++i) {
			type = locateStack.Pop();
			var tileObj = (GameObject)Instantiate(locateTiles[type], new Vector3(0f, 0f, i * radius) + offset, Quaternion.identity);
			tileObj.transform.SetParent(tileParent);
			network.AddTile(tileObj, (CATANUtil.MapTileType)type);	//タイルの頂点は中心から1の距離にある(radiusは使わない)
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

	/// <summary>
	/// ノードに建築
	/// 建築できた場合はtrueを返す
	/// </summary>
	private bool BuildObj(GameObject obj, CATANMapElement elem) {
		if(elem == null) return false;
		if(elem.isBuild) return false;
		Vector3 pos = elem.position;
		pos.y = yOffset;
		var building = (GameObject)Instantiate(homePref, pos, Quaternion.identity);
		elem.SetBuilding(building);
		return true;
	}

	/// <summary>
	/// 家の建築
	/// 建築できた場合はtrueを返す
	/// </summary>
	public bool BuildHome(Vector3 pos) {
		if(_network == null) return false;
		//最寄りのノードを探索を探索して建築
		return BuildObj(homePref, _network.GetNearNode(pos));
	}

	/// <summary>
	/// 都市の建築(拠点のアップグレード)
	/// 建築できた場合はtrueを返す
	/// </summary>
	public bool BuildCity(Vector3 pos) {
		if(_network == null) return false;
		//最寄りのノードを探索を探索して建築
		return BuildObj(cityPref, _network.GetNearNode(pos));
	}

	/// <summary>
	/// 街道の建築
	/// 建築できた場合はtrueを返す
	/// </summary>
	public bool BuildRoad(Vector3 pos) {
		if(_network == null) return false;
		//最寄りのノードを探索を探索して建築
		return BuildObj(roadPref, _network.GetNearLink(pos));
	}

	#endregion
}