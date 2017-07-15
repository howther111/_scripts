// 防術機ヒュペルボレア用スクリプト
// エネルギーマネージャのみ試験的に実装

// LuaからC#に移行する際の参考用にtrainer.txt(oldフォルダにあるLuaスクリプト)をC#で書き直したもの

using UnityEngine;

public class HyperboreaPS : UserScript
{
	// 攻撃検出用マスク
	const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    int aimMode = 0;

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
        //索敵(前方優先で近い敵を選択)
        if (ap.CheckEnemy() == false && aimMode == 2) {
            ap.SearchEnemy();
        }
        int enemyDistance = ap.GetEnemyDistance();

        // 攻撃
        int energy = ap.GetEnergy();
		if(energy > 15 && Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }
        if (energy > 10 && Input.GetMouseButton(1) && !Input.GetMouseButton(0)) {
            ap.StartAction("ATK2", -1);
        } else {
            ap.EndAction("ATK2");
        }

        //サイドブースト
        if (energy > 25 && Input.GetKey(KeyCode.Q)) {
            ap.StartAction("Ldash", -1);
        } else if (energy < 10 || !Input.GetKey(KeyCode.Q)) {
            ap.EndAction("Ldash");
        }
        if (energy > 25 && Input.GetKey(KeyCode.E)) {
            ap.StartAction("Rdash", -1);
        } else if (energy < 10 || !Input.GetKey(KeyCode.E)) {
            ap.EndAction("Rdash");
        }

        //サイコスキャニングモード
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            aimMode = 1;
            ap.StartAction("AIM1", -1);
            ap.EndAction("AIM2");
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            aimMode = 2;
            ap.StartAction("AIM1", -1);
            ap.StartAction("AIM2", -1);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            aimMode = 0;
            ap.EndAction("AIM1");
            ap.EndAction("AIM2");
        }

        //ターゲットリセット
        ap.SelectEnemy(KeyCode.Alpha2);

        //情報表示
        ap.Print(0, "Enemy : " + ap.GetEnemyName());
        ap.Print(1, "Distance : " + enemyDistance);
        ap.Print(2, "AngleR : " + ap.GetEnemyAngleR());
        ap.Print(3, "AimMode : " + aimMode);

        // エイム & 攻撃(敵の速度と距離を考慮して目標座標を補正)
        Vector3 ev = ap.GetEnemyVelocity();
        float ed = enemyDistance * 0.002f;
        Vector3 mv = ap.MulVec(ev, ed);

        if (aimMode == 2) {
            Vector3 estPos = ap.AddVec(ap.GetEnemyPosition(), mv);
            ap.Aim(estPos);
        }
    }
}
