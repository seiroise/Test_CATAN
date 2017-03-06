using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ゲームを駆動させるマン
/// </summary>
public class CATANEngine : MonoBehaviour {

	[Header("Component")]
	[SerializeField]
	private CATANMapCreater map;
	private CATANMapNetwork network;

	//プレイヤー関連
	private List<CATANPlayer> players;
	private int playerNum;
	private int maxPlayerNum;
	private int hostPlayerIndex;

	//処理関連
	private Coroutine mainThread;

	//認証ID
	private string turnGuid;

	#region UnityEvent

	private void Awake() {
		players = new List<CATANPlayer>();
	}

	#endregion

	#region Function

	/// <summary>
	/// メインスレッドの開始
	/// </summary>
	public void StartMainThread() {
		if(!(players == null || players.Count < 2 || map)){
			Debug.LogError("ゲームを開始できません。");
			return;
		}
		playerNum = players.Count;
		mainThread = StartCoroutine(MainThread());
	}

	/// <summary>
	/// プレイヤーの追加
	/// プレイヤー用の銀行を返す
	/// </summary>
	public CATANPlayerBank AddPlayer(CATANPlayer player) {
		if(player == null) {
			Debug.LogError("不正なプレイヤーです");
			return null;
		}
		if(players.Count < maxPlayerNum) {
			Debug.LogError("プレイヤーの数が上限です");
			return null;
		}
		//プレイヤーの追加
		players.Add(player);
		//プレイヤーに対応するBankを返す
		return new CATANPlayerBank(this, map);
	}

	/// <summary>
	/// ターン毎のGUIDを設定
	/// </summary>
	private void SetTurnGuid() {
		turnGuid = System.Guid.NewGuid().ToString();
	}

	/// <summary>
	/// ターン毎に設定されているGUIDの確認
	/// </summary>
	public bool CheckTurnGuid(string id) {
		if(string.Equals(turnGuid, id)) return true;
		return false;
	}

	#endregion

	#region Engine

	/// <summary>
	/// メインスレッド
	/// </summary>
	private IEnumerator MainThread() {
		//マップの作成
		network = map.Create();
		//Guidの発行
		SetTurnGuid();
		//初期配置
		yield return StartCoroutine(InitLocate());
		//ターンループ
		while(true) {
			//Guidの発行
			SetTurnGuid();
			//サイコロ振る
			ThrowDice();
			//資源分配

			//親プレイヤー行動
			yield return StartCoroutine(players[hostPlayerIndex].Action());
			//子プレイヤー行動
			for(int i = 1; i < playerNum; ++i) {
				yield return StartCoroutine(players[(hostPlayerIndex + i) % playerNum].Negotiate());
			}
			//ターン終了処理
			hostPlayerIndex = (hostPlayerIndex + 1) % playerNum;
		}
	}

	/// <summary>
	/// 初期配置
	/// </summary>
	private IEnumerator InitLocate() {
		for(int i = 0; i < players.Count; ++i) {
			yield return StartCoroutine(players[i].PrimaryInitLocate());
		}
		for(int i = players.Count - 1; i >= 0; --i) {
			yield return StartCoroutine(players[i].SecondaryInitLocate());
		}
	}

	/// <summary>
	/// サイコロを振る
	/// </summary>
	private void ThrowDice() {
		int pip = Random.Range(1, 7) + Random.Range(1, 7);
		//出目処理

		//頂点を操作する
	}

	#endregion
}