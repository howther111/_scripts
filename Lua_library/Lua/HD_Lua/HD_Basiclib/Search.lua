--攻撃用関数

module("Search", package.seeall)

local ap



--変数宣言-------

--初期化-----
function Init(_ap)
ap = _ap
end	

--ロックオン保持-----
function Lockon_keep(Lock_Name)
	
	if ap:CheckEnemy() == false then 
	
		local Lockon_Enemy = ap:SearchEnemy()
	
		if Lockon_Enemy ~= Lock_Name then
			ap:ForgetEnemy()
		end
	end
end	
	



