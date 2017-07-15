// AI_Trainer(PRACTICEの敵マシン)用スクリプト
// 攻撃されると反撃する
// 4種の武器を使い分ける

// LuaからC#に移行する際の参考用にtrainer.txt(oldフォルダにあるLuaスクリプト)をC#で書き直したもの

using UnityEngine;

public class Trainer : UserScript
{
	// 攻撃検出用マスク
	const int MASK_BULLET = 1;  // 通常弾(Cannon)
	const int MASK_SHELL = 2;   // 戦車砲弾(Cannon)
	const int MASK_GRENADE = 4; // 榴弾(Cannon)
	const int MASK_BLADE = 8;   // 刃(Sword)
	const int MASK_PLASMA = 16; // プラズマ(Discharger)
	const int MASK_LASER = 32;  // レーザー(Beamer)
	const int MASK_MISSILE = 64;// ミサイル(Launcher)
	const int MASK_ALL = 0xff;

	Vector3 startPosition;
	string attackAction = "Beamer";
	int chargeCount;
	int suicideWait;
	int wait;
	int count;

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
		ap.SetAimTolerance(15);	// エイム許容誤差設定
		ap.SetAutoLockon(true);	// ランチャーの自動ロックオン開始
		startPosition = ap.GetPosition();	// 開始座標保存
	}

	//----------------------------------------------------------------------------------------------
	// 更新処理
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		// 待機
		if(wait > 0)
		{
			--wait;
			ap.Log("wait=" + wait);
			return;
		}

		// 自爆演出(右回転+上昇→自爆)
		if(suicideWait > 0)
		{
			ap.TurnMover(ap.GetPosition() + ap.GetRight());
			ap.SetMoverAltitude(100);
			if(--suicideWait == 0) ap.Suicide();
			return;
		}

		// 索敵(ヘルスが満タンでなかったら前方優先で近い敵を選択,攻撃を検出したら捕捉対象を変更)
		if(!ap.CheckEnemy() && ap.GetHealth() < 100) ap.SearchEnemy();
		if(count % 600 == 0) ap.SetCounterSearch(500);

		float enemyDistance = ap.GetEnemyDistance();
		float enemyAngleR = ap.GetEnemyAngleR();
		float enemyAngleU = ap.GetEnemyAngleU();
		Vector3 enemyVelocity = ap.GetEnemyVelocity();

		// 情報表示
		ap.Print(0, "Enemy : " + ap.GetEnemyName());
		ap.Print(1, "Distance : " + enemyDistance);
		ap.Print(2, "AngleR : " + enemyAngleR);
		ap.Print(3, "AngleU : " + enemyAngleU);
		ap.Print(4, "Speed(m/s) : " + Mathf.RoundToInt(Vector3.Magnitude(enemyVelocity)));
		if(Time.frameCount % 60 == 0)
		{
			ap.Print(8, "Random : " + Random.Range(0,99));
		}

		if(ap.CheckEnemy())
		{
			var relVel = ap.GetEnemyVelocity() - ap.GetVelocity();	// 相対速度
			var estPos = ap.GetEnemyPosition() + relVel * 0.2f;	// 着弾予測座標
			ap.TurnMover(estPos);	// 旋回(Mover専用)
			ap.Aim(estPos);	// エイム
		}

		// 武器選択&通知
		var prevAttackAction = attackAction;
		int attackAngle = 10;
		int attackEnergy = 50;
		if(enemyDistance < 20)
		{
			attackAction = "Sword";
			attackAngle = 60;
			attackEnergy = 20;
		}
		else if(chargeCount > 0)
		{
			attackAction = "Beamer";
		}
		else if(enemyDistance > 40 && enemyDistance < 250)
		{
			attackAction = "Cannon";
		}
		else if(enemyDistance > 300)
		{
			attackAction = "Beamer";
		}

		// 武器破損チェック(代替武器に変更,代替武器も破損していたら自爆開始)
		if(ap.GetSurvivalRate(attackAction) < 50)
		{
			attackAction = attackAction == "Cannon" ? "Beamer" : "Cannon";
		}
		if(ap.GetSurvivalRate(attackAction) < 50) suicideWait = 180;

		// 武器変更通知(ログ出力)
		bool isWeaponChanged = attackAction != prevAttackAction;
		if(isWeaponChanged) ap.Log(attackAction + " is selected.");

		// 攻撃中止
		int energy = ap.GetEnergy();
		if(Mathf.Abs(enemyAngleR) > attackAngle*2 || energy < 10 || isWeaponChanged || !ap.CheckEnemy())
		{
			ap.EndAction("Sword");
			ap.EndAction("Cannon");
			ap.EndAction("Beamer");
		}

		// 攻撃開始(ダメージを受けた後ENに余裕があったら選択中の武器で攻撃,下方に敵がいたらグレネード)
		if(ap.GetHealth() < 100 && ap.CheckEnemy())
		{
			if(enemyAngleU < -70)
			{
				ap.StartAction("Grenade", 1);
			}
			else if(Mathf.Abs(enemyAngleR) < attackAngle && energy > attackEnergy)
			{
				ap.StartAction(attackAction, -1);
			}
		}

		// ランチャーチャージ開始(ダメージを受けた後ENに余裕があったら実行)
		if(chargeCount == 0 && energy > 60 && ap.GetHealth() < 100 && ap.CheckEnemy())
		{
			ap.StartAction("Launcher", -1);
		}

		// ランチャー発射(一定時間チャージした後ロックオン中またはEN切れなら実行)
		if(ap.CheckAction("Launcher"))
		{
			++chargeCount;
			if(chargeCount > 200)
			{
				if(ap.CheckLockOn() || energy < 5 || chargeCount > 800)
				{
					ap.EndAction("Launcher");
					chargeCount = 0;
				}
			}
		}

		// 水平移動(脅威を回避してターゲットとの距離を保つ,至近距離なら近づく,暇なら開始座標に戻る)
		ap.SearchThreat(MASK_ALL, 100);
		if(ap.CheckThreat())
		{
			ap.StartAction(ap.GetThreatAngleR() < 0 ? "HoverR" : "HoverL", 10);
		}
		else if(ap.CheckEnemy())
		{
			if(energy < 30)
			{
				ap.StartAction("HoverB", 10);
			}
			else if(Mathf.Abs(enemyAngleR) < 45)
			{
				if(enemyDistance > 10 && enemyDistance < 50)
				{
					ap.StartAction("HoverF", 10);
				}
				else if(enemyDistance < 100)
				{
					ap.StartAction("HoverB", 10);
				}
				else if(enemyDistance > 500)
				{
					ap.StartAction("HoverF", 10);
				}
			}
		}
		else if(Vector3.Distance(ap.GetPosition(), startPosition) > 50)
		{
			ap.TurnMover(startPosition);
			ap.SetMoverAltitude(10);
			ap.StartAction("HoverF", 10);
		}

		// 垂直移動(敵を少し見上げる)
		if(chargeCount > 0)
		{
			ap.SetMoverAltitude(100);
		}
		else if(ap.CheckEnemy())
		{
			if(enemyAngleU < 5)
			{
				ap.StartAction("HoverD", 10);
			}
			else if(enemyAngleU > 15)
			{
				ap.StartAction("HoverU", 10);
			}
		}

		// 転覆から復帰
		if(ap.GetTilt() > 60) ap.Recover();

		++count;
	}
}
