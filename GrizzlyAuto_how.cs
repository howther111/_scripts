    // 防術機グリズリー用スクリプト
// エネルギーマネージャのみ試験的に実装

// LuaからC#に移行する際の参考用にtrainer.txt(oldフォルダにあるLuaスクリプト)をC#で書き直したもの

using UnityEngine;

public class GrizzlyAuto: UserScript
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
    bool autoAim = true;
    int count = 0;
    bool autoPilot = true;
    int random = 0;
    int chargeCount;
    string attackAction = "Beamer";
    Vector3 startPosition;
    bool moveStart = false;
    int sampleDis = 0;
    string searchAction = "Standby";
    int checker = 0;
    bool friendFlg = false;
    bool returnFlg = true; //実装予定:味方も敵もいないときにリスポーン地点に帰還
    bool wallCheck = false; //実装予定:正面に壁がある時に方向転換

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
        ap.SetAimTolerance(15); // エイム許容誤差設定
        ap.SetAutoLockon(true); // ランチャーの自動ロックオン開始
        startPosition = ap.GetPosition();   // 開始座標保存
    }

	//----------------------------------------------------------------------------------------------
	// 更新処理
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
        if (ap.CheckEnemy() == false && (aimMode == 3 || aimMode == 4)) {
            ap.SearchEnemy();
        }

        enemyHeight = ap.GetEnemyPosition().y;
        selfHeight = ap.GetPosition().y;

        if (ap.CheckEnemy() == true) {
            friendFlg = false;
            newHeight = Mathf.Abs(enemyHeight - selfHeight);
        }

        //カウント
        if (count < 3000) {
            count = count + 1;
        } else {
            count = 0;
        }

        if (count % 20 == 0) {
            random = Random.Range(0, 1000);
        }

        //ターゲットリセット
        ap.SearchThreat(MASK_ALL, 1000f);
        if (count % 600 == 0) {
            ap.SetCounterSearch(500);
        }

        float enemyDistance = ap.GetEnemyDistance();
        float enemyAngleR = ap.GetEnemyAngleR();
        float enemyAngleU = ap.GetEnemyAngleU();
        Vector3 enemyVelocity = ap.GetEnemyVelocity() - ap.GetVelocity();

        //味方との連携
        int friendDistance = ap.GetFriendDistance();
        if (!ap.CheckFriend() && friendFlg) {
            ap.SearchFriend("");
        }

        if (friendDistance < 200 || !ap.CheckFriend() || ap.CheckEnemy() || !friendFlg) {
            moveStart = false;
            sampleDis = 0;
            checker = 0;
        }

        //壁検知
        int wallZ = ap.MeasureClearanceToWall(0, -5, 100, 25);
        int wallXL = ap.MeasureClearanceToWall(100, -5, 0, 10);
        int wallXR = ap.MeasureClearanceToWall(-100, -5, 0, 10);
        if (wallZ < 25 || wallXL < 10 || wallXR < 10) {
            wallCheck = true;
        } else {
            wallCheck = false;
        }

        // 移動による探索
        int random2 = random / 10;
        if (!ap.CheckEnemy() && !moveStart && !wallCheck) {
            //ランダム探索
            searchAction = "Random";
            if (random2 < 20) {
                ap.StartAction("s", 1);
            } else if (random2 < 25) {
                ap.StartAction("a", 1);
            } else if (random2 < 30) {
                ap.StartAction("d", 1);
            } else {
                ap.StartAction("w", 1);
            }
        } else if (!ap.CheckEnemy() && friendDistance >= 50 && ap.CheckFriend() && friendFlg && !wallCheck) {
            searchAction = "MoveToFriend";
            //味方と合流
            int ang = ap.GetFriendAngleR();
            if (ang > 30) {
                ap.StartAction("d", 1);
            } else if (ang < -30) {
                ap.StartAction("a", 1);
            } else {
                ap.StartAction("w", 1);
            }
        } else if (ap.CheckEnemy() && enemyDistance <= 50 && !wallCheck) {
            //回避機動
            searchAction = "Back";
            if (random2 < 70) {
                ap.StartAction("w", 1);
            } else if (random2 > 65) {
                ap.StartAction("a", 1);
            } else if (random2 > 60) {
                ap.StartAction("d", 1);
            } else {
                ap.StartAction("s", 1);
            }
        } else if (wallCheck) {
            //壁回避機動
            searchAction = "AvoidWall";
            if (random2 < 20 || wallXR < 10) {
                ap.StartAction("d", 1);
            } else if (random2 < 40 || wallXL < 10) {
                ap.StartAction("a", 1);
            } else if (random2 < 60) {
                ap.StartAction("d", 1);
            } else if (random2 < 80) {
                ap.StartAction("a", 1);
            } else {
                ap.StartAction("w", 1);
            }
        } else {
            //通常交戦機動
            searchAction = "Battle";
            if (random2 < 20) {
                ap.StartAction("s", 1);
            } else if (random2 < 25) {
                ap.StartAction("a", 1);
            } else if (random2 > 60) {
                ap.StartAction("w", 1);
            } else if (random2 > 55) {
                ap.StartAction("d", 1);
            }
        }

        //ランダムジャンプ
        if (count % 20 == 0 && random % 10 == 0) {
            ap.StartAction("上昇", 10);
        }

        //AI操縦
        if (autoPilot && ap.CheckEnemy()) {
            //武装選択
            if (enemyDistance < 100 && random % 3 == 0) {
                attackAction = "Beamer";
            } else if (enemyDistance < 100 && random % 3 == 1) {
                attackAction = "Cannon";
            } else if (enemyDistance < 100 && random % 3 == 2) {
                attackAction = "Escape";
            } else if (enemyDistance < 200 && random % 3 == 0) {
                attackAction = "Escape";
            } else if (enemyDistance < 200 && random % 3 != 0) {
                attackAction = "Cannon";
            } else if (random % 3 == 0) {
                attackAction = "Escape";
            } else if (random % 3 == 1) {
                attackAction = "Cannon";
            } else if (random % 3 == 2) {
                attackAction = "Missile";
            } else {
                attackAction = "Cannon";
            }
        } else {
            attackAction = "Escape";
        }

        //対地対空機銃自動エイミングモード
        if (count % 100 == 10) {
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

        // 攻撃
        int energy = ap.GetEnergy();
		if(energy > 10 && (attackAction == "Cannon")) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }
        if (energy > 15 && (attackAction == "Beamer")) {
            ap.StartAction("ATK2", -1);
        } else {
            ap.EndAction("ATK2");
        }
        if (energy > 5 && (attackAction == "Missile")) {
            ap.StartAction("ATK3", -1);
            chargeCount++;
        } else if (energy > 5 && chargeCount > 0 && chargeCount < 2000) {
            chargeCount++;
        } else {
            chargeCount = 0;
            ap.EndAction("ATK3");
        }

        //情報表示
        ap.Print(0, "Enemy : " + ap.GetEnemyName());
        ap.Print(1, "Distance : " + enemyDistance);
        ap.Print(2, "AngleR : " + ap.GetEnemyAngleR());
        ap.Print(3, "AimMode : " + aimMode);
        ap.Print(4, "EnemyHeight : " + ap.GetEnemyPosition().y);
        ap.Print(5, "SelfHeight : " + ap.GetPosition().y);
        ap.Print(6, "Count : " + count);
        ap.Print(7, "Weapon : " + attackAction);
        ap.Print(8, "Search : " + searchAction);
        ap.Print(9, "WallCheck : " + wallCheck);

        // エイム
        float ed = enemyDistance * 0.003f;
        Vector3 mv = ap.MulVec(enemyVelocity, ed);

        if (aimMode == 3 || aimMode == 4) {
            //機銃オートエイム
            Vector3 estPos = ap.AddVec(ap.GetEnemyPosition(), mv);
            ap.Aim(estPos);
        }

        oldHeight = newHeight;
    }
}
