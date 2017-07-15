--入力制御

module("InCon", package.seeall)

--初期化-----
function Init(_ap)
ap = _ap
end


--変数宣言-------
local Mouse_time = 0
local Mouse_X1 = 0
local Mouse_X10 = 0
local Mouse_X_count = 0
local Mouse_Y1 = 0
local Mouse_Y10 = 0
local Mouse_Y_count = 0

--マウス入力-----
function Mouse_input()
	
	if Time.time > Mouse_time + 0.03 then
		
		if Input.GetAxis("Mouse X") then
			Mouse_X10 = Mouse_X10 + Input.GetAxis("Mouse X")
			Mouse_X_count = Mouse_X_count + 1
		end
		if Input.GetAxis("Mouse Y") then
			Mouse_Y10 = Mouse_Y10 + Input.GetAxis("Mouse Y")
			Mouse_Y_count = Mouse_Y_count + 1
		end
			
		Mouse_time = Time.time
		Mouse_X1 = 0
		Mouse_Y1 = 0
			
		if Mouse_X_count ~= 0 then
			Mouse_X1 = (Mouse_X10 / Mouse_X_count)
		end
			
		if Mouse_Y_count ~= 0 then
			Mouse_Y1 = (Mouse_Y10 / Mouse_Y_count)
		end
			
		Mouse_X10 = 0
		Mouse_Y10 = 0
		Mouse_X_count = 0
		Mouse_Y_count = 0

	else
		if Input.GetAxis("Mouse X") then
			Mouse_X10 = Mouse_X10 + Input.GetAxis("Mouse X")
			Mouse_X_count = Mouse_X_count + 1
		end
		if Input.GetAxis("Mouse Y") then
			Mouse_Y10 = Mouse_Y10 + Input.GetAxis("Mouse Y")
			Mouse_Y_count = Mouse_Y_count + 1
		end
			
	end
		
	return Mouse_X1,Mouse_Y1
end

--スティック入力-----
function Stick_input()
	
	if Time.time > Mouse_time + 0.03 then
		
		if Input.GetAxis("Mouse X") then
			Mouse_X10 = Mouse_X10 + Input.GetAxis("Mouse X")
			Mouse_X_count = Mouse_X_count + 1
		end
		if Input.GetAxis("Mouse Y") then
			Mouse_Y10 = Mouse_Y10 + Input.GetAxis("Mouse Y")
			Mouse_Y_count = Mouse_Y_count + 1
		end
			
		Mouse_time = Time.time
		Mouse_X1 = 0
		Mouse_Y1 = 0
			
		if Mouse_X_count ~= 0 then
			Mouse_X1 = (Mouse_X10 / Mouse_X_count)
		end
			
		if Mouse_Y_count ~= 0 then
			Mouse_Y1 = (Mouse_Y10 / Mouse_Y_count)
		end
			
		Mouse_X10 = 0
		Mouse_Y10 = 0
		Mouse_X_count = 0
		Mouse_Y_count = 0

	else
		if Input.GetAxis("Mouse X") then
			Mouse_X10 = Mouse_X10 + Input.GetAxis("Mouse X")
			Mouse_X_count = Mouse_X_count + 1
		end
		if Input.GetAxis("Mouse Y") then
			Mouse_Y10 = Mouse_Y10 + Input.GetAxis("Mouse Y")
			Mouse_Y_count = Mouse_Y_count + 1
		end
			
	end
		
	return Mouse_X1,Mouse_Y1
end

--指示ベクトル操作-----
function Target_vector_Control(Target_vector,Right_vector,Up_vector,Mouse_X_input,Mouse_Y_input,Angul_difU_now,Angul_difR_now,Mypos_now)

	local Target_vector_now = Vector3.zero
	local Right_vector_now = Vector3.zero
	local Up_vector_now = Vector3.zero
	local Target_pos_now = Vector3.zero
	local Mouse_X_rad = 0
	local Mouse_Y_rad = 0
	local Up_now = ap:GetUp();
	local Right_now = ap:GetRight();
	local Forward_now = ap:GetForward();
	local Angul_difU = 0
	local Angul_difR = 0
	
	Target_vector_now = Target_vector
	Right_vector_now = Right_vector
	Up_vector_now =	Up_vector
	
	if Mouse_X_input == 0 then
		Mouse_X_rad = 0
	else
		Mouse_X_rad = -math.atan2(Mouse_X_input, 20)
		if math.abs(Angul_difR_now +  math.deg(Mouse_X_rad)) < 50 then
			Target_vector_now = Cal.Vec_rotation_rad(Target_vector_now,Up_now,Mouse_X_rad)
			Right_vector_now = Cal.Vec_rotation_rad(Right_vector_now,Up_now,Mouse_X_rad)
			Up_vector_now = Cal.Vec_rotation_rad(Up_vector_now,Up_now,Mouse_X_rad)
		end
	end
	
	if Mouse_Y_input == 0 then
		Mouse_Y_rad = 0
	else
		Mouse_Y_rad = math.atan2(Mouse_Y_input, 20)
		if math.abs(Angul_difU_now + math.deg(Mouse_Y_rad)) < 50 then
			Target_vector_now = Cal.Vec_rotation_rad(Target_vector_now,Right_now,Mouse_Y_rad)
			Right_vector_now = Cal.Vec_rotation_rad(Right_vector_now,Right_now,Mouse_Y_rad)
			Up_vector_now = Cal.Vec_rotation_rad(Up_vector_now,Right_now,Mouse_Y_rad)
		end
	end
	
	Target_pos_now = ap:AddVec(Mypos_now,ap:MulVec(Target_vector_now,100))
	
	--Draw.Draw3DTri(Target_pos_now ,Color(0.3,0.8,0.1),2)
	
	ap:Aim(Target_pos_now)
	
	
	local Target_vector_x,Target_vector_y,Target_vector_z = Cal.GetLVec(Target_vector_now.x,Target_vector_now.y,Target_vector_now.z)
	
	if math.abs(Target_vector_y) < 0.001 then
		Angul_difU = 0
	else
		Angul_difU = math.deg(math.atan2(math.sqrt(Target_vector_x^2 + Target_vector_z^2),Target_vector_y))-90
	end
	
	if math.abs(Target_vector_x) < 0.001 then
		Angul_difR = 0
	else
		Angul_difR = math.deg(math.atan2(Target_vector_x ,Target_vector_z))
	end
	
	return Target_vector_now,Right_vector_now,Up_vector_now,Angul_difU,Angul_difR
end



