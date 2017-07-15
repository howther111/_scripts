--�}�V���N���t�g�̏��擾�p

module("Info", package.seeall)

--������-----
function Init(_ap)
ap = _ap
end

local Time_Past = 0
local FrameCount_Past = 0

--�}�V���̃s�b�`�p�擾(rad)-----
function Get_AnglePitch()
	
	local Forward = ap:GetForward()
	return math.atan2(Forward.y, math.sqrt(Forward.x^2 + Forward.z^2))
	
end	

--�}�V���̃��[�p�擾(rad)-----
function Get_AngleYaw()
	
	local Forward = ap:GetForward()
	return math.atan2(Forward.x, Forward.z)
	
end	

--�}�V���̃��[���p�擾(rad)-----
function Get_AngleRoll()
	
	local Right = ap:GetRight()
	return math.atan2(Right.y, math.sqrt(Right.x^2 + Right.z^2))
	
end	

--�}�V���̃s�b�`�p���x�擾(world���W��Arad)-----
function Get_AngleP_Velocity(AngleP_Past)
	
	local AngleP = Info.Get_AnglePitch()
	local AngleP_V = 0
	
	AngleP_V = AngleP - AngleP_Past
	return AngleP_V , AngleP
end	

--�}�V���̃��[�p���x�擾(world���W��Arad)-----
function Get_AngleY_Velocity(AngleY_Past)
	
	local AngleY = Info.Get_AngleYaw()
	local AngleY_V = 0
	
	AngleY_V = AngleY - AngleY_Past
	return AngleY_V ,AngleY
end	

--�}�V���̃��[���p���x�擾(world���W��Arad)-----
function Get_AngleR_Velocity(AngleY_Past)
	
	local AngleR = Info.Get_AngleRoll()
	local AngleR_V = 0
	
	AngleR_V = AngleR - AngleR_Past
	return AngleR_V ,AngleR
end	

--�}�V���̃s�b�`�p���x�擾(���[�J�����W��A�o�͓͂x)-----
function Get_L_AngleP_Velocity(Right_past,Up_past,Forward_past)
	
	local AR_Forward = ap:GetForward()
	local AP_Velocity = 0
	
	local AR_Forward_x,AR_Forward_y,AR_Forward_z = Cal.GetAssignVec(AR_Forward.x,AR_Forward.y,AR_Forward.z,Right_past,Up_past,Forward_past)
	
	if math.abs(AR_Forward_y) < 0.001 then
		AP_Velocity = 0
	else
		AP_Velocity = math.deg(math.atan2(math.sqrt(AR_Forward_x^2 + AR_Forward_z^2),AR_Forward_y))-90
	end
	
	return AP_Velocity
end	

--�}�V���̃��[�p���x�擾(���[�J�����W�)-----
function Get_L_AngleY_Velocity(Right_past,Up_past,Forward_past)

	local AY_Forward = ap:GetForward()
	local AY_Velocity = 0
	
	local AY_Forward_x,AY_Forward_y,AY_Forward_z = Cal.GetAssignVec(AY_Forward.x,AY_Forward.y,AY_Forward.z,Right_past,Up_past,Forward_past)
	
	if math.abs(AY_Forward_x) < 0.001 then
		AY_Velocity = 0
	else
		AY_Velocity = math.deg(math.atan2(AY_Forward_x ,AY_Forward_z))
	end
	
	return AY_Velocity
end	


--�}�V���̃��[���p���x�擾(���[�J�����W�)-----
function Get_L_AngleR_Velocity(Right_past,Up_past,Forward_past)
	
	local AR_Right = ap:GetRight()
	local AR_Velocity = 0
	
	local AR_Right_x,AR_Right_y,AR_Right_z = Cal.GetAssignVec(AR_Right.x,AR_Right.y,AR_Right.z,Right_past,Up_past,Forward_past)
	
	if math.abs(AR_Right_y) < 0.001 then
		AR_Velocity = 0
	else
		AR_Velocity = math.deg(math.atan2(AR_Right_y,math.sqrt(AR_Right_x^2 + AR_Right_z^2)))
	end
	
	return AR_Velocity
end	

--�C�Ӄx�N�g���ɑ΂��郍�[���p���擾(���[�J�����W��A�o�͓͂x)-----
function Get_FreeAngleR(AngleR_Right,AngleR_Up,AngleR_Forward)

	local GF_Right = ap:GetRight()
	local Roll_x,Roll_y,Roll_z = Cal.GetAssignVec(GF_Right.x,GF_Right.y,GF_Right.z,AngleR_Right,AngleR_Up,AngleR_Forward)
	local AngleRoll = -math.deg(math.atan2(Roll_y, math.sqrt(Roll_x^2 + Roll_z^2)))
	
	return AngleRoll
end	

--���σ��[�g�Z�o-----
function Real_late()
	
	if Time.realtimeSinceStartup >= Time_Past + 0.5 then
	
		Frame_dev = Time.frameCount - FrameCount_Past
		Now_late = Frame_dev/(Time.realtimeSinceStartup - Time_Past)
		FrameCount_Past = Time.frameCount
		Time_Past = Time.realtimeSinceStartup
		
	end
	
	return Now_late
end
----------


