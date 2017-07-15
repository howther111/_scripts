using UnityEngine;

// �X�^�e�B�N�N���X�̗�(�N���X�錾�̑O��"static"���t���Ă���)

public static class Util2
{
	public static void DrawLines(AutoPilot ap)
	{
		var pos1 = ap.GetPosition();
		for(int i=0; i<5000; ++i)
		{
			var pos2 = new Vector3(Random.Range(-100f,100f), 10f, Random.Range(-100f,100f));
			var col = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
			ap.DrawLine3D(col, pos1, pos2);
		}
	}
}
