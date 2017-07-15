using UnityEngine;
using System.Collections.Generic;

// 通常のクラスの例

public class Util1
{
	List<GameObject> sprites = new List<GameObject>();

	/// スプライト追加
	public void AddSprite(AutoPilot ap)
	{
		sprites.Add(ap.CreateSprite("sample"));	// スプライトを作成してリストに追加
	}

	/// スプライト削除
	public void RemoveSprite(AutoPilot ap)
	{
		if(sprites.Count == 0) return;	// スプライトが無い場合はキャンセル
		GameObject.Destroy(sprites[0]);	// 一番古いスプライトを破棄
		sprites.RemoveAt(0);	// 破棄したスプライトをリストから削除
	}

	/// スプライト移動
	public void MoveSprites(AutoPilot ap)
	{
		// スプライトが無い場合は情報表示をクリアして終了
		if(sprites.Count == 0)
		{
			ap.Print(1);
			ap.Print(2);
			return;
		}

		float time = Time.realtimeSinceStartup;	// 経過時間(秒)

		for(int i=0; i<sprites.Count; ++i)
		{
			var spriteRT = sprites[i].GetComponent<RectTransform>();	// スプライトのUIトランスフォーム取得
			Vector2 spritePos;
			float modTime = time + i;	// 重ならないようにずらす
			spritePos.x = Mathf.Cos(modTime) * 640;	// 論理画面幅(1280)の半分
			spritePos.y = Mathf.Sin(modTime) * 360;	// 論理画面高さ(720)の半分
			spriteRT.anchoredPosition = spritePos;
			float spriteRot = Mathf.Repeat(time * 100, 360f);	// 回転角(度)
			spriteRT.localEulerAngles = Vector3.forward * spriteRot;	// Z軸まわりに回転

			/// 最初のスプライトに関する情報を表示
			if(i == 0)
			{
				ap.Print(1, "pos=" + spritePos);
				ap.Print(2, "rot=" + spriteRot);
			}
		}
	}

	// スプライト数取得
	public int GetSpriteCount()
	{
		return sprites.Count;
	}
}
