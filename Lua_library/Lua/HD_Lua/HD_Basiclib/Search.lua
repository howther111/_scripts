--�U���p�֐�

module("Search", package.seeall)

local ap



--�ϐ��錾-------

--������-----
function Init(_ap)
ap = _ap
end	

--���b�N�I���ێ�-----
function Lockon_keep(Lock_Name)
	
	if ap:CheckEnemy() == false then 
	
		local Lockon_Enemy = ap:SearchEnemy()
	
		if Lockon_Enemy ~= Lock_Name then
			ap:ForgetEnemy()
		end
	end
end	
	



