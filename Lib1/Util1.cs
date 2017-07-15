using UnityEngine;
using System.Collections.Generic;

// �ʏ�̃N���X�̗�

public class Util1
{
	List<GameObject> sprites = new List<GameObject>();

	/// �X�v���C�g�ǉ�
	public void AddSprite(AutoPilot ap)
	{
		sprites.Add(ap.CreateSprite("sample"));	// �X�v���C�g���쐬���ă��X�g�ɒǉ�
	}

	/// �X�v���C�g�폜
	public void RemoveSprite(AutoPilot ap)
	{
		if(sprites.Count == 0) return;	// �X�v���C�g�������ꍇ�̓L�����Z��
		GameObject.Destroy(sprites[0]);	// ��ԌÂ��X�v���C�g��j��
		sprites.RemoveAt(0);	// �j�������X�v���C�g�����X�g����폜
	}

	/// �X�v���C�g�ړ�
	public void MoveSprites(AutoPilot ap)
	{
		// �X�v���C�g�������ꍇ�͏��\�����N���A���ďI��
		if(sprites.Count == 0)
		{
			ap.Print(1);
			ap.Print(2);
			return;
		}

		float time = Time.realtimeSinceStartup;	// �o�ߎ���(�b)

		for(int i=0; i<sprites.Count; ++i)
		{
			var spriteRT = sprites[i].GetComponent<RectTransform>();	// �X�v���C�g��UI�g�����X�t�H�[���擾
			Vector2 spritePos;
			float modTime = time + i;	// �d�Ȃ�Ȃ��悤�ɂ��炷
			spritePos.x = Mathf.Cos(modTime) * 640;	// �_����ʕ�(1280)�̔���
			spritePos.y = Mathf.Sin(modTime) * 360;	// �_����ʍ���(720)�̔���
			spriteRT.anchoredPosition = spritePos;
			float spriteRot = Mathf.Repeat(time * 100, 360f);	// ��]�p(�x)
			spriteRT.localEulerAngles = Vector3.forward * spriteRot;	// Z���܂��ɉ�]

			/// �ŏ��̃X�v���C�g�Ɋւ������\��
			if(i == 0)
			{
				ap.Print(1, "pos=" + spritePos);
				ap.Print(2, "rot=" + spriteRot);
			}
		}
	}

	// �X�v���C�g���擾
	public int GetSpriteCount()
	{
		return sprites.Count;
	}
}
