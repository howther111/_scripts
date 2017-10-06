// 防術機トバルカイン用スクリプト
// Armoriser3.csを参考にさせていただきました。さくさく様に感謝
// 作成者:江ノ宮（howther111）
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Zansetsu2 : UserScript {
    // 攻撃検出用マスク
    const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    int jetPower;
    bool autoAim;
    bool bit;
    int scope;

    //アサイン関係
    KeyCode Wep1 = KeyCode.Mouse0; //マシンガン射撃
    KeyCode JetUp = KeyCode.Alpha2; //エンジン出力増加
    KeyCode JetDown = KeyCode.Alpha3; //エンジン出力減少
    KeyCode JetToggle = KeyCode.Space; //エンジントグル
    KeyCode AimToggle = KeyCode.LeftShift; //オートエイムオンオフ
    KeyCode AimReset = KeyCode.Alpha4; //オートエイムリセット
    KeyCode BitToggle = KeyCode.Alpha5; //子機トグル
    KeyCode ScopeToggle = KeyCode.Alpha6; //カメラトグル


    //----------------------------------------------------------------------------------------------
    // ユーザー名取得
    //----------------------------------------------------------------------------------------------
    public override string GetUserName() {
        using (StreamReader sr = new StreamReader(Application.dataPath + "/../UserData/User.mcsd"))
            return LitJson.JsonMapper.ToObject(sr.ReadToEnd())["userName"].ToString();
    }

    //----------------------------------------------------------------------------------------------
    // 開始処理
    //----------------------------------------------------------------------------------------------
    public override void OnStart(AutoPilot ap) {
        jetPower = 0;
        autoAim = false;
        bit = false;
        scope = 0;
    }

    //----------------------------------------------------------------------------------------------
    // 更新処理
    //----------------------------------------------------------------------------------------------
    public override void OnUpdate(AutoPilot ap) {
        // 攻撃
        int energy = ap.GetEnergy();

        //エンジン
        if (jetPower > 0) {
            if (Input.GetKeyDown(JetDown)) {
                jetPower--;
            }
        }
        if (jetPower < 4) {
            if (Input.GetKeyDown(JetUp)) {
                jetPower++;
            }
        }

        if (jetPower == 0 && Input.GetKeyDown(JetToggle)) {
            jetPower = 3;
        } else if (jetPower == 1) {
            ap.StartAction("Jet1", 1);
            if (Input.GetKeyDown(JetToggle)) {
                jetPower = 3;
            }
        } else if (jetPower == 2) {
            ap.StartAction("Jet2", 1);
            if (Input.GetKeyDown(JetToggle)) {
                jetPower = 3;
            }
        } else if (jetPower == 3) {
            ap.StartAction("Jet3", 1);
            if (Input.GetKeyDown(JetToggle)) {
                jetPower = 0;
            }
        } else if (jetPower == 4) {
            ap.StartAction("Boost", 1);
            jetPower--;
        }

        // 子機起動
        if (bit && Input.GetKeyDown(BitToggle)) {
            bit = false;
            autoAim = false;
            scope = 0;
        } else if (!bit && Input.GetKeyDown(BitToggle)) {
            bit = true;
            autoAim = true;
            scope = 2;
        } else if (autoAim && Input.GetKeyDown(AimToggle)) {
            autoAim = false;
        } else if (!autoAim && Input.GetKeyDown(AimToggle)) {
            autoAim = true;
        } else if (scope == 0 && Input.GetKeyDown(ScopeToggle)) {
            scope = 1;
        } else if (scope == 1 && Input.GetKeyDown(ScopeToggle)) {
            scope = 2;
        } else if (scope == 2 && Input.GetKeyDown(ScopeToggle)) {
            scope = 0;
        }

        if (bit) {
            ap.StartAction("Split", 1);
        }

        //ターゲットリセット
        if (Input.GetKeyDown(AimReset)) {
            ap.ForgetEnemy();
        }

        //索敵(前方優先で近い敵を選択)
        if (ap.CheckEnemy() == false && autoAim) {
            ap.SearchEnemy();
        }

        if (ap.CheckEnemy() && autoAim) {
            int enemyDistance = ap.GetEnemyDistance();
            // エイム(敵の速度と距離を考慮して目標座標を補正)
            Vector3 ev = ap.GetEnemyVelocity() - ap.GetVelocity();
            float ed = enemyDistance * 0.003f;
            Vector3 mv = ap.MulVec(ap.GetEnemyVelocity(), ed);
            Vector3 estPos = ap.AddVec(ap.GetEnemyPosition(), mv);
            ap.Aim(estPos);

            bit = true;

            //情報表示
            ap.Print(3, "Enemy : " + ap.GetEnemyName());
            ap.Print(4, "Distance : " + enemyDistance);
            ap.Print(5, "AngleR : " + ap.GetEnemyAngleR());
            ap.Print(6, "AngleU : " + ap.GetEnemyAngleU());
        } else if (!ap.CheckEnemy() && autoAim) {
            bit = false;

            //情報表示
            ap.Print(5, "Enemy : None");
            ap.Print(6, "Distance : None");
            ap.Print(7, "AngleR : None");
            ap.Print(8, "AngleU : None");
        } else {
            //情報表示
            ap.Print(5, "Enemy : None");
            ap.Print(6, "Distance : None");
            ap.Print(7, "AngleR : None");
            ap.Print(8, "AngleU : None");
        }

        //カメラ
        if (scope == 1) {
            ap.StartAction("Camera1", 1);
        } else if (scope == 2) {
            ap.StartAction("Camera2", 1);
        }

        //情報表示
        ap.Print(0, "EnginePower : " + jetPower);
        ap.Print(1, "EnemyExist : " + ap.CheckEnemy());
        ap.Print(2, "AimMode : " + autoAim);
        ap.Print(3, "CameraMode : " + scope);
        ap.Print(4, "BitMode : " + bit);

        //銃
        if (energy > 10 && Input.GetKey(Wep1) && !autoAim) {
            ap.StartAction("Cannon", 1);
        } else if (energy > 10 && Input.GetKey(Wep1) && autoAim && ap.CheckEnemy()
            && (ap.GetEnemyAngleR() < 90 && ap.GetEnemyAngleR() > -90) && (ap.GetEnemyAngleU() < 45 && ap.GetEnemyAngleU() > -90)) {
            ap.StartAction("Cannon", 1);
        }
    }
}
