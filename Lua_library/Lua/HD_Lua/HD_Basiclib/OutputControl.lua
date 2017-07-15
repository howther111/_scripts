--出力制御

module("OutCon", package.seeall)

--Unity定義-----------



--PID出力算出-----(算出結果がマイナスの場合の処理に注意)
function PID(Kp,Ki,Kd,Desired_Val,Now_Val,PID_P_Power,Dev_val_P1,Dev_val_P2)
	
	local Dev_val = Desired_Val - Now_Val

	local Mp = Kp*(Dev_val-Dev_val_P1)
	local Mi = Ki*Dev_val
	local Md = Kd*((Dev_val - Dev_val_P1)-(Dev_val_P1 - Dev_val_P2))
	
	local PID_Power = PID_P_Power + Mp + Mi + Md
	
	return PID_Power,Dev_val,Dev_val_P1
end
---------------------------------------

--パルス出力実施判定（物理演算レートを基準に計算）--------------
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

--パルス出力実施判定（カウンタ Power(％)）--------------
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


