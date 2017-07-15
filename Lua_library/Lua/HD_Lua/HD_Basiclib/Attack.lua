--攻撃用関数

module("Atk", package.seeall)

local ap



--変数宣言-------

local TPSAimAssist_lag = Vector3.zero --TPSエイムアシストの旋回ラグ
local TPSAimAssist_lag_now = Vector3.zero --TPSエイムアシストの旋回ラグ
local TPSAimAssist_lag_val_now = 0 --旋回ラグの値
local TPSAimAssist_lag_val = 0 --旋回ラグの値

local FPSAimAssist_lag = Vector3.zero --FPSエイムアシストの旋回ラグ
local FPSAimAssist_lag_now = Vector3.zero --FPSエイムアシストの旋回ラグ
local FPSAimAssist_lag_val_now = 0 --旋回ラグの値
local FPSAimAssist_lag_val = 0 --旋回ラグの値

--初期化-----
function Init(_ap)
ap = _ap
end	

--FPSエイムアシスト-----
function FPS_aimassist(FPS_My_Pos,Target_Dis,C_Target_Dis,Assist_Pos,Gun_Speed,MySpeed,Inertia,FPS_Offset,Target_F)

	local Angul_difU = 0
	local Angul_difR = 0
	
	local FPS_lx = ap:GetRight(FPS_Offset[1])
	local FPS_ly = ap:GetUp(FPS_Offset[2])
	local FPS_lz = ap:GetForward(FPS_Offset[3])
	local FPS_oft = ap:AddVec(ap:AddVec(FPS_lx,FPS_ly),FPS_lz)
	local FPS_start = ap:AddVec(FPS_My_Pos,FPS_oft)
	
	local Control_Foward = Target_F
	Control_Foward =  ap:MulVec( Control_Foward , C_Target_Dis )
	local Camera_Pos = ap:GetCameraPosition()
	local Target_pos = ap:AddVec(Camera_Pos,Control_Foward)
	local time = Target_Dis / Gun_Speed
	Target_pos = ap:SubVec( Target_pos , ap:MulVec( MySpeed , Inertia * time ) ) 
	
	local Aim_pos = ap:AddVec(FPS_start,ap:GetForward(Target_Dis))
	
	local Percent_Vec = ap:SubVec(Target_pos,Assist_Pos)
	local Percent = Percent_Vec.sqrMagnitude
	Percent = Cal.limit(Percent/Target_Dis,0,150)/15 
	
	Target_pos[0] = Target_pos[0]*Percent/10 + Assist_Pos[0]*(10-Percent)/10
	Target_pos[1] = Target_pos[1]*Percent/10 + Assist_Pos[1]*(10-Percent)/10
	Target_pos[2] = Target_pos[2]*Percent/10 + Assist_Pos[2]*(10-Percent)/10
	
	FPSAimAssist_lag_now = ap:SubVec( Target_pos , Aim_pos)
	FPSAimAssist_lag_val_now = FPSAimAssist_lag_now.sqrMagnitude
	
	
	if FPSAimAssist_lag_val_now > FPSAimAssist_lag_val*0.9 then
		
		if FPSAimAssist_lag_val/Target_Dis > 100 then 
			FPSAimAssist_lag = ap:AddVec(ap:MulVec(FPSAimAssist_lag,0.5) ,ap:MulVec(ap:SubVec( Target_pos , Aim_pos),1.0))
		else
			FPSAimAssist_lag = ap:AddVec(ap:MulVec(FPSAimAssist_lag,0.4) ,ap:MulVec(ap:SubVec( Target_pos , Aim_pos),0.8))
		end
	else
		FPSAimAssist_lag = ap:AddVec(ap:MulVec(FPSAimAssist_lag,0.3) ,ap:MulVec(ap:SubVec( Target_pos , Aim_pos),0.7))
	end
	
	FPSAimAssist_lag_val = FPSAimAssist_lag_val_now
	
	if Percent < 8 and Target_Dis > 20 then
		Target_pos = ap:AddVec(Target_pos , FPSAimAssist_lag)
	end
	
	ap:Aim(Target_pos)
	
	local Target_Vec = ap:SubVec(Target_pos,FPS_start)
	
	local Target_Vec_x,Target_Vec_y,Target_Vec_z = Cal.GetLVec(Target_Vec.x,Target_Vec.y,Target_Vec.z)
	
	if math.abs(Target_Vec_y) < 0.001 then
		Angul_difU = 0
	else
		Angul_difU = math.deg(math.atan2(math.sqrt(Target_Vec_x^2 + Target_Vec_z^2),Target_Vec_y))-90
	end
	
	if math.abs(Target_Vec_x) < 0.001 then
		Angul_difR = 0
	else
		Angul_difR = math.deg(math.atan2(Target_Vec_x ,Target_Vec_z))
	end
	
	return Angul_difU,Angul_difR
	
end

--TPSエイム-----
function TPS_aim(TPS_My_Pos,Target_dis,Gun_Speed,MySpeed,Inertia,TPS_Offset)
	
	local Angul_difU = 0
	local Angul_difR = 0
	
	local TPS_lx = ap:GetRight(TPS_Offset[1])
	local TPS_ly = ap:GetUp(TPS_Offset[2])
	local TPS_lz = ap:GetForward(TPS_Offset[3])
	
	local TPS_oft = ap:AddVec(ap:AddVec(TPS_lx,TPS_ly),TPS_lz)
	local TPS_start = ap:AddVec(TPS_My_Pos,TPS_oft)
	
	local Camera_Foward = ap:GetCameraForward()
	Camera_Foward =  ap:MulVec( Camera_Foward , Target_dis )
	local Camera_Pos = ap:GetCameraPosition()
	local Target_pos = ap:AddVec(Camera_Pos,Camera_Foward)
	local time = Target_dis / Gun_Speed
	Target_pos = ap:SubVec( Target_pos , ap:MulVec( MySpeed , Inertia * time ) ) 

	--Draw.Draw3DTri(Target_pos ,Color(0.3,0.8,0.1),2)
	
	ap:Aim(Target_pos)
	
	local Target_Vec = ap:SubVec(Target_pos,TPS_start)
	
	--ap:DrawLine3D(Color(0.3,0.8,0.1),TPS_start,Target_pos)
	
	local Target_Vec_x,Target_Vec_y,Target_Vec_z = Cal.GetLVec(Target_Vec.x,Target_Vec.y,Target_Vec.z)
	
	if math.abs(Target_Vec_y) < 0.001 then
		Angul_difU = 0
	else
		Angul_difU = math.deg(math.atan2(math.sqrt(Target_Vec_x^2 + Target_Vec_z^2),Target_Vec_y))-90
	end
	
	if math.abs(Target_Vec_x) < 0.001 then
		Angul_difR = 0
	else
		Angul_difR = math.deg(math.atan2(Target_Vec_x ,Target_Vec_z))
	end
	
	return Angul_difU,Angul_difR
	
end	
	
	
--TPSアシスト付きエイム-----
function TPS_aim_assist(TPS_My_Pos,Target_Dis,C_Target_Dis,Assist_Pos,Gun_Speed,MySpeed,Inertia,TPS_Offset)

	local Angul_difU = 0
	local Angul_difR = 0
	
	local TPS_lx = ap:GetRight(TPS_Offset[1])
	local TPS_ly = ap:GetUp(TPS_Offset[2])
	local TPS_lz = ap:GetForward(TPS_Offset[3])
	local TPS_oft = ap:AddVec(ap:AddVec(TPS_lx,TPS_ly),TPS_lz)
	local TPS_start = ap:AddVec(TPS_My_Pos,TPS_oft)
	
	local Camera_Foward = ap:GetCameraForward()
	Camera_Foward =  ap:MulVec( Camera_Foward , C_Target_Dis )
	local Camera_Pos = ap:GetCameraPosition()
	local Target_pos = ap:AddVec(Camera_Pos,Camera_Foward)
	local time = Target_Dis / Gun_Speed
	Target_pos = ap:SubVec( Target_pos , ap:MulVec( MySpeed , Inertia * time ) ) 
	
	local Aim_pos = ap:AddVec(TPS_start,ap:GetForward(Target_Dis))
	
	local Percent_Vec = ap:SubVec(Target_pos,Assist_Pos)
	local Percent = Percent_Vec.sqrMagnitude
	Percent = Cal.limit(Percent/Target_Dis,0,150)/15 
	
	Target_pos[0] = Target_pos[0]*Percent/10 + Assist_Pos[0]*(10-Percent)/10
	Target_pos[1] = Target_pos[1]*Percent/10 + Assist_Pos[1]*(10-Percent)/10
	Target_pos[2] = Target_pos[2]*Percent/10 + Assist_Pos[2]*(10-Percent)/10
	
	TPSAimAssist_lag_now = ap:SubVec( Target_pos , Aim_pos)
	TPSAimAssist_lag_val_now = TPSAimAssist_lag_now.sqrMagnitude
	
	
	if TPSAimAssist_lag_val_now > TPSAimAssist_lag_val*0.9 then
		
		if TPSAimAssist_lag_val/Target_Dis > 100 then 
			TPSAimAssist_lag = ap:AddVec(ap:MulVec(TPSAimAssist_lag,0.5) ,ap:MulVec(ap:SubVec( Target_pos , Aim_pos),1.0))
		else
			TPSAimAssist_lag = ap:AddVec(ap:MulVec(TPSAimAssist_lag,0.4) ,ap:MulVec(ap:SubVec( Target_pos , Aim_pos),0.8))
		end
	else
		TPSAimAssist_lag = ap:AddVec(ap:MulVec(TPSAimAssist_lag,0.3) ,ap:MulVec(ap:SubVec( Target_pos , Aim_pos),0.7))
	end
	
	TPSAimAssist_lag_val = TPSAimAssist_lag_val_now
	
	if Percent < 8 and Target_Dis > 20 then
		Target_pos = ap:AddVec(Target_pos , TPSAimAssist_lag)
	end
	
	ap:Aim(Target_pos)
	
	local Target_Vec = ap:SubVec(Target_pos,TPS_start)
	
	local Target_Vec_x,Target_Vec_y,Target_Vec_z = Cal.GetLVec(Target_Vec.x,Target_Vec.y,Target_Vec.z)
	
	if math.abs(Target_Vec_y) < 0.001 then
		Angul_difU = 0
	else
		Angul_difU = math.deg(math.atan2(math.sqrt(Target_Vec_x^2 + Target_Vec_z^2),Target_Vec_y))-90
	end
	
	if math.abs(Target_Vec_x) < 0.001 then
		Angul_difR = 0
	else
		Angul_difR = math.deg(math.atan2(Target_Vec_x ,Target_Vec_z))
	end
	
	return Angul_difU,Angul_difR
	
end
	
	
--弾道表示-----
function Canonn_Ballistic(Mypos,Gun_Forward,Gun_Speed,MySpeed,Inertia,Offset)
	
	local C_B_lx = ap:GetRight(Offset[1])
	local C_B_ly = ap:GetUp(Offset[2])
	local C_B_lz = ap:GetForward(Offset[3])
	
	local C_B_oft = ap:AddVec(ap:AddVec(C_B_lx,C_B_ly),C_B_lz)
	local Ballistic_start = ap:AddVec(Mypos,C_B_oft)
		
		local Distance = 50
		local time = Distance / Gun_Speed
		local display_start = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Distance) , ap:MulVec( MySpeed , Inertia * time ) ) )
		time = Distance * 10 / Gun_Speed
		local Ballistic_Pos = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Distance*10) , ap:MulVec( MySpeed , Inertia * time ) ) )
		Draw.Draw3DRhombus(Ballistic_Pos,Color(0.0,0.8,0.8),8)
		time = Distance * 20 / Gun_Speed
		Ballistic_Pos = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Distance*20) , ap:MulVec( MySpeed , Inertia * time ) ) )
		Draw.Draw3DRhombus(Ballistic_Pos,Color(0.0,0.8,0.8),25)
		ap:DrawLine3D(Color(0.0,0.8,0.8),display_start,Ballistic_Pos)
	
end
----------

--弾道表示(距離指定)-----
function Distance_Ballistic(Mypos,Target_Dis,Gun_Forward,Gun_Speed,MySpeed,Inertia,Offset)
	
	local D_B_lx = ap:GetRight(Offset[1])
	local D_B_ly = ap:GetUp(Offset[2])
	local D_B_lz = ap:GetForward(Offset[3])
	
	local D_B_oft = ap:AddVec(ap:AddVec(D_B_lx,D_B_ly),D_B_lz)
	local Ballistic_start = ap:AddVec(Mypos,D_B_oft)
	local Distance = 40
	local time = Distance / Gun_Speed
	local display_start
	
		if Target_Dis > 40 then
			display_start = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Distance) , ap:MulVec( MySpeed , Inertia * time ) ) )
			D_time = Target_Dis / Gun_Speed
			local Ballistic_Pos = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Target_Dis) , ap:MulVec( MySpeed , Inertia * time ) ) )
			Draw.Draw3DRhombus(Ballistic_Pos,Color(0.0,0.8,0.8),8)
			ap:DrawLine3D(Color(0.0,0.8,0.8),display_start,Ballistic_Pos)
		elseif Target_Dis < 20 then
			time = Target_Dis / Gun_Speed
			local Ballistic_Pos = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Target_Dis) , ap:MulVec( MySpeed , Inertia * time ) ) )
			Draw.Draw3DTri(Ballistic_Pos,Color(0.8,0.8,0.0),8)	
		else
			time = Target_Dis / Gun_Speed
			local Ballistic_Pos = ap:AddVec( Ballistic_start ,ap:AddVec( ap:MulVec(Gun_Forward, Target_Dis) , ap:MulVec( MySpeed , Inertia * time ) ) )
			Draw.Draw3DRhombus(Ballistic_Pos,Color(0.0,0.8,0.8),8)
		end
	
end


--敵未来座標予測-----
function Prediction_Target(P_T_Enemypos,P_T_EnemySpeed,P_T_EnemyDis,P_T_Mypos,P_T_MySpeed,P_T_Bullet_Speed,P_T_Inertia,P_T_Offset)
	
	local P_T_lx = ap:GetRight(P_T_Offset[1])
	local P_T_ly = ap:GetUp(P_T_Offset[2])
	local P_T_lz = ap:GetForward(P_T_Offset[3])
	
	local P_T_oft = ap:AddVec(ap:AddVec(P_T_lx,P_T_ly),P_T_lz)
	local P_T_Muzzle_Pos = ap:AddVec(P_TMypos,P_T_oft)
	
	local P_T_FuturePos = ap:AddVec( P_T_Enemypos , ap:MulVec( ap:SubVec(P_T_EnemySpeed , ap:MulVec( P_T_MySpeed , P_T_Inertia ) ) , P_T_EnemyDis / P_T_Bullet_Speed  ) )
	
	ap:DrawLine3D(Color(1,0,0),P_T_Enemypos,P_T_FuturePos)
	Draw.Draw3DRhombus(P_T_FuturePos,Color(1,0,0),6)
	
	return P_T_FuturePos

end
----------

