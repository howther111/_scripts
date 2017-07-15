// 描画サンプル

// 普通のクラス(Util1)は自分で必要な数だけ生成します
// スタティッククラス(Util2)は起動時に自動で1つ生成され、2つにはできません

using UnityEngine;

public class Graphics : UserScript
{
	Util1 util;

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
		util = new Util1();
	}

	//----------------------------------------------------------------------------------------------
	// 更新処理
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		/// 左クリックでスプライト追加
		if(Input.GetKeyDown(KeyCode.Mouse0)) util.AddSprite(ap);

		/// 右クリックでスプライト削除
		if(Input.GetKeyDown(KeyCode.Mouse1)) util.RemoveSprite(ap);

		// 全スプライト移動
		util.MoveSprites(ap);

		// スプライトが無い場合のみライン描画
		int spriteCount = util.GetSpriteCount();
		if(spriteCount == 0) Util2.DrawLines(ap);
		ap.Print(0, "SpriteCount=" + spriteCount);
	}
}
