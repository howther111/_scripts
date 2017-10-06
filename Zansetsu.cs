// 防術機トバルカイン用スクリプト
// Armoriser3.csを参考にさせていただきました。さくさく様に感謝
// 作成者:江ノ宮（howther111）
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Zansetsu : UserScript {
    // 攻撃検出用マスク
    const int MASK_BULLET = 1;  /// 通常弾(Cannon)
	const int MASK_SHELL = 2;   /// 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; /// 榴弾(Cannon)
	const int MASK_BLADE = 8;   /// 刃(Sword)
	const int MASK_PLASMA = 16; /// プラズマ(Launcher)
	const int MASK_LASER = 32;  /// レーザー(Beamer)
	const int MASK_ALL = 0xff;
    int jetPower;

    //アサイン関係
    KeyCode Wep1 = KeyCode.Mouse0; //マシンガン射撃
    KeyCode JetUp = KeyCode.Alpha2; //エンジン出力増加
    KeyCode JetDown = KeyCode.Alpha3; //エンジン出力減少
    KeyCode JetToggle = KeyCode.Space; //エンジントグル

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
    }

    //----------------------------------------------------------------------------------------------
    // 更新処理
    //----------------------------------------------------------------------------------------------
    public override void OnUpdate(AutoPilot ap) {
        // 攻撃
        int energy = ap.GetEnergy();

        //銃
        if (energy > 10 && Input.GetKey(Wep1)) {
            ap.StartAction("Cannon", -1);
        } else {
            ap.EndAction("Cannon");
        }

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
    }
}
