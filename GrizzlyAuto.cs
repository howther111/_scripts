// 防術機ヘルグリズリー用スクリプト


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
	const int MASK_ALL = 0xff; /// 全部
    int aimMode = 0; //照準方式　敵機との高さの違いにより変化します
    float heightSwitch = 10f; //対空・対地照準モード切替基準値（デフォルトでは10m）
    float enemyHeight = 0f; //敵機高さ
    float selfHeight = 0f; //自機高さ
    float newHeight = 0f; //敵機と自機の高さの差　どうしてこうなった
    bool autoAim = true; //自動照準モードのオンオフ　完全自動ならtrue固定
    int count = 0; //カウント（一定時間ごとの周期的な動作に利用します）
    bool autoPilot = true; //自動操縦のオンオフ　この機体の場合はずっとON
    int random = 0; //乱数
    int chargeCount; //ミサイルのチャージ時間を測るためのカウンターです
    string attackAction = "Beamer"; //選択武装
    Vector3 startPosition; //開始位置（復帰用）
    bool moveStart = false; //友軍との合流フラグ
    string searchAction = "Standby"; //サーチというより移動方法
    bool friendFlg = false; //動作未確認:友軍機に追従するかどうか
    bool returnFlg = true; //動作未確認:味方も敵もいないときにリスポーン地点に帰還
    bool wallCheck = false; //正面に壁がある時に方向転換するかどうか

    //----------------------------------------------------------------------------------------------
    // ユーザー名取得
    //----------------------------------------------------------------------------------------------
    public override string GetUserName()
	{
        //↓に名前を入れないと動きません
		return "GAIDA";
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
        //グリズリーのAIをやっつけで自動化したので余計な変数が多々あります　よって解説付き

        //敵を捕捉していないときに索敵します
        if (ap.CheckEnemy() == false) {
            ap.SearchEnemy();
        }

        //敵と味方の高さ（Y軸）を記録
        enemyHeight = ap.GetEnemyPosition().y;
        selfHeight = ap.GetPosition().y;

        //敵がいるときは友軍との合流をあきらめ（1行目）、自機と敵機の高さの差を測ります（2行目）
        if (ap.CheckEnemy() == true) {
            friendFlg = false;
            newHeight = Mathf.Abs(enemyHeight - selfHeight);
        }

        //カウント（一定時間ごとの周期的な動作に利用します）
        if (count < 3000) {
            count = count + 1;
        } else {
            count = 0;
        }

        //20フレームごとに乱数を更新します
        if (count % 20 == 0) {
            random = Random.Range(0, 1000);
        }

        //ターゲットリセット
        ap.SearchThreat(MASK_ALL, 1000f);
        if (count % 600 == 0) {
            ap.SetCounterSearch(500);
        }

        //敵の方向・距離・相対的な移動速度を取得
        float enemyDistance = ap.GetEnemyDistance();
        float enemyAngleR = ap.GetEnemyAngleR();
        float enemyAngleU = ap.GetEnemyAngleU();
        Vector3 enemyVelocity = ap.GetEnemyVelocity() - ap.GetVelocity();

        //味方との連携用
        int friendDistance = ap.GetFriendDistance();
        if (!ap.CheckFriend() && friendFlg) {
            ap.SearchFriend("");
            moveStart = true;
        }

        if (friendDistance < 200 || !ap.CheckFriend() || ap.CheckEnemy() || !friendFlg) {
            moveStart = false;
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

        // 移動方法
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

        //ランダムにジャンプ
        if (count % 20 == 0 && random % 10 == 0) {
            ap.StartAction("上昇", 10);
        }

        //AI操縦時の武装選択
        if (autoPilot && ap.CheckEnemy()) {
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

        //対地・対空エイミングモード自動切替
        if (newHeight > heightSwitch && autoAim) {
            aimMode = 4;
            ap.StartAction("AIM1", 1);
            ap.EndAction("AIM2");
        }
        if (newHeight <= heightSwitch && autoAim) {
            aimMode = 3;
            ap.StartAction("AIM2", 1);
            ap.EndAction("AIM1");
        }

        //自動攻撃（エネルギー切れ防止機構付き）
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

        //エイム用データ取得
        float ed = enemyDistance * 0.003f;
        Vector3 mv = ap.MulVec(enemyVelocity, ed);
        Vector3 estPos = ap.AddVec(ap.GetEnemyPosition(), mv);
        ap.Aim(estPos);
    }
}
