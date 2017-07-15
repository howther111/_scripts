// BritzVogelEng

using UnityEngine;

public class BlitzVogelEng : UserScript
{
	// 攻撃検出用マスク
	const int MASK_BULLET = 1;  /// (Cannon1)
	const int MASK_SHELL = 2;   /// (Cannon2)
	const int MASK_GRENADE = 4; /// (Cannon3)
	const int MASK_BLADE = 8;   /// (Sword)
	const int MASK_PLASMA = 16; /// (Launcher)
	const int MASK_LASER = 32;  /// (Beamer)
	const int MASK_ALL = 0xff;
    bool sword = false;
    bool flightMode = false;
    bool autoPilot = false;
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

    //Alpha1:Aim
    //Alpha2:Change
    //Alpha3:AutoAim
    //Alpha4:Reset
    //Alpha5:SearchFrieng
    //Alpha6:JumpFromUnderWater

    //----------------------------------------------------------------------------------------------
    // UserName
    //----------------------------------------------------------------------------------------------
    public override string GetUserName()
	{
		return "howther111";
	}

	//----------------------------------------------------------------------------------------------
	// Start
	//----------------------------------------------------------------------------------------------
	public override void OnStart(AutoPilot ap)
	{

	}

	//----------------------------------------------------------------------------------------------
	// Update
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		// Attack
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

        //Move
        if (!flightMode) {
            //Human Mode
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
                ap.StartAction("up", 1);
            }
            if (Input.GetKey(KeyCode.F)) {
                ap.StartAction("down", 1);
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
            //Fighter Mode
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

        //AutoAim
        float enemyDistance = ap.GetEnemyDistance();
        float enemyAngleR = ap.GetEnemyAngleR();
        float enemyAngleU = ap.GetEnemyAngleU();
        Vector3 enemyVelocity = ap.GetEnemyVelocity() - ap.GetVelocity();
        enemyHeight = ap.GetEnemyPosition().y;
        selfHeight = ap.GetPosition().y;
        //AutoPilot
        if (autoPilot) {
            if (!ap.CheckEnemy()) {
                ap.SearchEnemy();
            }

            aim = true;

            //TargetReset
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
                // Aim & Attack
                Vector3 ev = ap.GetEnemyVelocity() - ap.GetVelocity();
                float ed = enemyDistance * 0.003f;
                Vector3 mv = ap.MulVec(ev, ed);

                //AutoAim
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
            if (!ap.CheckFriend()) {
                ap.SearchFriend("");
            }

            if (!ap.CheckEnemy() && ap.CheckFriend()) {
                searchAction = "SearchFriend";
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

        //AGD
        if (flightMode && (ap.GetGroundClearance() > 10 || Input.GetKey(KeyCode.Alpha6))) {
            ap.StartAction("AGD", 1);
        }

        //Display Information
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
