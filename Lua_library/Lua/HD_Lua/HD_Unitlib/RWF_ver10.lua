--RWフレームver1.0用

module("RWF10", package.seeall)



--RWフレーム用変数定義-----------

local Gyro_PID_P = {0,0,0}	--ジャイロのPID出力値保持用(x,y,z)
local Gyro_PID_Dev1 = {0,0,0}	--ジャイロのPID制御保持用1(x,y,z) 
local Gyro_PID_Dev2 = {0,0,0}	--ジャイロのPID制御保持用2(x,y,z)
local Gyro_Power = {0,0,0}		--ジャイロの出力値(x,y,z)
local Gyro_Choice_P = {0,0,0}		--ジャイロの実効出力値(x,y,z)
local Gyro_flag = {0,0,0}		--ジャイロの出力実施フラグ(x,y,z)

local Gyro_Threshold_0 = 10		--ジャイロの閾値
local Gyro_Threshold_1 = 20
local Gyro_Threshold_2 = 30
local Gyro_Threshold_3 = 60
local Gyro_Threshold_4 = 80

local RWFtarget_F = Vector3.forward --仮想の姿勢方向ターゲットベクトル前方方向
local RWFtarget_R = Vector3.right 	--仮想の姿勢方向ターゲットベクトル右方方向
local RWFtarget_U = Vector3.up 	--仮想の姿勢方向ターゲットベクトル右方方向
local MytargetU = 0 			--ターゲットベクトルまでの角度U
local MytargetR = 0 			--ターゲットベクトルまでの角度R
local MyRoll = 0 				--ロール制御ベクトルまでの角度
local Roll_input = 0 				--手動ロール入力値
local Roll_holdR = 0 				--ロールR入力維持
local Roll_holdL = 0 				--ロールL入力維持

local RWF_wepon_speed = {0,0,0,0,800}		--銃の弾速
local RWF_wepon_type = {0,0,0,0,5}		--銃のタイプ(5はソード)
local RWF_wepon_Inertia = {0,0,0,0,0.2}		--銃の慣性
local RWF_InertiaSet = {0,0,0.5,0,0,0.2}		--銃の慣性見本(なし、キャノン、戦車砲、ビーマー、ランチャー、格闘)
local RWF_Gunpos = {{0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0}} --銃口の位置（コアからの座標x.y.z）

local Stub_chain = 0			--現在格闘段数把握用
local Stub_time = 0			--格闘入力時間保持用
local Stubhold_time = {0,0,0,0,0,0}			--各段数持続時間

local RWF_W_past = 0				--ブースト入力判定用
local RWF_B_past = 0
local RWF_L_past = 0
local RWF_R_past = 0
local RWF_U_past = 0
local RWF_D_past = 0
local RWF_RollL_past = 0
local RWF_RollR_past = 0
local RWF_Roll_time = 0 				--ロール時点火用

local RWF_Low_U = 0	--通常時移動カウント用
local RWF_Low_D = 0

local RWF_MyForward1 = Vector3.forward --1フレーム前の情報保持用
local RWF_MyRight1 = Vector3.right
local RWF_MyUp1 = Vector3.up
local RWF_Past1 = 0

local RWF_MyForward2 = Vector3.forward --2フレーム前の情報保持用
local RWF_MyRight2 = Vector3.right
local RWF_MyUp2 = Vector3.up
local RWF_Past2 = 0

local RWF_P_Velocity = 0 --角加速度
local RWF_Y_Velocity = 0
local RWF_R_Velocity = 0

local Startup_Time = Time.time

--初期化-----
function Init(_ap)
ap = _ap
end

--スタートアップ-----
function RWF_Startup(Startup_wepon_type,Startup_wepon_speed,startup_now,Stubtime,now_count)
	
	local Startup_fn = startup_now
	local Startup_forward = ap:AddVec(ap:GetPosition() ,ap:GetForward(1000))
	local offset = Vector3.zero
	
	Stubhold_time[1] = Stubtime[1]
	Stubhold_time[2] = Stubtime[2]
	Stubhold_time[3] = Stubtime[3]
	Stubhold_time[4] = Stubtime[4]
	Stubhold_time[5] = Stubtime[5]
	Stubhold_time[6] = Stubtime[6]
	
	if Startup_fn == 1 then
		if Startup_wepon_type ~= 0 then
			if Time.time > Startup_Time + 5 then
				ap:StartAction("aimpose1",1)
				ap:Aim(Startup_forward)
				ap:StartAction("camera1",1)
				
				RWF_wepon_speed[1] = Startup_wepon_speed
				RWF_wepon_type[1] = Startup_wepon_type
				RWF_wepon_Inertia[1] = RWF_InertiaSet[Startup_wepon_type+1]
				offset = ap:SubVec(ap:GetCameraPosition(),ap:GetPosition())
				
				local lx,ly,lz = Cal.GetLVec(offset.x,offset.y,offset.z)
				
				RWF_Gunpos[1][1] = lx
				RWF_Gunpos[1][2] = ly
				RWF_Gunpos[1][3] = lz
				
				Startup_Time = Time.time
				Startup_fn = Startup_fn + 1
			else
				ap:StartAction("aimpose1",1)
				ap:StartAction("camera1",1)
				ap:Aim(Startup_forward)
			end
		else
			
			Startup_Time = Time.time
			Startup_fn = Startup_fn + 1
		end
		
	elseif Startup_fn == 2 then
		if Startup_wepon_type ~= 0 then
			if Time.time > Startup_Time + 5 then
				ap:StartAction("aimpose2",1)
				ap:Aim(Startup_forward)
				ap:StartAction("camera2",1)
				
				RWF_wepon_speed[2] = Startup_wepon_speed
				RWF_wepon_type[2] = Startup_wepon_type
				RWF_wepon_Inertia[2] = RWF_InertiaSet[Startup_wepon_type+1]
				offset = ap:SubVec(ap:GetCameraPosition(),ap:GetPosition())
				
				local lx,ly,lz = Cal.GetLVec(offset.x,offset.y,offset.z)
				
				RWF_Gunpos[2][1] = lx
				RWF_Gunpos[2][2] = ly
				RWF_Gunpos[2][3] = lz
				
				Startup_Time = Time.time
				Startup_fn = Startup_fn + 1
			else
				ap:StartAction("aimpose2",1)
				ap:StartAction("camera2",1)
				ap:Aim(Startup_forward)
			end
		else
			
			Startup_Time = Time.time
			Startup_fn = Startup_fn + 1
		end
	
	elseif Startup_fn == 3 then
		if Startup_wepon_type ~= 0 then
			if Time.time > Startup_Time + 5 then
				ap:StartAction("aimpose3",1)
				ap:Aim(Startup_forward)
				ap:StartAction("camera3",1)
				
				RWF_wepon_speed[3] = Startup_wepon_speed
				RWF_wepon_type[3] = Startup_wepon_type
				RWF_wepon_Inertia[3] = RWF_InertiaSet[Startup_wepon_type+1]
				offset = ap:SubVec(ap:GetCameraPosition(),ap:GetPosition())
				local lx,ly,lz = Cal.GetLVec(offset.x,offset.y,offset.z)
				
				RWF_Gunpos[3][1] = lx
				RWF_Gunpos[3][2] = ly
				RWF_Gunpos[3][3] = lz
				
				Startup_Time = Time.time
				Startup_fn = Startup_fn + 1
			else
				ap:StartAction("aimpose3",1)
				ap:StartAction("camera3",1)
				ap:Aim(Startup_forward)
			end
		else
			
			Startup_Time = Time.time
			Startup_fn = Startup_fn + 1
		end
	
	elseif Startup_fn == 4 then
		if Startup_wepon_type ~= 0 then
			if Time.time > Startup_Time + 5 then
				ap:StartAction("aimpose4",1)
				ap:Aim(Startup_forward)
				ap:StartAction("camera4",1)
				
				RWF_wepon_speed[4] = Startup_wepon_speed
				RWF_wepon_type[4] = Startup_wepon_type
				RWF_wepon_Inertia[4] = RWF_InertiaSet[Startup_wepon_type+1]
				offset = ap:SubVec(ap:GetCameraPosition(),ap:GetPosition())
				local lx,ly,lz = Cal.GetLVec(offset.x,offset.y,offset.z)
				
				RWF_Gunpos[4][1] = lx
				RWF_Gunpos[4][2] = ly
				RWF_Gunpos[4][3] = lz
				
				Startup_Time = Time.time
				Startup_fn = Startup_fn + 1
			else
				ap:StartAction("aimpose4",1)
				ap:StartAction("camera4",1)
				ap:Aim(Startup_forward)
			end
		else
			
			Startup_Time = Time.time
			Startup_fn = Startup_fn + 1
		end
	end
	
	MytargetR = -ap:GetDirection()
	MyRoll = ap:GetBank()
	MytargetU = ap:GetPitch()
	
	RWF_Gyro_X(MytargetR,now_count)
	RWF_Gyro_Y(MytargetU,now_count)
	RWF_Gyro_Z(MyRoll,now_count)
	
 	return Startup_fn
end

--自機情報アップデート-----
function RWF_Myupdate(RWF_Forward,RWF_Right,RWF_Up)
	
	RWF_P_Velocity = Info.Get_L_AngleP_Velocity(RWF_MyRight2,RWF_MyUp2,RWF_MyForward2)/(Time.time-RWF_Past2)/20
	RWF_Y_Velocity = Info.Get_L_AngleY_Velocity(RWF_MyRight2,RWF_MyUp2,RWF_MyForward2)/(Time.time-RWF_Past2)/20
	RWF_R_Velocity = Info.Get_L_AngleR_Velocity(RWF_MyRight2,RWF_MyUp2,RWF_MyForward2)/(Time.time-RWF_Past2)/20
	
	RWF_MyForward2 = RWF_MyForward1
	RWF_MyRight2 = RWF_MyRight1
	RWF_MyUp2 = RWF_MyUp1
	RWF_Past2 = RWF_Past1
	
	RWF_MyForward1 = RWF_Forward
	RWF_MyRight1 = RWF_Right
	RWF_MyUp1 = RWF_Up
	RWF_Past1 = Time.time
	
end

--敵機情報アップデート-----
function RWF_Enemyupdate(RWF_EnemyPos,RWF_EnemySpeed,RWF_EnemyDis,RWF_MyPos,RWF_MySpeed,Weapon_Choice)
	
	local RWF_EnemyPred_Pos =  Atk.Prediction_Target(RWF_EnemyPos,RWF_EnemySpeed,RWF_EnemyDis,RWF_MyPos,RWF_MySpeed,RWF_wepon_speed[Weapon_Choice],RWF_wepon_Inertia[Weapon_Choice],RWF_Gunpos[Weapon_Choice])
	
	return RWF_EnemyPred_Pos
end

--通常時アップデート-----
function RWF_Nomal_update(now_count)

	if Input.GetKey(KeyCode.W) then --前進
		ap:StartAction("walk",1)
	end

	if Input.GetKey(KeyCode.S) then --後退
			ap:StartAction("walk_B",1)
	end	
	
	MytargetR = 0
	
	if Input.GetKey(KeyCode.D) then --右旋回	
			MytargetR = 50
	end	
	
	if Input.GetKey(KeyCode.A) then --左旋回	
			MytargetR = -50
	end	
	
	
	if Input.GetKey(KeyCode.Space) then --上昇
		if RWF_Low_U == 8 then
			ap:StartAction("MoveU",1)
			RWF_Low_U = 0
		else
			RWF_Low_U = RWF_Low_U + 1
		end
	end	
	
	if Input.GetKey(KeyCode.LeftShift) then --降下
		if RWF_Low_D == 8 then
			ap:StartAction("MoveD",1)
			RWF_Low_D = 0
		else
			RWF_Low_D = RWF_Low_D + 1
		end
	end	
	
	MyRoll = ap:GetBank()
	MytargetU = ap:GetPitch()
	
	RWF_Gyro_X(MytargetR,now_count)
	RWF_Gyro_Y(MytargetU,now_count)
	RWF_Gyro_Z(MyRoll,now_count)
	
	RWFtarget_F = ap:GetForward()
	RWFtarget_R = ap:GetRight()
	RWFtarget_U = ap:GetUp()
end

--戦闘時アップデート-----
function RWF_Combat_update(weapon_ch)
	
	if Input.GetKey(KeyCode.W) then --前進
		ap:StartAction("MoveF",1)
		if Input.GetKeyDown(KeyCode.W) then
			if Time.time < RWF_W_past + 0.5 then
				ap:StartAction("F_boost",1)
			else
				RWF_W_past = Time.time
			end
		end
	end

	if Input.GetKey(KeyCode.S) then --後退
		ap:StartAction("MoveB",1)
		if Input.GetKeyDown(KeyCode.S) then
			if Time.time < RWF_B_past + 0.5 then
				ap:StartAction("B_boost",1)
			else
				RWF_B_past = Time.time
			end
		end
	end	
	
	if Input.GetKey(KeyCode.D) then --右移動	
		ap:StartAction("MoveR",1)
		if Input.GetKeyDown(KeyCode.D) then
			if Time.time < RWF_R_past + 0.5 then
				ap:StartAction("R_boost",1)
			else
				RWF_R_past = Time.time
			end
		end
	end	
	
	if Input.GetKey(KeyCode.A) then --左移動	
		ap:StartAction("MoveL",1)
		if Input.GetKeyDown(KeyCode.A) then
			if Time.time < RWF_L_past + 0.5 then
				ap:StartAction("L_boost",1)
			else
				RWF_L_past = Time.time
			end
		end
	end	
	
	
	if Input.GetKey(KeyCode.Space) then --上昇
		ap:StartAction("MoveU",1)
		if Input.GetKeyDown(KeyCode.Space) then
			if Time.time < RWF_U_past + 0.5 then
				ap:StartAction("U_boost",1)
			else
				RWF_U_past = Time.time
			end
		end	
	end	
	
	if Input.GetKey(KeyCode.LeftShift) then --降下
		ap:StartAction("MoveD",1)
		if Input.GetKeyDown(KeyCode.LeftShift) then
			if Time.time < RWF_D_past + 0.5 then
				ap:StartAction("D_boost",1)
			else
				RWF_D_past = Time.time
			end
		end	
	end	
	
	if Input.GetKey(KeyCode.Q) then --左ロール
		Roll_input = Roll_input + 20
		if Input.GetKeyDown(KeyCode.Q) then
			if Time.time < RWF_RollL_past + 0.5 then
				Roll_L = 1
				Roll_R = 0
				RWF_Roll_time = Time.time
			else
				RWF_RollL_past = Time.time
			end
		end	
	
	elseif Input.GetKey(KeyCode.E) then --右ロール
		Roll_input = Roll_input - 20
		if Input.GetKeyDown(KeyCode.E) then
			if Time.time < RWF_RollR_past + 0.5 then
				Roll_L = 0
				Roll_R = 1
				RWF_Roll_time = Time.time
			else
				RWF_RollR_past = Time.time
			end
		end
		
	else
		if Roll_L == 1 and Time.time < RWF_RollL_past + 2 then
			Roll_input = Roll_input + 20
			if Time.time > RWF_Roll_time + 0.4 and Time.time < RWF_Roll_time + 0.7 then
				ap:StartAction("U_boost",1)
			end
		elseif Roll_R == 1 and Time.time < RWF_RollR_past + 2 then
			Roll_input = Roll_input - 20
			if Time.time > RWF_Roll_time + 0.4 and Time.time < RWF_Roll_time + 0.7 then
				ap:StartAction("U_boost",1)
			end
		else
			Roll_L = 0
			Roll_R = 0
			Roll_input = 0
		end
	end	
	
	
	if Input.GetKey(KeyCode.Mouse0) then
		if weapon_ch == 1 then
			Stub_chain = 0
			ap:StartAction("weapon1",1)
		elseif weapon_ch == 2 then
			Stub_chain = 0
			ap:StartAction("weapon2",1)
		elseif weapon_ch == 3 then
			Stub_chain = 0
			ap:StartAction("weapon3",1)
		elseif weapon_ch == 4 then
			Stub_chain = 0
			ap:StartAction("weapon4",1)
		elseif weapon_ch == 5 then
			RWF_Stub_action()
		end
	else
		Stub_chain = 0
	end
	
	if weapon_ch == 1 then
		ap:StartAction("aimpose1",1)
	elseif weapon_ch == 2 then
		ap:StartAction("aimpose2",1)
	elseif weapon_ch == 3 then
		ap:StartAction("aimpose3",1)
	elseif weapon_ch == 4 then
		ap:StartAction("aimpose4",1)
	end
	
end

--弾道表示-----
function RWF_Ballistic_display(MyPos,EnemyDis,MyForward,MySpeed,weapon_ch,lockon)
	
	if lockon == false then
		Atk.Canonn_Ballistic(MyPos,MyForward,RWF_wepon_speed[weapon_ch],MySpeed,RWF_wepon_Inertia[weapon_ch],RWF_Gunpos[weapon_ch])
	else
		Atk.Distance_Ballistic(MyPos,EnemyDis,MyForward,RWF_wepon_speed[weapon_ch],MySpeed,RWF_wepon_Inertia[weapon_ch],RWF_Gunpos[weapon_ch])
	end
end

--FPSアップデート-----
function RWF_FPS_update(now_count,MyPos,Mouse_X,Mouse_Y,weapon_ch)
	
	RWFtarget_F,RWFtarget_R,RWFtarget_U,MytargetU,MytargetR = InCon.Target_vector_Control(RWFtarget_F,RWFtarget_R,RWFtarget_U,Mouse_X,Mouse_Y,MytargetU,MytargetR,MyPos)
	
	MyRoll = Info.Get_FreeAngleR(RWFtarget_R,RWFtarget_U,RWFtarget_F)
	
	ap:StartAction("FPS",1)
	
	--if weapon_ch == 1 then
	--	ap:StartAction("camera1",1)
	--elseif weapon_ch == 2 then
	--	ap:StartAction("camera2",1)
	--elseif weapon_ch == 3 then
	--	ap:StartAction("camera3",1)
	--elseif weapon_ch == 4 then
	--	ap:StartAction("camera4",1)
	--elseif weapon_ch == 5 then
	
	--end
	
	RWF_Gyro_X(MytargetR,now_count)
	RWF_Gyro_Y(MytargetU,now_count)
	RWF_Gyro_Z(MyRoll+ Roll_input,now_count)
end

--ロックオンFPSアップデート-----
function RWF_LockonFPS_update(now_count,MyPos,EnemyDis,C_EnemyDis,EnemyPred_Pos,MySpeed,Mouse_X,Mouse_Y,weapon_ch)
	
	RWFtarget_F,RWFtarget_R,RWFtarget_U,MytargetU,MytargetR = InCon.Target_vector_Control(RWFtarget_F,RWFtarget_R,RWFtarget_U,Mouse_X,Mouse_Y,MytargetU,MytargetR,MyPos)
	
	MyRoll = Info.Get_FreeAngleR(RWFtarget_R,RWFtarget_U,RWFtarget_F)
	
	local Assist_U,Assist_R = Atk.FPS_aimassist(MyPos,EnemyDis,C_EnemyDis,EnemyPred_Pos,RWF_wepon_speed[weapon_ch],MySpeed,RWF_wepon_Inertia[weapon_ch],RWF_Gunpos[weapon_ch],RWFtarget_F)
	
	ap:StartAction("FPS",1)
	
	--if weapon_ch == 1 then
	--	ap:StartAction("camera1",1)
	--elseif weapon_ch == 2 then
	--	ap:StartAction("camera2",1)
	--elseif weapon_ch == 3 then
	--	ap:StartAction("camera3",1)
	--elseif weapon_ch == 4 then
	--	ap:StartAction("camera4",1)
	--elseif weapon_ch == 5 then
	
	--end
	
	RWF_Gyro_X(Assist_R,now_count)
	RWF_Gyro_Y(Assist_U,now_count)
	RWF_Gyro_Z(MyRoll+ Roll_input,now_count)

end


--TPSアップデート-----
function RWF_TPS_update(now_count,MyPos,C_EnemyDis,MySpeed,weapon_ch,C_right,C_up,C_forward)
		
	MytargetU,MytargetR = Atk.TPS_aim(MyPos,C_EnemyDis,RWF_wepon_speed[weapon_ch],MySpeed,RWF_wepon_Inertia[weapon_ch],RWF_Gunpos[weapon_ch])
	
	RWFtarget_F = C_forward
	RWFtarget_R = C_right
	RWFtarget_U = C_up
	
	MyRoll = Info.Get_FreeAngleR(RWFtarget_R,RWFtarget_U,RWFtarget_F)
	
	RWF_Gyro_X(MytargetR,now_count)
	RWF_Gyro_Y(MytargetU,now_count)
	RWF_Gyro_Z(MyRoll + Roll_input,now_count)
	
end


--ロックオンTPSアップデート-----
function RWF_LockonTPS_update(now_count,MyPos,EnemyDis,C_EnemyDis,EnemyPred_Pos,MySpeed,weapon_ch,C_right,C_up,C_forward)

	MytargetU,MytargetR = Atk.TPS_aim_assist(MyPos,EnemyDis,C_EnemyDis,EnemyPred_Pos,RWF_wepon_speed[weapon_ch],MySpeed,RWF_wepon_Inertia[weapon_ch],RWF_Gunpos[weapon_ch])
	
	RWFtarget_F = C_forward
	RWFtarget_R = C_right
	RWFtarget_U = C_up
	
	MyRoll = Info.Get_FreeAngleR(RWFtarget_R,RWFtarget_U,RWFtarget_F)
	
	RWF_Gyro_X(MytargetR,now_count)
	RWF_Gyro_Y(MytargetU,now_count)
	RWF_Gyro_Z(MyRoll + Roll_input,now_count)
	
end

--格闘モーション--------------------
function RWF_Stub_action()
	
	ap:StartAction("Sword",1)
	if Stub_chain == 0 then --0段目
		ap:StartAction("stub1",1)
		Stub_time = Time.time
		Stub_chain = Stub_chain + 1
	elseif Stub_chain == 1 then --1段目
		ap:StartAction("stub1",1)
		if Stub_time + Stubhold_time[1] < Time.time then
			if Stubhold_time[1] > 99 then
				Stub_time = Time.time
			elseif Stubhold_time[1] == 0 then
				Stub_chain = 1
				Stub_time = Time.time
			else
				Stub_time = Time.time
				Stub_chain = Stub_chain + 1
			end
		end
	elseif Stub_chain == 2 then --2段目	
		ap:StartAction("stub2",1)
		if Stub_time + Stubhold_time[2] < Time.time then
			if Stubhold_time[2] > 99 then
				Stub_time = Time.time
			elseif Stubhold_time[2] == 0 then
				Stub_chain = 1
				Stub_time = Time.time
			else
				Stub_time = Time.time
				Stub_chain = Stub_chain + 1
			end
		end
	elseif Stub_chain == 3 then --3段目	
		ap:StartAction("stub3",1)
		if Stub_time + Stubhold_time[3] < Time.time then
			if Stubhold_time[3] > 99 then
				Stub_time = Time.time
			elseif Stubhold_time[3] == 0 then
				Stub_chain = 1
				Stub_time = Time.time
			else
				Stub_time = Time.time
				Stub_chain = Stub_chain + 1
			end
		end
	elseif Stub_chain == 4 then --4段目
		ap:StartAction("stub4",1)
		if Stub_time + Stubhold_time[4] < Time.time then	
			if Stubhold_time[4] > 99 then
				Stub_time = Time.time
			elseif Stubhold_time[4] == 0 then
				Stub_chain = 1
				Stub_time = Time.time
			else
				Stub_time = Time.time
				Stub_chain = Stub_chain + 1
			end
		end
	elseif Stub_chain == 5 then --5段目	
		ap:StartAction("stub5",1)
		if Stub_time + Stubhold_time[5] < Time.time then
			if Stubhold_time[5] > 99 then
				Stub_time = Time.time
			elseif Stubhold_time[5] == 0 then
				Stub_chain = 1
				Stub_time = Time.time
			else
				Stub_time = Time.time
				Stub_chain = Stub_chain + 1
			end
		end
	elseif Stub_chain == 6 then --6段目	
		ap:StartAction("stub6",1)
		if Stub_time + Stubhold_time[6] < Time.time then
			if Stubhold_time[6] > 99 then
				Stub_time = Time.time
			else
				Stub_chain = 1
				Stub_time = Time.time
			end
		end
	
	end

end



--ジャイロ制御(X軸)--------------------
function RWF_Gyro_X(RWF_MytargetR,RWF_Pulse_count)

Gyro_Power[1],Gyro_PID_P[1],Gyro_PID_Dev1[1],Gyro_PID_Dev2[1] = Gyro.Angle_Gyro_Output(RWF_MytargetR,RWF_Y_Velocity,Gyro_PID_P[1],Gyro_PID_Dev1[1],Gyro_PID_Dev2[2],RWF_Pulse_count,1,2)

	if Gyro_Power[1] > Gyro_Threshold_4 then
		Gyro_Choice_P[1] = Gyro_Power[1] - Gyro_Threshold_4
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_4)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnR2",1) --100
		else
			ap:StartAction("TurnR2",1) --75
			ap:StartAction("TurnR1",1)
		end
	elseif Gyro_Power[1] > Gyro_Threshold_3 then
		Gyro_Choice_P[1] = Gyro_Power[1] - Gyro_Threshold_3
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_3)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnR2",1) --75
			ap:StartAction("TurnR1",1)
		else
			ap:StartAction("TurnR1",1) --50
		end

	elseif Gyro_Power[1] > Gyro_Threshold_2 then
		Gyro_Choice_P[1] = Gyro_Power[1] - Gyro_Threshold_2
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_2)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnR1",1) --50
		else
			ap:StartAction("TurnR2",1) --33
			ap:StartAction("TurnR1",1)
			ap:StartAction("TurnL1",1)
		end
		
	elseif Gyro_Power[1] > Gyro_Threshold_1 then
		Gyro_Choice_P[1] = Gyro_Power[1] - Gyro_Threshold_1
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_1)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnR2",1) --33
			ap:StartAction("TurnR1",1)
			ap:StartAction("TurnL1",1)
		else
			ap:StartAction("TurnR2",1) --25
			ap:StartAction("TurnL1",1)
		end
	elseif Gyro_Power[1] > Gyro_Threshold_0 then
		Gyro_Choice_P[1] = Gyro_Power[1] - Gyro_Threshold_0
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_0)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnR2",1) --25
			ap:StartAction("TurnL1",1)
		end
		
	elseif Gyro_Power[1] < -Gyro_Threshold_4 then
		Gyro_Choice_P[1] = - Gyro_Power[1] - Gyro_Threshold_4
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_4)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnL2",1)
		else
			ap:StartAction("TurnL2",1)
			ap:StartAction("TurnL1",1)
		end
	elseif Gyro_Power[1] < -Gyro_Threshold_3 then
		Gyro_Choice_P[1] = - Gyro_Power[1] - Gyro_Threshold_3
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_3)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnL2",1)
			ap:StartAction("TurnL1",1)
		else
			ap:StartAction("TurnL1",1)
		end
	elseif Gyro_Power[1] < -Gyro_Threshold_2 then
		Gyro_Choice_P[1] = - Gyro_Power[1] - Gyro_Threshold_2
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_2)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnL1",1)
		else
			ap:StartAction("TurnL2",1)
			ap:StartAction("TurnL1",1)
			ap:StartAction("TurnR1",1)
		end
	elseif Gyro_Power[1] < -Gyro_Threshold_1 then
		Gyro_Choice_P[1] = - Gyro_Power[1] - Gyro_Threshold_1
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_1)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnL2",1)
			ap:StartAction("TurnL1",1)
			ap:StartAction("TurnR1",1)
		else
			ap:StartAction("TurnL2",1)
			ap:StartAction("TurnR1",1)
		end
		elseif Gyro_Power[1] < -Gyro_Threshold_0 then
		Gyro_Choice_P[1] = - Gyro_Power[1] - Gyro_Threshold_0
		Gyro_Choice_P[1] = Gyro_Choice_P[1] * 100 / (100 - Gyro_Threshold_0)
		Gyro_flag[1] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[1])
		
		if Gyro_flag[1] == 1 then
			ap:StartAction("TurnL2",1)
			ap:StartAction("TurnR1",1)
		end
	
	end
end

--ジャイロ制御(Y軸)--------------------
function RWF_Gyro_Y(RWF_MytargetU,RWF_Pulse_count)

Gyro_Power[2],Gyro_PID_P[2],Gyro_PID_Dev1[2],Gyro_PID_Dev2[2] = Gyro.Angle_Gyro_Output(RWF_MytargetU,RWF_P_Velocity,Gyro_PID_P[2],Gyro_PID_Dev1[2],Gyro_PID_Dev2[2],RWF_Pulse_count,1,2)

	if Gyro_Power[2] > Gyro_Threshold_4 then
		Gyro_Choice_P[2] = Gyro_Power[2] - Gyro_Threshold_4
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_4)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnD2",1)
		else
			ap:StartAction("TurnD2",1)
			ap:StartAction("TurnD1",1)
		end
	elseif Gyro_Power[2] > Gyro_Threshold_3 then
		Gyro_Choice_P[2] = Gyro_Power[2] - Gyro_Threshold_3
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_3)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnD2",1)
			ap:StartAction("TurnD1",1)
		else
			ap:StartAction("TurnD1",1)
		end

	elseif Gyro_Power[2] > Gyro_Threshold_2 then
		Gyro_Choice_P[2] = Gyro_Power[2] - Gyro_Threshold_2
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_2)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnD1",1)
		else
			ap:StartAction("TurnD2",1)
			ap:StartAction("TurnD1",1)
			ap:StartAction("TurnU1",1)
		end
		
	elseif Gyro_Power[2] > Gyro_Threshold_1 then
		Gyro_Choice_P[2] = Gyro_Power[2] - Gyro_Threshold_1
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_1)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnD2",1)
			ap:StartAction("TurnD1",1)
			ap:StartAction("TurnU1",1)
		else
			ap:StartAction("TurnD2",1)
			ap:StartAction("TurnU1",1)
		end
	elseif Gyro_Power[2] > Gyro_Threshold_0 then
		Gyro_Choice_P[2] = Gyro_Power[2] - Gyro_Threshold_0
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_0)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnD2",1)
			ap:StartAction("TurnU1",1)
		end
		
	elseif Gyro_Power[2] < -Gyro_Threshold_4 then
		Gyro_Choice_P[2] = - Gyro_Power[2] - Gyro_Threshold_4
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_4)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnU2",1)
		else
			ap:StartAction("TurnU2",1)
			ap:StartAction("TurnU1",1)
		end
	elseif Gyro_Power[2] < -Gyro_Threshold_3 then
		Gyro_Choice_P[2] = - Gyro_Power[2] - Gyro_Threshold_3
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_3)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnU2",1)
			ap:StartAction("TurnU1",1)
		else
			ap:StartAction("TurnU1",1)
		end
	elseif Gyro_Power[2] < -Gyro_Threshold_2 then
		Gyro_Choice_P[2] = - Gyro_Power[2] - Gyro_Threshold_2
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_2)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnU1",1)
		else
			ap:StartAction("TurnU2",1)
			ap:StartAction("TurnU1",1)
			ap:StartAction("TurnD1",1)
		end
	elseif Gyro_Power[2] < -Gyro_Threshold_1 then
		Gyro_Choice_P[2] = - Gyro_Power[2] - Gyro_Threshold_1
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_1)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnU2",1)
			ap:StartAction("TurnU1",1)
			ap:StartAction("TurnD1",1)
		else
			ap:StartAction("TurnU2",1)
			ap:StartAction("TurnD1",1)
		end
		elseif Gyro_Power[2] < -Gyro_Threshold_0 then
		Gyro_Choice_P[2] = - Gyro_Power[2] - Gyro_Threshold_0
		Gyro_Choice_P[2] = Gyro_Choice_P[2] * 100 / (100 - Gyro_Threshold_0)
		Gyro_flag[2] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[2])
		
		if Gyro_flag[2] == 1 then
			ap:StartAction("TurnU2",1)
			ap:StartAction("TurnD1",1)
		end
	
	end

end

--ジャイロ制御(Z軸)--------------------
function RWF_Gyro_Z(RWF_MyRoll,RWF_Pulse_count)

Gyro_Power[3],Gyro_PID_P[3],Gyro_PID_Dev1[3],Gyro_PID_Dev2[3] = Gyro.Angle_Gyro_Output(RWF_MyRoll,RWF_R_Velocity,Gyro_PID_P[3],Gyro_PID_Dev1[3],Gyro_PID_Dev2[3],RWF_Pulse_count,1,2)

	if Gyro_Power[3] > Gyro_Threshold_4 then
		Gyro_Choice_P[3] = Gyro_Power[3] - Gyro_Threshold_4
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_4)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollR2",1)
		else
			ap:StartAction("RollR2",1)
			ap:StartAction("RollR1",1)
		end
	elseif Gyro_Power[3] > Gyro_Threshold_3 then
		Gyro_Choice_P[3] = Gyro_Power[3] - Gyro_Threshold_3
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_3)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollR2",1)
			ap:StartAction("RollR1",1)
		else
			ap:StartAction("RollR1",1)
		end

	elseif Gyro_Power[3] > Gyro_Threshold_2 then
		Gyro_Choice_P[3] = Gyro_Power[3] - Gyro_Threshold_2
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_2)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollR1",1)
		else
			ap:StartAction("RollR2",1)
			ap:StartAction("RollR1",1)
			ap:StartAction("RollL1",1)
		end
		
	elseif Gyro_Power[3] > Gyro_Threshold_1 then
		Gyro_Choice_P[3] = Gyro_Power[3] - Gyro_Threshold_1
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_1)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollR2",1)
			ap:StartAction("RollR1",1)
			ap:StartAction("RollL1",1)
		else
			ap:StartAction("RollR2",1)
			ap:StartAction("RollL1",1)
		end
	elseif Gyro_Power[3] > Gyro_Threshold_0 then
		Gyro_Choice_P[3] = Gyro_Power[3] - Gyro_Threshold_0
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_0)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollR2",1)
			ap:StartAction("RollL1",1)
		end
		
	elseif Gyro_Power[3] < -Gyro_Threshold_4 then
		Gyro_Choice_P[3] = - Gyro_Power[3] - Gyro_Threshold_4
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_4)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollL2",1)
		else
			ap:StartAction("RollL2",1)
			ap:StartAction("RollL1",1)
		end
	elseif Gyro_Power[3] < -Gyro_Threshold_3 then
		Gyro_Choice_P[3] = - Gyro_Power[3] - Gyro_Threshold_3
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_3)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollL2",1)
			ap:StartAction("RollL1",1)
		else
			ap:StartAction("RollL1",1)
		end
	elseif Gyro_Power[3] < -Gyro_Threshold_2 then
		Gyro_Choice_P[3] = - Gyro_Power[3] - Gyro_Threshold_2
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_2)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollL1",1)
		else
			ap:StartAction("RollL2",1)
			ap:StartAction("RollL1",1)
			ap:StartAction("RollR1",1)
		end
	elseif Gyro_Power[3] < -Gyro_Threshold_1 then
		Gyro_Choice_P[3] = - Gyro_Power[3] - Gyro_Threshold_1
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_1)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollL2",1)
			ap:StartAction("RollL1",1)
			ap:StartAction("RollR1",1)
		else
			ap:StartAction("RollL2",1)
			ap:StartAction("RollR1",1)
		end
		elseif Gyro_Power[3] < -Gyro_Threshold_0 then
		Gyro_Choice_P[3] = - Gyro_Power[3] - Gyro_Threshold_0
		Gyro_Choice_P[3] = Gyro_Choice_P[3] * 100 / (100 - Gyro_Threshold_0)
		Gyro_flag[3] = OutCon.Pulse(RWF_Pulse_count,Gyro_Choice_P[3])
		
		if Gyro_flag[3] == 1 then
			ap:StartAction("RollL2",1)
			ap:StartAction("RollR1",1)
		end
			
	end

end

