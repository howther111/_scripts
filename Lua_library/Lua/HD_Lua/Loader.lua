--���C�u�����ǂݍ��ݗp�֐�

module("Load", package.seeall)

local ap


--���C�u�����Ăяo��-------------------------------------------
require("Lua/HD_Lua/HD_Basiclib/OutputControl")
require("Lua/HD_Lua/HD_Basiclib/MachinCraftInfo")
require("Lua/HD_Lua/HD_Basiclib/InputControl")
require("Lua/HD_Lua/HD_Basiclib/Calculate")
require("Lua/HD_Lua/HD_Basiclib/Attack")
require("Lua/HD_Lua/HD_Basiclib/Draw")
require("Lua/HD_Lua/HD_Basiclib/Search")

require("Lua/HD_Lua/HD_Advancelib/Gyro")

--������-----
function Init(_ap)
ap = _ap

Atk.Init(ap)
Draw.Init(ap)
Cal.Init(ap)
Info.Init(ap)
InCon.Init(ap)
Search.Init(ap)

end	




