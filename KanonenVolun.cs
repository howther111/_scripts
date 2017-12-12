// 防術機カノーネンヴォルン用スクリプト
// Armoriser3.csを参考にさせていただきました。さくさく様に感謝
// 作成者:江ノ宮（howther111）

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class KanonenVolun : UserScript {
    // 攻撃検出用マスク
    const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    const int FIRE_COUNT_MAX = 40;
    bool missile;
    bool fixFlg;
    int missileMode;
    int fireCount;
    int camCount;

    //アサイン関係
    KeyCode Wep1 = KeyCode.Mouse0; //主砲射撃
    KeyCode BodyFix = KeyCode.Mouse2; //機体固定
    KeyCode Wep2 = KeyCode.Mouse1; //ミサイルロック・発射
    KeyCode MissileChange = KeyCode.LeftControl; //ミサイル射撃モード切替
    KeyCode Jump = KeyCode.Space; //跳躍
    KeyCode CamChange = KeyCode.LeftShift; //カメラ切り替え
    KeyCode backMove = KeyCode.S; //後退

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
        missile = false;
        fixFlg = false;
        camCount = 0;
        missileMode = 1;
        fireCount = FIRE_COUNT_MAX;
    }

    //----------------------------------------------------------------------------------------------
    // 更新処理
    //----------------------------------------------------------------------------------------------
    public override void OnUpdate(AutoPilot ap) {
        // 攻撃
        int energy = ap.GetEnergy();

        if ((Input.GetKeyDown(BodyFix) && fixFlg) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
            fixFlg = false;
            fireCount = FIRE_COUNT_MAX;
        } else if ((Input.GetKeyDown(BodyFix) && !fixFlg) || Input.GetKeyDown(Wep1)) {
            fixFlg = true;
        }

        if (fixFlg) {
            if (fireCount > 0) {
                fireCount = fireCount - 1;
            }
            ap.StartAction("FIX", 1);
        }

        //銃
        if (energy > 30 && fireCount == 0 && Input.GetKey(Wep1) && fixFlg && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.S)
            && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D)) {
            ap.StartAction("ATK1", 1);
        }

        //ミサイル
        if (!missile && Input.GetKeyDown(Wep2)) {
            ap.StartAction("ATK2", -1);
            if (missileMode == 1) {
                ap.StartAction("ATK2-1", -1);
            } else if (missileMode == 2) {
                ap.StartAction("ATK2-2", -1);
            }
            missile = true;
        } else if ((missile && Input.GetKeyDown(Wep2)) || energy < 10) {
            ap.EndAction("ATK2");
            ap.EndAction("ATK2-1");
            ap.EndAction("ATK2-2");
            missile = false;
        }

        //ミサイル切り替え
        if (Input.GetKeyDown(MissileChange) && missileMode == 1 && !missile) {
            missileMode = 2;
        } else if (Input.GetKeyDown(MissileChange) && missileMode == 2 && !missile) {
            missileMode = 1;
        }

        //ジャンプ
        if (energy > 65 && Input.GetKey(Jump)) {
            ap.StartAction("Jump", -1);
            fixFlg = false;
            fireCount = FIRE_COUNT_MAX;
        } else if (!Input.GetKey(Jump) || energy < 10) {
            ap.EndAction("Jump");
        }

        if (Input.GetKeyDown(CamChange) && camCount < 2) {
            camCount = camCount + 1;
        } else if (Input.GetKeyDown(CamChange) && camCount >= 2) {
            camCount = 0;
        }

        if (camCount == 1) {
            ap.StartAction("Camera", 1);
        } else if (camCount == 2) {
            ap.StartAction("CamPlus", 1);
        }
    }
}
