// 防術機ブリッツフォーゲル用スクリプト
// エネルギーマネージャのみ試験的に実装

using UnityEngine;

public class BlitzVogel : UserScript
{
	// 攻撃検出用マスク
	const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    bool sword = false;
    bool flightMode = false;
    bool autoPilot = false; //後々実装
    bool aim = false;
    const float heightSwitch = 10f;
    float enemyHeight = 0f;
    float selfHeight = 0f;
    float newHeight = 0f;
    bool friendFlg = false;
    int friendCount = 0;
    string searchAction = "Standby";
    int aimMode = 0;
    bool jetStriker = false;
    const float upDownVelLim = 10f;
    bool missile = false;

    //Alpha1:射撃システム起動
    //Alpha2:変形
    //Alpha3:自動照準システム起動
    //Alpha4:ターゲットリセット
    //Alpha5:フレンドサーチ
    //Alpha6:水中復帰

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
		if(energy > 20 && Input.GetMouseButton(0)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }
        if (Input.GetMouseButtonDown(2)) {
            if (energy > 10 && !sword) {
                ap.StartAction("ATK3", -1);
                sword = true;
            } else {
                ap.EndAction("ATK3");
                sword = false;
            }
        }
        if (energy > 10 && jetStriker) {
            ap.StartAction("ATK3", 1);
        }
        if (energy <= 10) {
            ap.EndAction("ATK3");
            sword = false;
        }
        if (energy > 10 && Input.GetMouseButtonDown(1) && !missile) {
            ap.StartAction("ATK2", -1);
            missile = true;
        } else if (Input.GetMouseButtonDown(1) || energy <= 10) {
            ap.EndAction("ATK2");
            missile = false;
        }

        //移動
        if (!flightMode) {
            //人型形態
            if (Input.GetKey(KeyCode.W)) {
                ap.StartAction("w", 1);
            }
            if (Input.GetKey(KeyCode.A)) {
                ap.StartAction("a", 1);
            }
            if (Input.GetKey(KeyCode.S)) {
                ap.StartAction("s", 1);
            }
            if (Input.GetKey(KeyCode.D)) {
                ap.StartAction("d", 1);
            }
            if (Input.GetKey(KeyCode.Space)) {
                ap.StartAction("上昇", 1);
            }
            if (Input.GetKey(KeyCode.F)) {
                ap.StartAction("下降", 1);
            }
            if ((Input.GetKeyDown(KeyCode.Alpha1) && aim) || Input.GetKeyDown(KeyCode.Q)) {
                ap.EndAction("AIM1");
                aim = false;
            } else if (Input.GetKeyDown(KeyCode.Alpha1) && !aim || Input.GetKeyUp(KeyCode.Q)) {
                ap.StartAction("AIM1", -1);
                aim = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                ap.StartAction("change", -1);
                ap.EndAction("AIM1");
                flightMode = true;
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                ap.StartAction("change", -1);
                ap.EndAction("AIM1");
                flightMode = true;
                autoPilot = true;
                jetStriker = true;
            } else {
                jetStriker = false;
            }
        } else {
            //飛行形態
            if (jetStriker && Input.GetKeyDown(KeyCode.E)) {
                jetStriker = false;
                autoPilot = false;
            } else if (!jetStriker && Input.GetKeyDown(KeyCode.E)) {
                jetStriker = true;
                autoPilot = true;
                ap.ForgetEnemy();
            }
            if (Input.GetKey(KeyCode.W) || jetStriker) {
                ap.StartAction("FMforth", 1);
            }
            if (Input.GetKey(KeyCode.A)) {
                ap.StartAction("a", 1);
            }
            if (Input.GetKey(KeyCode.S)) {
                ap.StartAction("FMback", 1);
            }
            if (Input.GetKey(KeyCode.D)) {
                ap.StartAction("d", 1);
            }
            if (Input.GetKey(KeyCode.Space)) {
                ap.StartAction("FMup", 1);
            }
            if (Input.GetKey(KeyCode.F)) {
                ap.StartAction("FMdown", 1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                ap.EndAction("change");
                flightMode = false;
                ap.StartAction("AIM1", -1);
                aim = true;
            }
        }

        //自動照準システム
        float enemyDistance = ap.GetEnemyDistance();
        float enemyAngleR = ap.GetEnemyAngleR();
        float enemyAngleU = ap.GetEnemyAngleU();
        Vector3 enemyVelocity = ap.GetEnemyVelocity() - ap.GetVelocity();
        enemyHeight = ap.GetEnemyPosition().y;
        selfHeight = ap.GetPosition().y;
        //オートパイロット
        if (autoPilot) {
            if (!ap.CheckEnemy()) {
                ap.SearchEnemy();
            }

            aim = true;

            //ターゲットリセット
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                ap.ForgetEnemy();
            }
            ap.SelectEnemy(KeyCode.Alpha4);

            if (ap.CheckEnemy()) {
                newHeight = Mathf.Abs(enemyHeight - selfHeight);
            } else {
                newHeight = 0;
            }

            if (newHeight > heightSwitch && !Input.GetKey(KeyCode.Q)) {
                aimMode = 1;
                if (!flightMode) {
                    ap.StartAction("AIM1", -1);
                    if (enemyAngleR < -15 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("a", 1);
                    }
                    if (enemyAngleR > 15 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("d", 1);
                    }
                } else {
                    if (enemyAngleR < -5 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("a", 1);
                    }
                    if (enemyAngleR > 5 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("d", 1);
                    }
                    if (enemyHeight - selfHeight > heightSwitch && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.F)) {
                        ap.StartAction("FMup", 1);
                    }
                    if (enemyHeight - selfHeight < -heightSwitch && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.F)) {
                        ap.StartAction("FMdown", 1);
                    }
                }
            } else if (!Input.GetKey(KeyCode.Q)) {
                aimMode = 2;
                if (!flightMode) {
                    ap.StartAction("AIM1", -1);
                    if (enemyAngleR < -15 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("a", 1);
                    }
                    if (enemyAngleR > 15 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("d", 1);
                    }
                } else {
                    if (enemyAngleR < -5 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("a", 1);
                    }
                    if (enemyAngleR > 5 && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
                        ap.StartAction("d", 1);
                    }
                    if (enemyHeight - selfHeight < heightSwitch && enemyHeight - selfHeight > -heightSwitch && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.F)) {
                        if (ap.GetVelocity().y < -10) {
                            ap.StartAction("FMup", 1);
                        }
                        if (ap.GetVelocity().y > 10) {
                            ap.StartAction("FMdown", 1);
                        }
                    }
                }
            } else {
                aimMode = 0;
                ap.EndAction("AIM1");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) || (Input.GetKeyDown(KeyCode.Alpha1) && aim)) {
                ap.ForgetEnemy();
                autoPilot = false;
                jetStriker = false;
                if (!flightMode) {
                    ap.StartAction("AIM1", -1);
                }
            }

            if (aimMode == 1 || aimMode == 2) {
                // エイム & 攻撃(敵の速度と距離を考慮して目標座標を補正)
                Vector3 ev = ap.GetEnemyVelocity() - ap.GetVelocity();
                float ed = enemyDistance * 0.003f;
                Vector3 mv = ap.MulVec(ev, ed);

                //機銃オートエイム
                Vector3 estPos = ap.AddVec(ap.GetEnemyPosition(), mv);
                ap.Aim(estPos);
            }
        } else {
            aimMode = 0;
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                autoPilot = true;
            }
        }

        if (friendFlg) {
            //味方との連携
            if (!ap.CheckFriend()) {
                ap.SearchFriend("");
            }

            if (!ap.CheckEnemy() && ap.CheckFriend()) {
                searchAction = "SearchFriend";
                //味方と合流
                int ang = ap.GetFriendAngleR();
                if (ang > 15) {
                    ap.StartAction("d", 1);
                } else if (ang < -15) {
                    ap.StartAction("a", 1);
                }
                friendCount++;
                if (Input.GetKeyDown(KeyCode.Alpha5) || friendCount > 199) {
                    friendFlg = false;
                    friendCount = 0;
                    ap.ForgetFriend();
                    searchAction = "Standby";
                }
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                friendFlg = true;
                searchAction = "SearchFriend";
            }
        }

        //AGD制御
        if (flightMode && (ap.GetGroundClearance() > 10 || Input.GetKey(KeyCode.Alpha6))) {
            ap.StartAction("AGD", 1);
        }

        //情報表示
        ap.Print(0, "Enemy : " + ap.GetEnemyName());
        ap.Print(1, "Distance : " + enemyDistance);
        ap.Print(2, "AngleR : " + ap.GetEnemyAngleR());
        ap.Print(3, "AimMode : " + aimMode);
        ap.Print(4, "EnemyHeight : " + ap.GetEnemyPosition().y);
        ap.Print(5, "SelfHeight : " + ap.GetPosition().y);
        ap.Print(6, "NewHeight : " + newHeight);
        ap.Print(7, "SearchAction : " + searchAction);
        ap.Print(8, "UpDownVel : " + ap.GetVelocity().y);
    }
}
