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
        //位置取得
        ap.Print(0, "Position : " + ap.GetPosition());
        ap.Print(1, "Pitch : " + ap.GetPitch());
        ap.Print(2, "Direction : " + ap.GetDirection());
        ap.Print(3, "Back : " + ap.GetBank());

        // 攻撃
        int energy = ap.GetEnergy();
		if(energy > 15 && Input.GetMouseButton(0)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }
        if (energy > 10 && Input.GetMouseButton(1)) {
            ap.StartAction("ATK2", -1);
        } else {
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
    }
}
