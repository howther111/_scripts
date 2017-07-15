// 防術機ヒュペルノワール用スクリプト
// エネルギーマネージャのみ試験的に実装

using UnityEngine;

public class Hypernoir : UserScript
{
	// 攻撃検出用マスク
	const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    bool missile = false;

	//----------------------------------------------------------------------------------------------
	// ユーザー名取得
	//----------------------------------------------------------------------------------------------
	public override string GetUserName()
	{
		return "howther111";
	}

	//----------------------------------------------------------------------------------------------
	// 開始処理
	//----------------------------------------------------------------------------------------------
	public override void OnStart(AutoPilot ap)
	{

	}

	//----------------------------------------------------------------------------------------------
	// 更新処理
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		// 攻撃
		int energy = ap.GetEnergy();
		if(energy > 15 && Input.GetMouseButton(0)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }
        if (energy > 30 && Input.GetMouseButton(1)) {
            ap.StartAction("ATK3", -1);
        } else {
            ap.EndAction("ATK3");
        }
        if (!missile && energy > 15 && Input.GetMouseButtonDown(2)) {
            ap.StartAction("ATK2", -1);
            missile = true;
        } else if ((missile && Input.GetMouseButtonDown(2)) || energy < 10) {
            ap.EndAction("ATK2");
            missile = false;
        }
    }
}
