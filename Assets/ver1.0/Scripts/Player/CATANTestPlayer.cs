using UnityEngine;
using System.Collections;

//テストのプレイヤー
public class CATANTestPlayer : CATANPlayer{

	#region VirtualFunction

	public override IEnumerator PrimaryInitLocate() {
		//とりあえず重み付け的に一番の箇所をとる
		while(true) {
			var n = playerBank.GetMostWeightNode();
			if(n != null) {
				playerBank.BuildHome(n.position, true);
				break;
			}
			yield return 0;
		}
	}

	#endregion
}