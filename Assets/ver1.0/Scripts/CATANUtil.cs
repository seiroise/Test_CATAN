using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// カタン用ユーティリティ
/// </summary>
public class CATANUtil {
	
	/// <summary>
	/// マップタイルタイプ
	/// </summary>
	public enum MapTileType {
		Desert,		//砂漠
		Forest,		//森林	-> 木材(Lumber)
		Pastures,	//牧草地	-> 羊毛(Wool)
		Fields,     //畑		-> 小麦(Grain)
		Hills,		//丘陵	-> レンガ(Brick)
		Mountains,	//山地	-> 鉱石(Ore)
		NoUse		//未使用
	}
}