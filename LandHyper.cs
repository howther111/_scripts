// 防術機ランドヒュペル用スクリプト

using UnityEngine;

public class LandHyper : UserScript
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
    bool camera = false;
    int cameraZoom = 1;
    int gunMode = 1;

    //----------------------------------------------------------------------------------------------
    // ユーザー名取得
    //s----------------------------------------------------------------------------------------------
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
        //移動
        if (Input.GetKey(KeyCode.W)) {
            ap.StartAction("foot", 1);
            ap.StartAction("Rforth", 1);
        } else if (Input.GetKey(KeyCode.S)) {
            ap.StartAction("foot2", 1);
            ap.StartAction("Rforth", 1);
        } else if (Input.GetKey(KeyCode.A)) {
            ap.StartAction("foot", 1);
            ap.StartAction("Rback", 1);
        } else if (Input.GetKey(KeyCode.D)) {
            ap.StartAction("foot2", 1);
            ap.StartAction("Rback", 1);
        }

        // 攻撃
        //銃・キャノン
        if (gunMode == 1 && Input.GetKeyDown(KeyCode.Q)) {
            gunMode = 2;
        } else if (gunMode == 2 && Input.GetKeyDown(KeyCode.Q)) {
            gunMode = 1;
        }
        int energy = ap.GetEnergy();
		if(energy > 10 && Input.GetMouseButton(0) && gunMode == 1) {
            ap.StartAction("ATK1", 1);
        } else if (energy > 50 && Input.GetMouseButton(0) && gunMode == 2) {
            ap.StartAction("ATK4", 1);
        }

        //ミサイル  
        if (!missile && energy > 15 && Input.GetMouseButtonDown(1)) {
            ap.StartAction("ATK2", -1);
            missile = true;
        } else if ((missile && Input.GetMouseButtonDown(1)) || energy < 10) {
            ap.EndAction("ATK2");
            missile = false;
        }

        //ソード
        if (energy > 15 && Input.GetMouseButtonDown(2) && !sword) {
            ap.StartAction("ATK3", -1);
            sword = true;
        } else if ((Input.GetMouseButtonDown(2) || energy < 10) && sword) {
            ap.EndAction("ATK3");
            sword = false;
        }

        //カメラ
        if (Input.GetKeyDown(KeyCode.LeftShift) && !camera) {
            camera = true;
        } else if (Input.GetKeyDown(KeyCode.LeftShift) && camera) {
            camera = false;
        }

        if (camera && cameraZoom == 1) {
            ap.StartAction("Camera", 1);
            if (Input.GetKeyDown(KeyCode.E)) {
                cameraZoom = 2;
            }
        } else if (camera && cameraZoom == 2) {
            ap.StartAction("Camera2", 1);
            if (Input.GetKeyDown(KeyCode.E)) {
                cameraZoom = 1;
            }
        }
    }
}
