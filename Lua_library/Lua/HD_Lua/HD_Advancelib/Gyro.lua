--ジャイロ制御

module("Gyro", package.seeall)

--ジャイロ出力算出(入力角応答)-----
function Angle_Gyro_Output(InputAngle,Acceleration,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2,Pulse_count,Reactivity,Braking)
	
	local Inputpower = InputAngle * Reactivity
	local Gyro_Accel = Acceleration * Braking
	
	if  math.abs(Inputpower) < 20 then
		Gyro_Accel = Gyro_Accel/2 * (21 - math.abs(Inputpower))
	end	
	
	
	
	if Inputpower > 0 then
		Inputpower = math.log(Inputpower+1)/math.log(1.1)
	elseif Inputpower < 0 then
		Inputpower = -math.log(math.abs(Inputpower)+1)/math.log(1.1)
	end
	
		Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2 = OutCon.PID(4,0,0,Inputpower,Gyro_Accel,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2)
		--0.001,0.0001
		local PID_Power_lim = Cal.limit(Gy_PID_Power,-100,100)

	
	
	return PID_Power_lim,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2
end
---------------------------------------

--ジャイロ出力算出(リニア応答)-----
function Gyro_Output(Input,Acceleration,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2,Pulse_count,Reactivity)
	
	local braking = 1
	local Inputpower = Input * Reactivity
	
	if Inputpower > 0 then
		if Inputpower < 10 then
			braking = 0
		elseif Inputpower - Acceleration > 1 then
			braking = 0
		end	
	elseif Inputpower < 0 then
		if Inputpower > -10 then
			braking = 0
		elseif Inputpower - Acceleration < -1 then
			braking = 0
		end
	else 
		if math.abs(Acceleration) > 5 then
			Acceleration = Acceleration * Reactivity
		elseif math.abs(Acceleration) > 3 then
			Acceleration = Acceleration * (Reactivity + Reactivity / 5)
		elseif math.abs(Acceleration) > 1 then
			Acceleration = Acceleration * (Reactivity + Reactivity / 2)
		end
	end
	
	if braking == 1 then
		Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2 = OutCon.PID(10,0,0,Inputpower,Acceleration,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2)
	else
		Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2 = OutCon.PID(10,0,0,Inputpower,0,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2)
	end
		--local Pulse_flag = 0
		
		local PID_Power_lim = Cal.limit(Gy_PID_Power,-100,100)
		
		--if PID_Power_lim > 0 then
		--	Pulse_flag = OutCon.Pulse(Pulse_count,PID_Power_lim)
		--elseif PID_Power_lim < 0 then
		--	Pulse_flag = OutCon.Pulse(Pulse_count,-PID_Power_lim)
		--end
	
	
	return PID_Power_lim,Gy_PID_Power,Gy_PID_Dev1,Gy_PID_Dev2
end
---------------------------------------



