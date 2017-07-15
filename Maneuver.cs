// 機動サンプル

using UnityEngine;

public class Maneuver : UserScript
{
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
	}

	//----------------------------------------------------------------------------------------------
	// 更新処理
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		// クリックで開始
		if(Input.GetKeyDown(KeyCode.Mouse0)) count = -400;	// 反時計回り
		if(Input.GetKeyDown(KeyCode.Mouse1)) count = 400;	// 時計回り

		// カウントに応じて動作
		switch(count)
		{
			case 400:
			case -400:
				ap.SetMoverAltitude(50);
				ap.StartAction("HoverF", 100);
				break;
			case 300:
			case -100:
				ap.StartAction("HoverR", 100);
				break;
			case 200:
			case -200:
				ap.StartAction("HoverB", 100);
				break;
			case 100:
			case -300:
				ap.StartAction("HoverL", 100);
				break;
			case 1:
			case -1:
				ap.SetMoverAltitude(0);
				break;
		}

		/// カウントダウンorアップ
		if(count > 0) --count;
		if(count < 0) ++count;
		ap.Print(0, "count=" + count);
	}
}
