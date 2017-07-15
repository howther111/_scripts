    // 防術機グリズリー用スクリプト
// エネルギーマネージャのみ試験的に実装

// LuaからC#に移行する際の参考用にtrainer.txt(oldフォルダにあるLuaスクリプト)をC#で書き直したもの

using UnityEngine;

public class GrizzlyBM : UserScript
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
    int aimChange = 0;
    float oldHeight = 0f;
    float heightSwitch = 10f;
    float enemyHeight = 0f;
    float selfHeight = 0f;
    float newHeight = 0f;
    bool autoAim = false;
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
        //索敵(前方優先で近い敵を選択)
        if (ap.CheckEnemy() == false && (aimMode == 2 || aimMode == 3 || aimMode == 4)) {
            ap.SearchEnemy();
        }
        int enemyDistance = ap.GetEnemyDistance();

        enemyHeight = ap.GetEnemyPosition().y;
        selfHeight = ap.GetPosition().y;
        if (ap.CheckEnemy() ) {
            newHeight = Mathf.Abs(enemyHeight - selfHeight);
        } else {
            newHeight = 0f;
        }

        //対地対空機銃自動エイミングモード
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            autoAim = true;
            if (newHeight > heightSwitch) {
                aimMode = 4;
                ap.StartAction("AIM1", -1);
                ap.EndAction("AIM2");
            } else {
                aimMode = 3;
                ap.StartAction("AIM2", -1);
                ap.EndAction("AIM1");
            }
        }

        newHeight = (enemyHeight - selfHeight) * 2;
        if (newHeight > heightSwitch && oldHeight <= heightSwitch && autoAim) {
            aimMode = 4;
            ap.StartAction("AIM1", -1);
            ap.EndAction("AIM2");
        }
        if (newHeight < heightSwitch && oldHeight >= heightSwitch && autoAim) {
            aimMode = 3;
            ap.StartAction("AIM2", -1);
            ap.EndAction("AIM1");
        }

        //ターゲットリセット
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3)) {
            ap.ForgetEnemy();
        }
        ap.SelectEnemy(KeyCode.Alpha2);
        ap.SelectEnemy(KeyCode.Alpha3);

        // 攻撃
        int energy = ap.GetEnergy();
		if(energy > 10 && Input.GetMouseButton(0)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }
        if (energy > 15 && Input.GetMouseButton(1)) {
            ap.StartAction("ATK2", -1);
        } else {
            ap.EndAction("ATK2");
        }
        if (!missile && energy > 5 && Input.GetMouseButtonDown(2)) {
            ap.StartAction("ATK3", -1);
            missile = true;
        } else if (missile && (Input.GetMouseButtonDown(2) || energy < 5)) {
            ap.EndAction("ATK3");
            missile = false;
        }

        //エイムモード
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            aimMode = 1; //手動モード
            ap.StartAction("AIM1", -1);
            ap.EndAction("AIM2");
            autoAim = false;
        } else if (aimChange == 3) {
            aimMode = 3; //対地モード
            ap.StartAction("AIM2", -1);
            ap.EndAction("AIM1");
        } else if (aimChange == 4) {
            aimMode = 4; //対空モード
            ap.StartAction("AIM1", -1);
            ap.EndAction("AIM2");
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            aimMode = 0; //休止モード
            ap.EndAction("AIM1");
            ap.EndAction("AIM2");
            autoAim = false;
        }

        //情報表示
        ap.Print(0, "Enemy : " + ap.GetEnemyName());
        ap.Print(1, "Distance : " + enemyDistance);
        ap.Print(2, "AngleR : " + ap.GetEnemyAngleR());
        ap.Print(3, "AimMode : " + aimMode);
        ap.Print(4, "EnemyHeight : " + ap.GetEnemyPosition().y);
        ap.Print(5, "SelfHeight : " + ap.GetPosition().y);

        // エイム & 攻撃(敵の速度と距離を考慮して目標座標を補正)
        Vector3 ev = ap.GetEnemyVelocity() - ap.GetVelocity();
        float ed = enemyDistance * 0.003f;
        Vector3 mv = ap.MulVec(ev, ed);

        if (aimMode == 3 || aimMode == 4) {
            //機銃オートエイム
            Vector3 estPos = ap.AddVec(ap.GetEnemyPosition(), mv);
            ap.Aim(estPos);
        }

        oldHeight = newHeight;
    }
}
