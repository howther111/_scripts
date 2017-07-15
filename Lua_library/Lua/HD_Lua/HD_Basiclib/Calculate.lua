--���Z�⏕�p

module("Cal", package.seeall)

--������-----
function Init(_ap)
ap = _ap
end

--���[�J�����W�ϊ�-----
function GetLVec(wx,wy,wz)

	local x = ap:GetRight();
	local y = ap:GetUp();
	local z = ap:GetForward();

	local lx = x.x * wx + x.y * wy + x.z*wz
	local ly = y.x * wx + y.y * wy + y.z*wz
	local lz = z.x * wx + z.y * wy + z.z*wz
	return lx,ly,lz
end
----------

--�w��x�N�g�����W�ϊ�-----
function GetAssignVec(wx,wy,wz,Assign_x,Assign_y,Assign_z)

	local lx = Assign_x.x * wx + Assign_x.y * wy + Assign_x.z*wz
	local ly = Assign_y.x * wx + Assign_y.y * wy + Assign_y.z*wz
	local lz = Assign_z.x * wx + Assign_z.y * wy + Assign_z.z*wz
	return lx,ly,lz
end
----------

--���[���h���W�ϊ�-----
function GetWVec(lx,ly,lz)

	local W_X = Vector3.up
	local W_Y = Vector3.right
	local W_Z = Vector3.forward

	local wx=W_X.x*lx+W_X.y*ly+W_X.z*lz
	local wy=W_Y.x*lx+W_Y.y*ly+W_Y.z*lz
	local wz=W_Z.x*lx+W_Z.y*ly+W_Z.z*lz
	return wx,wy,wz
end
----------

--�x�N�g����]----------------------------------

function Vec_rotation_rad(r,n,euler) --sin,cos��0�ƂȂ�l�̓��͋֎~(r�����̃x�N�g���An����]���Aeuler�̓��W�A��)

	local a = ap:AddVec(ap:MulVec(r,math.cos(euler)),ap:AddVec(ap:MulVec(n,(Vector3.Dot(n,r)*(1 - math.cos(euler)))) , ap:MulVec(Vector3.Cross(r,n),math.sin(euler))))
	
	return a
end

--�x�N�g����]----------------------------------

function Vec_rotation(r,n,euler) --sin,cos��0�ƂȂ�l�̓��͋֎~(r�����̃x�N�g���An����]���Aeuler�́�)

	local a = ap:AddVec(ap:MulVec(r,math.cos(math.rad(euler))),ap:AddVec(ap:MulVec(n,(Vector3.Dot(n,r)*(1 - math.cos(math.rad(euler))))) , ap:MulVec(Vector3.Cross(r,n),math.sin(math.rad(euler)))))
	
	return a
end

--�l�̐���----------------------------------
function limit(val,min,max)
 return math.max(min,math.min(max,val))
end

