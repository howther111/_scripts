// 防術機アズラエル用スクリプト
// Armoriser3.csを参考にさせていただきました。さくさく様に感謝
// 作成者:江ノ宮（howther111）

using UnityEngine;

public class Azrael : UserScript {
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
    bool spin = false;
    int missileMode = 1;

    //アサイン関係
    KeyCode Wep1 = KeyCode.Mouse0; //マシンガン射撃
    KeyCode SwordOn = KeyCode.R; //ビームサイスON・OFF
    KeyCode SwordSlash = KeyCode.Q; //ビームサイス斬撃
    KeyCode SwordSpin = KeyCode.E; //ビームサイス回転
    KeyCode Wep2 = KeyCode.Mouse2;
    KeyCode Missile = KeyCode.Keypad6;
    KeyCode ChangeOptKey = KeyCode.Keypad5;
    KeyCode LauncherKey = KeyCode.Keypad4;
    KeyCode AimAssist = KeyCode.Alpha2;
    KeyCode AimKey = KeyCode.T;
    KeyCode BoostKey = KeyCode.F;
    KeyCode MoveF = KeyCode.W;
    KeyCode MoveB = KeyCode.S;
    KeyCode MoveU = KeyCode.Space;
    KeyCode MoveD = KeyCode.LeftShift;
    KeyCode MoveL = KeyCode.A;
    KeyCode MoveR = KeyCode.D;

    //----------------------------------------------------------------------------------------------
    // ユーザー名取得
    //----------------------------------------------------------------------------------------------
    public override string GetUserName() {
        return "howther111";
    }

    //----------------------------------------------------------------------------------------------
    // 開始処理
    //----------------------------------------------------------------------------------------------
    public override void OnStart(AutoPilot ap) {

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
        if (sword && energy > 10 && !spin) {
            ap.StartAction("ATK3BF", 1);
        }

        if (energy > 30 && !spin && Input.GetKeyDown(SwordSlash)) {
            ap.StartAction("spin", -1);
            sword = false;
            spin = true;
        } else if (spin && (energy < 10 || sword || Input.GetKeyDown(SwordSlash))) {
            ap.EndAction("spin");
            spin = false;
        }

        //ミサイル
        if (!missile && energy > 15 && Input.GetKeyDown(Wep2)) {
            ap.StartAction("ATK2", -1);
            missile = true;
        } else if ((missile && Input.GetKeyDown(Wep2)) || energy < 10) {
            ap.EndAction("ATK2");
            missile = false;
        }
    }
}
