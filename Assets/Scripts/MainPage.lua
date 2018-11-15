MainPage = {}
local this  = MainPage
local gameObject
local transform

print("load MainPage.lua")

function MainPage.Awake(obj)
	gameObject = obj   --得到gameObject对象
	transform = obj.transform

	this.Init()
	this.PrintInfo()
end

function MainPage.Init()
	this.btn_StartGame = transform:Find("btn_StartGame").gameObject
	this.btn_ExitGame = transform:Find("btn_ExitGame").gameObject
end

function MainPage.PrintInfo()
	print("btn_StartGame is : "..this.btn_StartGame)
	print("btn_ExitGame is : "..this.btn_ExitGame)
end