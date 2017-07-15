--ï`âÊêßå‰

module("Draw", package.seeall)

local ap

--èâä˙âª-----
function Init(_ap)
ap = _ap
end

--ÉeÉXÉgóp-----
function Draw3Dtest(test,myposi)
	
	ap:DrawLine3D(Color(0.5,0.8,1),myposi,test)
	
end



--3DéOäpå`ìäâe-----
function Draw3DTri(center,color,scale)
	
	local D_vt1 = Vector3.zero
	local D_vt2 = Vector3.zero
	local D_vt3 = Vector3.zero
	local D_x = ap:GetCameraRight()
	local D_y = ap:GetCameraUp()
	
	D_y = ap:MulVec(D_y, scale)
	D_vt1 = ap:AddVec(center, D_y)
	
	D_y = ap:MulVec(D_y, -0.5)
	D_x = ap:MulVec(D_x, 0.85 * scale)
	D_vt2 = ap:AddVec(center, ap:AddVec(D_y,D_x))
	
	D_x = ap:MulVec(D_x, -1)
	D_vt3 = ap:AddVec(center, ap:AddVec(D_y,D_x))
		
	ap:DrawLine3D(color,D_vt1,D_vt2)
	ap:DrawLine3D(color,D_vt2,D_vt3)
	ap:DrawLine3D(color,D_vt3,D_vt1)
	
end

--3Déläpå`ìäâe-----
function Draw3DQuad(center,color,scale)
	
	local D_vt1 = Vector3.zero
	local D_vt2 = Vector3.zero
	local D_vt3 = Vector3.zero
	local D_vt4 = Vector3.zero
	local D_x = ap:GetCameraRight()
	local D_x1 = Vector3.zero
	local D_y = ap:GetCameraUp()
	
	D_y = ap:MulVec(D_y, scale)
	D_x = ap:MulVec(D_x, scale)
	D_vt1 = ap:AddVec(center, ap:AddVec(D_y,D_x))
	
	D_x1 = ap:MulVec(D_x, -1)
	D_vt2 = ap:AddVec(center, ap:AddVec(D_y,D_x1))
	
	D_y = ap:MulVec(D_y, -1)
	D_vt3 = ap:AddVec(center, ap:AddVec(D_y,D_x1))
	D_vt4 = ap:AddVec(center, ap:AddVec(D_y,D_x))

		
	ap:DrawLine3D(color,D_vt1,D_vt2)
	ap:DrawLine3D(color,D_vt2,D_vt3)
	ap:DrawLine3D(color,D_vt3,D_vt4)
	ap:DrawLine3D(color,D_vt4,D_vt1)
	
end

--3DÇ–Çµå`ìäâe-----
function Draw3DRhombus(center,color,scale)
	
	local D_vt1 = Vector3.zero
	local D_vt2 = Vector3.zero
	local D_vt3 = Vector3.zero
	local D_vt4 = Vector3.zero
	local D_x = ap:GetCameraRight()
	local D_y = ap:GetCameraUp()
	
	D_y = ap:MulVec(D_y, scale)
	D_vt1 = ap:AddVec(center, D_y)
	
	D_x = ap:MulVec(D_x, scale)
	D_vt2 = ap:AddVec(center, D_x)
	
	D_y = ap:MulVec(D_y, -1)
	D_vt3 = ap:AddVec(center, D_y)
	
	D_x = ap:MulVec(D_x, -1)
	D_vt4 = ap:AddVec(center,D_x)
		
	ap:DrawLine3D(color,D_vt1,D_vt2)
	ap:DrawLine3D(color,D_vt2,D_vt3)
	ap:DrawLine3D(color,D_vt3,D_vt4)
	ap:DrawLine3D(color,D_vt4,D_vt1)
	
end
--ëΩäpå`ï`é -----
function drawPolygon3D(center,normal,radius,n)
	local U = Vector3.zero
	local V = Vector3.zero
	local CosT
	local SinT
	local CosP
	local SinP
	local x = normal.x
	local y = normal.z
	local z = normal.y
	local radian = 2*math.pi/n

	if x ~= 0.0 or y ~= 0.0 then
		CosT = x / math.sqrt(x * x + y * y)
		SinT = y / math.sqrt(x * x + y * y)
		SinP = z / normal.magnitude
		CosP = math.sqrt(1.0 - SinP * SinP)
		U = ArrayToVector3(-SinT, CosT, 0.0)
		V = ArrayToVector3(-SinP * CosT, -SinP * SinT, CosP)
		for i = 1, n do
			local x1 = math.cos( radian * (i-0.5)) *radius
			local z1 = math.sin( radian * (i-0.5)) *radius
			local x2 = math.cos( radian * (i+0.5)) *radius
			local z2 = math.sin( radian * (i+0.5)) *radius
			local vtx1 = ap:AddVec(center,ap:AddVec(ap:MulVec(U, x1), ap:MulVec(V, z1)) )
			local vtx2 = ap:AddVec(center,ap:AddVec(ap:MulVec(U, x2), ap:MulVec(V, z2)) )
			ap:DrawLine3D(vtx1,vtx2)
		
		end
	else
		for i = 1, n do
			local x1 = math.cos( radian * i ) *radius
			local y1 = math.sin( radian * i ) *radius
			local x2 = math.cos( radian * (i+1) ) *radius
			local y2 = math.sin( radian * (i+1) ) *radius
			local vtx1 = ap:AddVec(center,Vector3(x1,0,y1) )
			local vtx2 = ap:AddVec(center,Vector3(x2,0,y2) )
			ap:DrawLine3D(vtx1,vtx2)
		end
	end
end
---------------------------------------


--Ç»ÇÒÇ»ÇÒÇ∑Ç©Ç±ÇÍ--------------
function ArrayToVector3(x,z,y)
	local vector = Vector3.zero
	vector = ap:AddVec(ap:MulVec(Vector3.right,x), ap:MulVec(Vector3.up,y))
	vector = ap:AddVec(vector, ap:MulVec(Vector3.forward,z))
	return vector
end
---------------------------------------


