--�o�͐���

module("OutCon", package.seeall)

--Unity��`-----------



--PID�o�͎Z�o-----(�Z�o���ʂ��}�C�i�X�̏ꍇ�̏����ɒ���)
function PID(Kp,Ki,Kd,Desired_Val,Now_Val,PID_P_Power,Dev_val_P1,Dev_val_P2)
	
	local Dev_val = Desired_Val - Now_Val

	local Mp = Kp*(Dev_val-Dev_val_P1)
	local Mi = Ki*Dev_val
	local Md = Kd*((Dev_val - Dev_val_P1)-(Dev_val_P1 - Dev_val_P2))
	
	local PID_Power = PID_P_Power + Mp + Mi + Md
	
	return PID_Power,Dev_val,Dev_val_P1
end
---------------------------------------

--�p���X�o�͎��{����i�������Z���[�g����Ɍv�Z�j--------------
function Pulse(Count_pl,Power)
	
	local Late = Info.Real_late()
	Power = Power * Late / 100
	local Pulse_flag = 0
	local interval = 0
	
	if Power >= Late then
		Pulse_flag = 1
	else
		if Power > 0 then
			interval = Mathf.Round(Late/Power)
			if math.fmod(Count_pl, interval) == 0 then
				Pulse_flag = 1
			else
				Pulse_flag = 0
			end
		else
			Pulse_flag = 0
		end
	end
	return Pulse_flag
end
---------------------------------------

--�p���X�o�͎��{����i�J�E���^ Power(��)�j--------------
function Pulse_Count(Count_pl,P_Count_pl,Power)
	
	local Late = Info.Real_late()
	local Pulse_flag = 0
	local Last_Count_pl = P_Count_pl
	Power = Mathf.Round(Late/100 * Power)
	
		if Power > 0 then
			
			if Count_pl < Last_Count_pl + Power then
				Pulse_flag = 0
			else
				Pulse_flag = 1
				Last_Count_pl = Count_pl
			end
		else
			Pulse_flag = 0
		end
		
	return Pulse_flag,Last_Count_pl
end
---------------------------------------


