//@auther Sakusakumura[JP]
//@Version 4.4.5
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class Armoriser3 : UserScript
{
	bool isRotateByMouse;
	bool isAiming;
	bool IsPushedAlpha1;
	bool IsPushedAlpha2;
	bool IsPushedF;
	bool IsPushedKeyPad5;
	bool IsPushedT;
	bool IsPushedKeypad4;
	bool IsboostMode;
	bool RancherMode;
	bool Aiming;
	bool isChengedAiming;
	bool AimingOpt;
	bool isChengedAimingOpt;
	bool IsSGMode;
	bool PushedTab;
	Vector3 targetPosition;
	Ray ray;
	RaycastHit hit;
	float Counter;
	bool ChangeAim;
	//ArmoriserHUD armoriserHUD;
	Vector3 SavedVelocity;

	//アサイン関係
	KeyCode Wep1 = KeyCode.Mouse0;
	KeyCode Wep2 = KeyCode.Mouse1;
	KeyCode Wep3 = KeyCode.Mouse2;
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
	KeyCode MoveL = KeyCode.Q;
	KeyCode MoveR = KeyCode.R;

	FileInfo Base = new FileInfo(Application.dataPath + "/../UserData/_scripts/AssignList.txt");


	public override string GetUserName()
	{
		using (StreamReader sr = new StreamReader(Application.dataPath + "/../UserData/User.mcsd"))
        return LitJson.JsonMapper.ToObject(sr.ReadToEnd())["userName"].ToString();
	}


	public override void OnStart(AutoPilot ap)
	{
		isRotateByMouse = false;
		IsPushedAlpha1 = false;
		isAiming = false;
		IsboostMode = false;
		RancherMode = false;
		Aiming = false;
		AimingOpt = false;
		IsSGMode = false;
		IsPushedF = false;
		IsPushedKeyPad5 = false;
		IsPushedT = false;
		IsPushedKeypad4 = false;
		PushedTab = false;
		Counter = 0;
		ChangeAim = true;
		SavedVelocity = Vector3.zero;
		//armoriserHUD = new ArmoriserHUD();
		if(ap.GetSurvivalRate("CheckWep") != 0){
			ap.StartAction("FixedArm", -1);
		}

		ReadAssignFile(ap);
	}


	public override void OnUpdate(AutoPilot ap)
	{
		/*
		if(Counter == 0f){
			armoriserHUD.StartHUD(ap);
		}else{
			armoriserHUD.StartUpHUD(ap, Counter);
		}
		Counter += 2f;
		if(Counter > 361f){
			armoriserHUD.ContinueRend(ap);
		}
		armoriserHUD.SearchObject(ap);
		*/

		/*
		ap.Print(0,ap.GetFlightTime());
		ap.Print(1,ap.GetVelocity().y);
		ap.Print(2,Mathf.Floor(ap.GetSpeed()) * 3800 / 1000);
		ap.Print(3,Counter);
		*/

		//データロード
		if (Input.GetKey(KeyCode.F7) && Input.GetKey(KeyCode.LeftAlt)){
			if (PushedTab == false){
				ReadAssignFile(ap);
				PushedTab = true;
			}
		}else{
			PushedTab = false;
		}



		if(Input.GetKey(AimAssist)){
			if(IsPushedAlpha2 == false && isAiming == false){
				isAiming = true;
				IsPushedAlpha2 = true;
			}else if(IsPushedAlpha2 == false && isAiming == true){
				isAiming = false;
				IsPushedAlpha2 = true;
			}
		}else{
			IsPushedAlpha2 = false;
		}
		if(Input.GetKey(BoostKey)){
			if(IsPushedF == false){
				if(IsboostMode == false){
					IsboostMode = true;
				}else if(IsboostMode == true){
					IsboostMode = false;
				}
				IsPushedF = true;
			}
		}else{
			IsPushedF = false;
		}
		if(Input.GetKey(ChangeOptKey)){
			if(IsPushedKeyPad5 == false){
				if(RancherMode == false){
					RancherMode = true;
				}else if(RancherMode == true){
					RancherMode = false;
				}
				if(AimingOpt == false){
					AimingOpt = true;
				}else if(AimingOpt == true){
					AimingOpt = false;
				}
				IsPushedKeyPad5 = true;
			}
		}else{
			IsPushedKeyPad5 = false;
		}
		if(Input.GetKey(AimKey)){
			if(IsPushedT == false){
				if(Aiming == false){
					Aiming = true;
				}else if(Aiming == true){
					Aiming = false;
				}
				IsPushedT = true;
			}
		}else{
			IsPushedT = false;
		}
		if(Input.GetKey(LauncherKey)){
			if(IsPushedKeypad4 == false){
				if(IsSGMode == false){
					IsSGMode = true;
				}else if(IsSGMode == true){
					IsSGMode = false;
				}
				IsPushedKeypad4 = true;
			}
		}else{
			IsPushedKeypad4 = false;
		}


		if(AimingOpt == false){
			if(Input.GetKey(Wep1)){
				if(IsSGMode == false){
					ap.StartAction("Main", 1);
				}else if(IsSGMode == true){
					ap.StartAction("SG", 1);
				}
			}
			if(Input.GetKey(Wep2) && Input.GetKey(Wep3)){
				ap.StartAction("Sub2", 1);
			}else{
				if(Input.GetKey(Wep2)){
					ap.StartAction("Sword", 1);
				}
				if(Input.GetKey(Wep3)){
					ap.StartAction("Sub", 1);
				}
			}
		}else{
			if(Input.GetKey(Wep1)){
				if(IsSGMode == false){
					ap.StartAction("OptWepF1", 2);
				}else{
					ap.StartAction("OptWepF5", 2);
				}
			}
			if(Input.GetKey(Wep2) && Input.GetKey(Wep3)){
				ap.StartAction("OptWepF4", 2);
			}else{
				if(Input.GetKey(Wep2)){
					ap.StartAction("OptWepF2", 2);
				}
				if(Input.GetKey(Wep3)){
					ap.StartAction("OptWepF3", 2);
				}
			}
		}


		if(ChangeAim == true){
			if(Aiming == true && AimingOpt == false){
				if(isChengedAiming == false){
					ap.StartAction("Aim", -1);
					ap.EndAction("OptWep");
					isChengedAiming = true;
					isChengedAimingOpt = false;
				}
			}else if(Aiming == true && AimingOpt == true){
				if(isChengedAimingOpt == false){
					ap.StartAction("OptWep", -1);
					ap.EndAction("Aim");
					isChengedAiming = false;
					isChengedAimingOpt = true;
				}
			}else{
				ap.EndAction("OptWep");
				ap.EndAction("Aim");
				isChengedAiming = false;
				isChengedAimingOpt = false;
			}
		}else{
			if(Aiming == true && AimingOpt == false){
				if(isChengedAiming == false){
					ap.StartAction("Aim", -1);
					ap.EndAction("OptWep");
					isChengedAiming = true;
					isChengedAimingOpt = false;
				}
			}else if(Aiming == true && AimingOpt == true){
				if(isChengedAimingOpt == false){
					ap.StartAction("OptWep", -1);
					isChengedAiming = false;
					isChengedAimingOpt = true;
				}
			}else{
				ap.EndAction("OptWep");
				ap.EndAction("Aim");
				isChengedAiming = false;
				isChengedAimingOpt = false;
			}
		}

		if(RancherMode == true){
			if(Input.GetKey(Missile)){
				ap.StartAction("Launcher", 1);
			}
		}else{
			if(Input.GetKey(Missile)){
				ap.StartAction("Missile2", 1);
			}
		}


		if(ap.GetFlightTime() <= 0.5f){

			if(Input.GetKey(MoveF) || Input.GetKey(MoveB) || Input.GetKey(MoveL) || Input.GetKey(KeyCode.E)){
				ap.StartAction("AnyAssig", 1);
			}

			if(Input.GetKey(MoveB) == false){
				if(Input.GetKey(MoveF) == false){
					if(Input.GetKey(MoveL)){
						ap.StartAction("ThrustF", 1);
						ap.StartAction("MoveL", 1);
						ap.StartAction("WalkF", 1);
					}
					if(Input.GetKey(KeyCode.E)){
						ap.StartAction("ThrustF", 1);
						ap.StartAction("MoveR", 1);
						ap.StartAction("WalkF", 1);
					}
				}else{
					if(Input.GetKey(MoveL) || Input.GetKey(KeyCode.E)){
						if(Input.GetKey(MoveL)){
							ap.StartAction("ThrustF", 1);
							ap.StartAction("MoveF", 1);
							ap.StartAction("MoveL", 1);
							ap.StartAction("WalkF", 1);
						}
						if(Input.GetKey(KeyCode.E)){
							ap.StartAction("ThrustF", 1);
							ap.StartAction("MoveF", 1);
							ap.StartAction("MoveR", 1);
							ap.StartAction("WalkF", 1);
						}
					}else{
						ap.StartAction("ThrustF", 1);
						ap.StartAction("MoveF", 1);
						ap.StartAction("WalkF", 1);
					}
				}
			}else{
				if(Input.GetKey(MoveL) || Input.GetKey(KeyCode.E)){
					if(Input.GetKey(MoveL)){
						ap.StartAction("ThrustB", 1);
						ap.StartAction("MoveR", 1);
						ap.StartAction("MoveB", 1);
						ap.StartAction("WalkF", 1);
					}
					if(Input.GetKey(KeyCode.E)){
						ap.StartAction("ThrustB", 1);
						ap.StartAction("MoveL", 1);
						ap.StartAction("MoveB", 1);
						ap.StartAction("WalkF", 1);
					}
				}else{
					ap.StartAction("ThrustB", 1);
					ap.StartAction("MoveB", 1);
					ap.StartAction("WalkF", 1);
				}
			}

			if(IsboostMode == true){
				ap.EndAction("ThrustF");
				ap.EndAction("ThrustB");
				if(Input.GetKey(MoveF)){
					ap.StartAction("SlideF", 20);
					ap.EndAction("MoveF");
				}
				if(Input.GetKey(MoveB)){
					ap.StartAction("SlideB", 1);
					ap.EndAction("MoveB");
				}
				if(Input.GetKey(MoveL)){
					ap.StartAction("SlideF", 20);
					ap.StartAction("MoveL", 1);
				}
				if(Input.GetKey(KeyCode.E)){
					ap.StartAction("SlideF", 20);
					ap.StartAction("MoveR", 1);
				}
				if(Input.GetKey(MoveU)){
					ap.StartAction("SlideU", 1);
					ap.StartAction("SlideF", 20);
					ap.EndAction("MoveU");
				}else if(ap.GetVelocity().y < 0){
					ap.StartAction("SlideU", 1);
					ap.StartAction("SlideF", 20);
					ap.EndAction("MoveU");
				}
				if(Input.GetKey(MoveD)){
					//ap.StartAction("SlideD", 1);
					//ap.EndAction("MoveD");
				}
			}else{
				if(ap.CheckAction("ThrustF")){
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("ThrustF", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("ThrustF", 1);
					}
				}else if(ap.CheckAction("ThrustB")){
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("ThrustB", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("ThrustB", 1);
					}
				}else{
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("ThrustF", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("ThrustF", 1);
					}
				}
			}
		}else{

			if(ap.GetGroundClearance() > 3){
				ap.StartAction("FryMode",1);	
			}

			if(Input.GetKey(MoveF) || Input.GetKey(MoveB) || Input.GetKey(MoveL) || Input.GetKey(KeyCode.E)){
				ap.StartAction("AnyAssig", 1);
			}

			if(Input.GetKey(MoveB) == false){
				if(Input.GetKey(MoveF) == false){
					if(Input.GetKey(MoveL)){
						ap.StartAction("ThrustF", 1);
						ap.StartAction("MoveUL", 1);
					}
					if(Input.GetKey(KeyCode.E)){
						ap.StartAction("ThrustF", 1);
						ap.StartAction("MoveUR", 1);
					}
				}else{
					if(Input.GetKey(MoveL) || Input.GetKey(KeyCode.E)){
						if(Input.GetKey(MoveL)){
							ap.StartAction("ThrustF", 1);
							ap.StartAction("MoveUF", 1);
							ap.StartAction("MoveUL", 1);
						}
						if(Input.GetKey(KeyCode.E)){
							ap.StartAction("ThrustF", 1);
							ap.StartAction("MoveUF", 1);
							ap.StartAction("MoveUR", 1);
						}
					}else{
						ap.StartAction("ThrustF", 1);
						ap.StartAction("MoveUF", 1);
					}
				}
			}else{
				if(Input.GetKey(MoveL) || Input.GetKey(KeyCode.E)){
					if(Input.GetKey(MoveL)){
						ap.StartAction("ThrustB", 1);
						ap.StartAction("MoveULB", 1);
						ap.StartAction("MoveUB", 1);
					}
					if(Input.GetKey(KeyCode.E)){
						ap.StartAction("ThrustB", 1);
						ap.StartAction("MoveURB", 1);
						ap.StartAction("MoveUB", 1);
					}
				}else{
					ap.StartAction("ThrustB", 1);
					ap.StartAction("MoveUB", 1);
				}
			}

			

			if(IsboostMode == true){
				ap.EndAction("ThrustF");
				ap.EndAction("ThrustB");
				if(Input.GetKey(MoveF)){
					ap.StartAction("SlideF", 20);
					ap.EndAction("MoveUF");
				}
				if(Input.GetKey(MoveB)){
					ap.StartAction("SlideB", 1);
					ap.EndAction("MoveB");
				}
				if(Input.GetKey(MoveL)){
					ap.StartAction("SlideF", 20);
					ap.EndAction("MoveL");
				}
				if(Input.GetKey(KeyCode.E)){
					ap.StartAction("SlideF", 20);
					ap.EndAction("MoveR");
				}
				if(ap.CheckAction("SlideF")){
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("SlideF", 20);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("SlideF", 20);
					}
				}else if(ap.CheckAction("SlideB")){
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("SlideB", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("SlideB", 1);
					}
				}else{
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("SlideF", 20);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("SlideF", 20);
					}
				}
			}else{
				if(ap.CheckAction("ThrustF")){
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("ThrustF", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("ThrustF", 1);
					}
				}else if(ap.CheckAction("ThrustB")){
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("ThrustB", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("ThrustB", 1);
					}
				}else{
					if(Input.GetKey(MoveU)){
						ap.StartAction("MoveU", 1);
						ap.StartAction("ThrustF", 1);
					}else if(Input.GetKey(MoveD)){
						ap.StartAction("MoveD", 1);
						ap.StartAction("ThrustF", 1);
					}
				}
			}
		}


		if(isAiming){
			SavedVelocity = new Vector3(Mathf.Lerp(SavedVelocity.x, ap.GetVelocity().normalized.x, 0.5f), Mathf.Lerp(SavedVelocity.y, ap.GetVelocity().normalized.y, 0.5f), Mathf.Lerp(SavedVelocity.z, ap.GetVelocity().normalized.z, 0.5f));
			ray = new Ray (ap.AddVec(ap.GetPosition(),ap.MulVec(ap.GetCameraForward(), 10f)), ap.AddVec(ap.GetCameraForward(), ap.MulVec(SavedVelocity,0.1f)));
			targetPosition = ray.GetPoint(1000f);
			ap.DrawLine3D(Color.yellow, ray.origin, targetPosition);
			if(Physics.Raycast(ray.origin, ray.direction, out hit, 1000f)){
				ap.Print(4, hit.transform.name);
				if(ap.MeasureClearance((int)(ap.AddVec(ap.GetCameraForward(), ap.MulVec(SavedVelocity,0.05f)).x * 1000), (int)(ap.AddVec(ap.GetCameraForward(), ap.MulVec(SavedVelocity,0.05f)).y * 1000), (int)(ap.AddVec(ap.GetCameraForward(), ap.MulVec(SavedVelocity,0.05f)).z * 1000), (int)hit.distance + 50) != 9999){
					ap.DrawLine3D(Color.red,ap.GetPosition(), targetPosition);
				}else{
					ap.DrawLine3D(Color.green,ap.GetPosition(), targetPosition);
				}
			}else{
				ap.DrawLine3D(Color.red,ap.GetPosition(), targetPosition);
			}
		}

		Vector2 MachineForward = new Vector2(ap.GetForward().x, ap.GetForward().z);
		Vector2 CameraForward = new Vector2(ap.GetCameraForward().x, ap.GetCameraForward().z);
		float DotVariable = Vector2.Dot(MachineForward.normalized, CameraForward.normalized);
		int CalcAngleRWithCamera = ap.CalcAngleR(ap.GetCameraPosition());
		ap.Log(DotVariable);
		if(DotVariable < 0.6f){
			if(CalcAngleRWithCamera < 0){
				ap.StartAction("TurnR", 1);
			}else{
				ap.StartAction("TurnL", 1);
			}
		}else if(DotVariable < 0.8f){
			if(CalcAngleRWithCamera < 0){
				ap.StartAction("TurnR2", 1);
			}else{
				ap.StartAction("TurnL2", 1);
			}
		}else if(DotVariable < 0.98f){
			if(CalcAngleRWithCamera < 0){
				ap.StartAction("TurnR3", 1);
			}else{
				ap.StartAction("TurnL3", 1);
			}
		}
		
		/*
		int DotVariable = ap.CalcAngleR(ap.GetCameraPosition());
		ap.Log(DotVariable);
		if(DotVariable > -90 && DotVariable < 90){
			if(ap.CalcAngleR(ap.GetCameraPosition()) < 0){
				ap.StartAction("TurnR", 2);
			}else{
				ap.StartAction("TurnL", 2);
			}
		}else if(DotVariable > -170 && DotVariable < 170){
			if(ap.CalcAngleR(ap.GetCameraPosition()) < 0){
				ap.StartAction("TurnR2", 2);
			}else{
				ap.StartAction("TurnL2", 2);
			}
		}else if(DotVariable > -175 && DotVariable < 175){
			if(ap.CalcAngleR(ap.GetCameraPosition()) < 0){
				ap.StartAction("TurnR3", 2);
			}else{
				ap.StartAction("TurnL3", 2);
			}
		}
		*/

	}

	public void ReadAssignFile(AutoPilot ap) {

		string ConsiderStringMoveF = ReadFile(Base, 2, "MoveF:");
		if(ConsiderStringMoveF.Length != 6){
			MoveF = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMoveF.Substring(6,ConsiderStringMoveF.Length - 6));
		}else{
			MoveF = KeyCode.W;
		}
		string ConsiderStringMoveB = ReadFile(Base, 2, "MoveB:");
		if(ConsiderStringMoveB.Length != 6){
			MoveB = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMoveB.Substring(6,ConsiderStringMoveB.Length - 6));
		}else{
			MoveB = KeyCode.S;
		}
		string ConsiderStringMoveL = ReadFile(Base, 2, "MoveL:");
		if(ConsiderStringMoveL.Length != 6){
			MoveL = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMoveL.Substring(6,ConsiderStringMoveL.Length - 6));
		}else{
			MoveL = KeyCode.Q;
		}
		string ConsiderStringMoveR = ReadFile(Base, 2, "MoveR:");
		if(ConsiderStringMoveR.Length != 6){
			MoveR = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMoveR.Substring(6,ConsiderStringMoveR.Length - 6));
		}else{
			MoveR = KeyCode.E;
		}
		string ConsiderStringMoveU = ReadFile(Base, 2, "MoveU:");
		if(ConsiderStringMoveU.Length != 6){
			MoveU = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMoveU.Substring(6,ConsiderStringMoveU.Length - 6));
		}else{
			MoveU = KeyCode.Space;
		}
		string ConsiderStringMoveD = ReadFile(Base, 2, "MoveD:");
		if(ConsiderStringMoveD.Length != 6){
			MoveD = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMoveD.Substring(6,ConsiderStringMoveD.Length - 6));
		}else{
			MoveD = KeyCode.LeftShift;
		}


		string ConsiderStringWep1 = ReadFile(Base, 2, "Wep1:");
		if(ConsiderStringWep1.Length != 5){
			Wep1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringWep1.Substring(5,ConsiderStringWep1.Length - 5));
		}else{
			Wep1 = KeyCode.Mouse0;
		}
		string ConsiderStringWep2 = ReadFile(Base, 2, "Wep2:");
		if(ConsiderStringWep2.Length != 5){
			Wep2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringWep2.Substring(5,ConsiderStringWep2.Length - 5));
		}else{
			Wep2 = KeyCode.Mouse1;
		}
		string ConsiderStringWep3 = ReadFile(Base, 2, "Wep3:");
		if(ConsiderStringWep3.Length != 5){
			Wep3 = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringWep3.Substring(5,ConsiderStringWep3.Length - 5));
		}else{
			Wep3 = KeyCode.Mouse2;
		}
		string ConsiderStringMissile = ReadFile(Base, 2, "Missile:");
		if(ConsiderStringMissile.Length != 8){
			Missile = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringMissile.Substring(8,ConsiderStringMissile.Length - 8));
		}else{
			Missile = KeyCode.Keypad6;
		}


		string ConsiderStringChangeOpt = ReadFile(Base, 2, "ChangeOpt:");
		if(ConsiderStringChangeOpt.Length != 10){
			ChangeOptKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringChangeOpt.Substring(10,ConsiderStringChangeOpt.Length - 10));
		}else{
			ChangeOptKey = KeyCode.Keypad5;
		}
		string ConsiderStringChangeLauncher = ReadFile(Base, 2, "ChangeLauncher:");
		if(ConsiderStringChangeLauncher.Length != 15){
			LauncherKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringChangeLauncher.Substring(15,ConsiderStringChangeLauncher.Length - 15));
		}else{
			LauncherKey = KeyCode.Keypad4;
		}
		string ConsiderStringAimAssist = ReadFile(Base, 2, "AimAssist:");
		if(ConsiderStringAimAssist.Length != 10){
			AimAssist = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringAimAssist.Substring(10,ConsiderStringAimAssist.Length - 10));
		}else{
			AimAssist = KeyCode.Alpha2;
		}
		string ConsiderStringBoostMode = ReadFile(Base, 2, "BoostMode:");
		if(ConsiderStringBoostMode.Length != 10){
			BoostKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringBoostMode.Substring(10,ConsiderStringBoostMode.Length - 10));
		}else{
			BoostKey = KeyCode.F;
		}

		string ConsiderStringAim = ReadFile(Base, 2, "Aim:");
		if(ConsiderStringAim.Length != 4){
			AimKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), ConsiderStringAim.Substring(4,ConsiderStringAim.Length - 4));
		}else{
			AimKey = KeyCode.T;
		}


		string ConsiderStringChangeAim = ReadFile(Base, 2, "AimChange:");
		if(ConsiderStringChangeAim.Length != 10){
			if(ConsiderStringChangeAim.Substring(10,ConsiderStringChangeAim.Length - 10) == "true"){
				ChangeAim = true;
			}else if(ConsiderStringChangeAim.Substring(10,ConsiderStringChangeAim.Length - 10) == "false"){
				ChangeAim = false;
			}
		}else{
			ChangeAim = true;
		}


		ap.Log("Loaded");
	}

	//以下FileSystem.csコピペ
	//Mode
	//1 : 読み込み(全行読み込み)
	//2 : 読み込み(検索したい文字列が入っている行のみ読み出す)

	public static string ReadFile (FileInfo FI, int mode, string value) {
		string ReadedString = "";
		if ( mode == 1 ) {
			try {
	            using (StreamReader RFsr = new StreamReader(FI.OpenRead(), Encoding.UTF8)){
	            	while (RFsr.Peek() >= 0){
	                	ReadedString = RFsr.ReadToEnd();
	            	}
	            	return ReadedString;
	            }
	        } catch (System.Exception e){
	            return e.Message;
	        }
		}else if ( mode == 2 ) {
			try {
	            using (StreamReader SRL = new StreamReader(FI.OpenRead(), Encoding.UTF8)){
	            	while (SRL.Peek() >= 0 && ReadedString.IndexOf(value) == -1){
	                	ReadedString = SRL.ReadLine();
	            	}
	            	if (SRL.Peek() < 0 && ReadedString.IndexOf(value) == -1){
	            		return null;
	            	}else if (SRL.Peek() < 0 || ReadedString.IndexOf(value) != -1){
	            		return ReadedString;
	            	}else{
	            		return null;
	            	}
	            }
	        } catch (System.Exception e){
	            return e.Message;
	        }
		}else{
			return "存在しないモード番号です。";
		}
	}
}