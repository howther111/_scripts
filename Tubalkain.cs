// 防術機トバルカイン用スクリプト
// Armoriser3.csを参考にさせていただきました。さくさく様に感謝
// 作成者:江ノ宮（howther111）
//@auther Sakusakumura[JP]
//@Version 4.4.5
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Tubalkain : UserScript {
    // 攻撃検出用マスク
    const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    bool missile;
    bool sword;
    bool spin;
    bool shieldFlg;
    int missileMode;

    //アサイン関係
    KeyCode Wep1 = KeyCode.Mouse0; //マシンガン射撃
    KeyCode SwordOn = KeyCode.R; //ビームサイスON・OFF
    KeyCode SwordSlash = KeyCode.Q; //ビームサイス斬撃
    KeyCode SwordSpin = KeyCode.E; //ビームサイス回転
    KeyCode Wep2 = KeyCode.Mouse1; //ミサイルロック・発射
    KeyCode MissileChange = KeyCode.LeftControl; //ミサイル射撃モード切替
    KeyCode Jump = KeyCode.Space; //跳躍
    KeyCode Shield = KeyCode.Mouse2; //シールドオンオフ

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
        sword = false;
        spin = false;
        missileMode = 1;
    }

    //----------------------------------------------------------------------------------------------
    // 更新処理
    //----------------------------------------------------------------------------------------------
    public override void OnUpdate(AutoPilot ap) {
        // 攻撃
        int energy = ap.GetEnergy();

        //銃
        if (energy > 10 && Input.GetKey(Wep1)) {
            ap.StartAction("ATK1", -1);
        } else {
            ap.EndAction("ATK1");
        }

        //ソード
        if (energy > 30 && (Input.GetKeyDown(SwordOn) || Input.GetKeyDown(SwordSlash)) && !sword) {
            sword = true;
        } else if (Input.GetKeyDown(SwordOn) && sword) {
            sword = false;
        }
        if (sword && energy > 10) {
            ap.StartAction("ATK3BF", 1);
        }

        //連続攻撃
        if (energy > 30 && !spin && Input.GetKeyDown(SwordSpin)) {
            ap.StartAction("ATK3", -1);
            sword = false;
            spin = true;
        } else if (spin && (energy < 10 || sword || Input.GetKeyDown(SwordSpin))) {
            ap.EndAction("ATK3");
            spin = false;
            sword = false;
        }
        if (spin && energy > 10) {
            ap.StartAction("ATK3BF", 1);
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
        if (energy > 40 && Input.GetKey(Jump)) {
            ap.StartAction("Jump", -1);
        } else if (!Input.GetKey(Jump) || energy < 10) {
            ap.EndAction("Jump");
        }

        //シールド
        if (!shieldFlg && Input.GetKeyDown(Shield)) {
            shieldFlg = true;
        } else if (shieldFlg && Input.GetKeyDown(Shield)) {
            shieldFlg = false;
        }

        if (shieldFlg && energy > 40) {
            ap.StartAction("shield", 1);
        }
    }
}
