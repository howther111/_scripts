// 防術機DCエリアル用スクリプト
// エネルギーマネージャのみ試験的に実装

// LuaからC#に移行する際の参考用にtrainer.txt(oldフォルダにあるLuaスクリプト)をC#で書き直したもの

using UnityEngine;

public class DC : UserScript
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
    bool shield = false; //シールドオンオフ
    int cameraMode = 1; //カメラモード

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
        //射撃モード切替
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            if (gunMode == 1) {
                gunMode = 2;
            } else if (gunMode == 2) {
                gunMode = 1;
            }
        }

        // 射撃制御
        int energy = ap.GetEnergy();
		if(Input.GetMouseButton(0)) {
            if (energy > 5 && gunMode == 1) {
                ap.StartAction("ATK1-1", 1);
            }
            if (energy > 20 && gunMode == 2) {
                ap.StartAction("ATK1-2", 1);
            }
        }
        if (energy > 10 && Input.GetMouseButtonDown(1) && !missile) {
            missile = true;
            ap.StartAction("ATK2", -1);
        } else if (missile && (Input.GetMouseButtonDown(1) || energy <= 5)) {
            missile = false;
            ap.EndAction("ATK2");
        }
        if (energy > 50 && Input.GetMouseButtonDown(2) && !shield) {
            shield = true;
            ap.StartAction("Shield", -1);
        } else if (shield && Input.GetMouseButtonDown(2)) {
            shield = false;
            ap.EndAction("Shield");
        }

        //カメラモード切替
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (cameraMode == 1) {
                cameraMode = 2;
            } else if (cameraMode == 2) {
                cameraMode = 3;
            } else if (cameraMode == 3) {
                cameraMode = 1;
            }
        }

        //カメラ制御
        if (cameraMode == 2) {
            ap.StartAction("Camera1", 1);
        } else if (cameraMode == 3) {
            ap.StartAction("Camera2", 1);
        }
    }
}
