--ライブラリ読み込み用関数

module("Load", package.seeall)

local ap


--ライブラリ呼び出し-------------------------------------------
require("Lua/HD_Lua/HD_Basiclib/OutputControl")
require("Lua/HD_Lua/HD_Basiclib/MachinCraftInfo")
require("Lua/HD_Lua/HD_Basiclib/InputControl")
require("Lua/HD_Lua/HD_Basiclib/Calculate")
require("Lua/HD_Lua/HD_Basiclib/Attack")
require("Lua/HD_Lua/HD_Basiclib/Draw")
require("Lua/HD_Lua/HD_Basiclib/Search")

require("Lua/HD_Lua/HD_Advancelib/Gyro")

--初期化-----
function Init(_ap)
ap = _ap

Atk.Init(ap)
Draw.Init(ap)
Cal.Init(ap)
Info.Init(ap)
InCon.Init(ap)
Search.Init(ap)

end	




