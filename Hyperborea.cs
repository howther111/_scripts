// 防術機ヒュペルボレア用スクリプト
// エネルギーマネージャのみ試験的に実装

// LuaからC#に移行する際の参考用にtrainer.txt(oldフォルダにあるLuaスクリプト)をC#で書き直したもの

using UnityEngine;

public class Hyperborea : UserScript
{
	// 攻撃検出用マスク
	const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    int gunMode = 1; //射撃モード
    bool missile = false; //ミサイルオンオフ

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
		if(Input.GetMouseButton(0)) {
            if (energy > 15 && gunMode == 1) {
                ap.StartAction("ATK1-1", 1);
            }
            if (energy > 15 && gunMode == 2) {
                ap.StartAction("ATK1-2", 1);
            }
            if (gunMode == 3 && energy > 30) {
                ap.StartAction("ATK1-3", 1);
            }
        }
        if (energy > 10 && Input.GetMouseButtonDown(1) && !missile) {
            missile = true;
            ap.StartAction("ATK2", -1);
        } else if (missile && (Input.GetMouseButtonDown(1) || energy <= 10)) {
            missile = false;
            ap.EndAction("ATK2");
        }

        //サイドブースト
        if (energy > 40 && Input.GetKey(KeyCode.Q)) {
            ap.StartAction("Ldash", -1);
        } else if (energy < 10 || !Input.GetKey(KeyCode.Q)) {
            ap.EndAction("Ldash");
        }
        if (energy > 40 && Input.GetKey(KeyCode.E)) {
            ap.StartAction("Rdash", -1);
        } else if (energy < 10 || !Input.GetKey(KeyCode.E)) {
            ap.EndAction("Rdash");
        }

        //射撃モード切替
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            if (gunMode == 1) {
                gunMode = 2;
            } else if (gunMode == 2) {
                gunMode = 3;
            } else if (gunMode == 3) {
                gunMode = 1;
            }
        }
    }
}
