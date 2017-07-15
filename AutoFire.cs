// 汎用自動射撃スクリプト

// 画面の中心付近の敵を手動(任意のキー入力)で捕捉開始する
// "KeyCode.Mouse1"を"KeyCode.A"に書き換えれば、トリガーを右クリックからAキーに変更できる

using UnityEngine;

public class Graphics : UserScript
{
	//----------------------------------------------------------------------------------------------
	// ユーザー名取得
	//----------------------------------------------------------------------------------------------
	public override string GetUserName()
	{
		return "Author";
	}

	//----------------------------------------------------------------------------------------------
	// 開始処理
	//----------------------------------------------------------------------------------------------
	public override void OnStart(AutoPilot ap)
	{
		// 無駄な射撃の抑制
		// エイム方向と砲身の内角が15度以内になるまでCannon&Beamerを待機状態にする
		ap.SetAimTolerance(15);
	}

	//----------------------------------------------------------------------------------------------
	// 更新処理
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		// 敵選択(0.5秒以上押し続けると解除)
		ap.SelectEnemy(KeyCode.Mouse1);

		// エナジーが10%未満になったら捕捉を中止する
		if(ap.GetEnergy() < 10) ap.ForgetEnemy();

		// 情報取得&表示
		float dist = ap.GetEnemyDistance();
		ap.Print(0, "Enemy : " + ap.GetEnemyName());
		ap.Print(1, "Distance : " + dist);

		// 選択中の敵が600m以内ならエイミング
		if(dist < 600f)
		{
			Vector3 relVel = ap.GetEnemyVelocity() - ap.GetVelocity() * 0.5f;	// 相対速度
			Vector3 estPos = ap.GetEnemyPosition() + relVel * dist * 0.006f;	// 着弾予測座標
			ap.Aim(estPos);

			// 選択中の敵が500m以内なら射撃アクション実行
			if(dist < 500f)
			{
				ap.StartAction("CannonA", 1);
			}
		}
	}
}
