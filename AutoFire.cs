// �ėp�����ˌ��X�N���v�g

// ��ʂ̒��S�t�߂̓G���蓮(�C�ӂ̃L�[����)�ŕߑ��J�n����
// "KeyCode.Mouse1"��"KeyCode.A"�ɏ���������΁A�g���K�[���E�N���b�N����A�L�[�ɕύX�ł���

using UnityEngine;

public class Graphics : UserScript
{
	//----------------------------------------------------------------------------------------------
	// ���[�U�[���擾
	//----------------------------------------------------------------------------------------------
	public override string GetUserName()
	{
		return "Author";
	}

	//----------------------------------------------------------------------------------------------
	// �J�n����
	//----------------------------------------------------------------------------------------------
	public override void OnStart(AutoPilot ap)
	{
		// ���ʂȎˌ��̗}��
		// �G�C�������ƖC�g�̓��p��15�x�ȓ��ɂȂ�܂�Cannon&Beamer��ҋ@��Ԃɂ���
		ap.SetAimTolerance(15);
	}

	//----------------------------------------------------------------------------------------------
	// �X�V����
	//----------------------------------------------------------------------------------------------
	public override void OnUpdate(AutoPilot ap)
	{
		// �G�I��(0.5�b�ȏ㉟��������Ɖ���)
		ap.SelectEnemy(KeyCode.Mouse1);

		// �G�i�W�[��10%�����ɂȂ�����ߑ��𒆎~����
		if(ap.GetEnergy() < 10) ap.ForgetEnemy();

		// ���擾&�\��
		float dist = ap.GetEnemyDistance();
		ap.Print(0, "Enemy : " + ap.GetEnemyName());
		ap.Print(1, "Distance : " + dist);

		// �I�𒆂̓G��600m�ȓ��Ȃ�G�C�~���O
		if(dist < 600f)
		{
			Vector3 relVel = ap.GetEnemyVelocity() - ap.GetVelocity() * 0.5f;	// ���Α��x
			Vector3 estPos = ap.GetEnemyPosition() + relVel * dist * 0.006f;	// ���e�\�����W
			ap.Aim(estPos);

			// �I�𒆂̓G��500m�ȓ��Ȃ�ˌ��A�N�V�������s
			if(dist < 500f)
			{
				ap.StartAction("CannonA", 1);
			}
		}
	}
}
