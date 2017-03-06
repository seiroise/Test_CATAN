using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	[SerializeField]
	private CATANEngine engine;

	private void Update() {
		if(Input.GetMouseButtonUp(0)) {
			EngineStart();
		}
	}

	private void EngineStart() {
		if(!engine) return;

		//テストプレイヤーの追加
		var p = gameObject.AddComponent<CATANTestPlayer>();
		var bank = engine.AddPlayer(p);
		p.Initialize(bank);

		p = gameObject.AddComponent<CATANTestPlayer>();
		bank = engine.AddPlayer(p);
		p.Initialize(bank);

		p = gameObject.AddComponent<CATANTestPlayer>();
		bank = engine.AddPlayer(p);
		p.Initialize(bank);

		p = gameObject.AddComponent<CATANTestPlayer>();
		bank = engine.AddPlayer(p);
		p.Initialize(bank);

		engine.StartMainThread();

		Destroy(this);
	}
}