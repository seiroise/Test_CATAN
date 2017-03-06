using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CATANPlayer : MonoBehaviour {

	protected CATANPlayerBank playerBank;

	#region VirtualFunction

	/// <summary>
	/// 一次初期配置フェイズ
	/// </summary>
	public virtual IEnumerator PrimaryInitLocate() {
		yield return 0;
	}

	/// <summary>
	/// 二次初期配置フェイズ
	/// </summary>
	public virtual IEnumerator SecondaryInitLocate() {
		yield return 0;
	}

	/// <summary>
	/// 行動フェイズ
	/// </summary>
	public virtual IEnumerator Action() {
		yield return 0;
	}

	/// <summary>
	/// 交渉フェイズ
	/// </summary>
	public virtual IEnumerator Negotiate() {
		yield return 0;
	}

	#endregion

	#region Function

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize(CATANPlayerBank playerBank) {
		this.playerBank = playerBank;
	}

	/// <summary>
	/// マップ作成後
	/// </summary>
	public void OnMapCreate() {
		
	}

	#endregion
}