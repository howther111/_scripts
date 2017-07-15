// 防術機アズラエル用スクリプト
// エネルギーマネージャのみ試験的に実装

using UnityEngine;

public class Azrael : UserScript
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
    bool sword = false;
    bool spin = false;
    int missileMode = 1;

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

        //銃
		if(energy > 10 && Input.GetMouseButton(0)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }

        //ソード
        if (energy > 30 && (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Q)) && !sword) {
            sword = true;
        } else if (Input.GetKeyDown(KeyCode.R) && sword) {
            sword = false;
        }
        if (sword && energy > 10 && !spin) {
            ap.StartAction("ATK3BF", 1);
        }

        if (energy > 30 && !spin && Input.GetKeyDown(KeyCode.E)) {
            ap.StartAction("spin", -1);
            sword = false;
            spin = true;
        } else if (spin && (energy < 10 || sword || Input.GetKeyDown(KeyCode.E))) {
            ap.EndAction("spin");
            spin = false;
        }

        //ミサイル
        if (!missile && energy > 15 && Input.GetMouseButtonDown(1)) {
            ap.StartAction("ATK2", -1);
            if (missileMode == 1) {
                ap.StartAction("ATK2-1", -1);
            } else if (missileMode == 2) {
                ap.StartAction("ATK2-2", -1);
            }
            missile = true;
        } else if ((missile && Input.GetMouseButtonDown(1)) || energy < 10) {
            ap.EndAction("ATK2");
            ap.EndAction("ATK2-1");
            ap.EndAction("ATK2-2");
            missile = false;
        }

        //ミサイル切り替え
        if (Input.GetMouseButtonDown(2) && missileMode == 1 && !missile) {
            missileMode = 2;
        } else if (Input.GetMouseButtonDown(2) && missileMode == 2 && !missile) {
            missileMode = 1;
        }
    }
}
