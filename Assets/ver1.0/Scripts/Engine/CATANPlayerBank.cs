using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤー銀行
/// プレイヤー毎の資源、発展、使用済み発展などを管理
/// 基本的にプレイヤーが自身の持つ資源にアクセスするためのインタフェース
/// </summary>
public class CATANPlayerBank {

	//資材名称
	private const string RES_LUMBER = "Lumber";
	private const string RES_WOOL = "wool";
	private const string RES_GRAIN = "Grain";
	private const string RES_BRICK = "Brick";
	private const string RES_ORE = "Ore";

	//エンジン
	private CATANEngine engine;

	//マップ
	private CATANMapCreater map;

	//建物
	private List<CATANMapHome> homes;
	private List<CATANMapRoad> roads;

	//カード
	private Dictionary<string, int> resCards;
	private Dictionary<string, int> devCards;
	private Dictionary<string, int> usedDevCards;

	public CATANPlayerBank(CATANEngine engine, CATANMapCreater map) {
		this.engine = engine;
		this.map = map;
		resCards = new Dictionary<string, int>();
		devCards = new Dictionary<string, int>();
		usedDevCards = new Dictionary<string, int>();
	}

	#region Function

	/// <summary>
	/// 資源の確認
	/// 指定した資源が指定された数だけある場合にtrueを返す
	/// </summary>
	public bool CheckResources(string res, int num) {
		if(resCards.ContainsKey(res)) {
			if(resCards[res] >= num) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 資源の消費
	/// 消費できない場合は何もしない
	/// </summary>
	private void UseResources(string res, int num) {
		if(CheckResources(res, num)) {
			resCards[res] -= num;
		}
	}

	/// <summary>
	/// 資源の追加
	/// 指定した資源を指定した数だけ追加する
	/// 追加にはターン毎の認証IDが必要
	/// </summary>
	public void AddResources(string res, int num, string guid) {
		//数値確認
		if(num <= 0) return;
		//guidの確認
		if(!(engine || engine.CheckTurnGuid(guid))) return;
		//資源の追加
		if(!resCards.ContainsKey(res)) {
			resCards.Add(res, num);
		} else {
			resCards[res] += num;
		}
	}

	/// <summary>
	/// 発展カードの確認
	/// 指定された発展カードが存在する場合にtrueを返す
	/// </summary>
	public bool CheckDevCard(string dev) {
		if(devCards.ContainsKey(dev)) {
			if(devCards[dev] > 0) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 発展カードの消費
	/// </summary>
	private void UseDevCard(string dev) {
		if(!CheckDevCard(dev)) return;
		devCards[dev]--;
	}

	/// <summary>
	/// 発展カードの追加
	/// 追加にはターン毎の認証IDが必要
	/// </summary>
	public void AddDevCard(string dev, string guid) {
		//guidの確認
		if(!(engine || engine.CheckTurnGuid(guid))) return;
		//資源の追加
		if(!resCards.ContainsKey(dev)) {
			resCards.Add(dev, 1);
		} else {
			resCards[dev]++;
		}
	}

	/// <summary>
	/// 現段階で建築可能な最も重みの大きいノードを返す
	/// 返すノードが存在しない場合はnullを返す
	/// </summary>
	public CATANMapNode GetMostWeightNode() {
		if(map.network == null) return null;
		return map.network.GetMostWeightNode();
	}

	#endregion

	#region BuildFunction

	/// <summary>
	/// 拠点の建設
	/// 第二引数は強制的に建てるか
	/// </summary>
	public void BuildHome(Vector3 pos, bool forced = false) {
		//ノードの探索
		if(map.network == null) return;
		//資材確認
		if(!forced) {
			//Wool : 1, Lumber : 1, Brick : 1, Grain : 1
			if(!(CheckResources(RES_WOOL, 1) &&
			     CheckResources(RES_LUMBER, 1) &&
			     CheckResources(RES_BRICK, 1) &&
			     CheckResources(RES_GRAIN, 1))) return;
		}
		//建築
		if(!map.BuildHome(pos)) return;
		//資材を減らす
		if(!forced) {
			UseResources(RES_WOOL, 1);
			UseResources(RES_LUMBER, 1);
			UseResources(RES_BRICK, 1);
			UseResources(RES_GRAIN, 1);
		}
	}

	/// <summary>
	/// 拠点の強化
	/// </summary>
	public void UpgradeHome(Vector3 pos, bool forced = false) {
		//ノードの探索
		if(map.network == null) return;
		var node = map.network.GetNearNode(pos);

		//資材確認
		//
	}

	/// <summary>
	/// 道の建設
	/// 第二引数は強制的に建てるか
	/// </summary>
	public void BuildRoad(Vector3 pos, bool forced = false) {
		//ノードの探索
		if(map.network == null) return;
		//資材確認
		if(!forced) {
			//Lumber : 1, Brick : 1
			if(!(CheckResources(RES_LUMBER, 1) && 
			     CheckResources(RES_BRICK, 1))) return;
		}
		//建築
		if(!map.BuildRoad(pos)) return;
		//資材を減らす
		if(!forced) {
			UseResources(RES_LUMBER, 1);
			UseResources(RES_BRICK, 1);
		}
	}

	#endregion
}