# 标题
Release（可能）每个大版本更新一次，有想弄最新的可以自己编译一下，想发就发   

# PVZRHCustomization
植物大战僵尸融合版二创植物与僵尸 by [@Infinite75](https://space.bilibili.com/672619350)    
适配游戏版本3.7      
已构建版本的链接在Release中    
CustomizeLib和我的模组未来想使用和适配请自便，在遵守开源协议的前提下默认授权

基于[BepInEx](https://github.com/BepInEx/BepInEx)开发      

融合版制作组：    
[@蓝飘飘fly](https://space.bilibili.com/3546619314178489) 请在此处下载游戏本体  
[@机鱼吐司](https://space.bilibili.com/85881762)   
[@梦珞呀](https://space.bilibili.com/270840380)    
[@蓝蝶蝶Starryfly](https://space.bilibili.com/27033629)    

感谢[@理科疯子](https://space.bilibili.com/237491236)(Github:[@likefengzi](https://github.com/likefengzi))的技术支持  
贴图替换部分使用了[PvZ-Fusion-Blooms](https://github.com/Dynamixus/PvZ-Fusion-Blooms)的代码，感谢Blooms开发组      
感谢[@高数带我飞](https://space.bilibili.com/1117414477)(Github:[@LibraHp](https://github.com/LibraHp/))的技术支持    

PVZ Fusion Customized Plants and Zombies    
Game Version: 3.7    
Please download in Release page    

Based on [BepInEx](https://github.com/BepInEx/BepInEx)     

PVZ Fusion Dev Team:     
[@蓝飘飘fly](https://space.bilibili.com/3546619314178489)     
[@机鱼吐司](https://space.bilibili.com/85881762)   
[@梦珞呀](https://space.bilibili.com/270840380)    
[@蓝蝶蝶Starryfly](https://space.bilibili.com/27033629)    


Thanks for [@理科疯子](https://space.bilibili.com/237491236)(Github:[@likefengzi](https://github.com/likefengzi))'s help 
Used codes of [PvZ-Fusion-Blooms](https://github.com/Dynamixus/PvZ-Fusion-Blooms) to replace texures, thanks for Blooms       
Thanks for [@高数带我飞](https://space.bilibili.com/1117414477)(Github:[@LibraHp](https://github.com/LibraHp/))'s help 

### 注册皮肤
将你要注册皮肤的植物的预览预制体和植物预制体分别以"Prefab"和"Preview"命名     
将其打包为AssetBundle包，并放到 游戏目录\BepInEx\plugins\Skin 目录下（如果没有Skin文件夹请自己创建）     
把AssetBundle包重命名为 "skin_plantID[_int]" 最后一部分的_int为可选项，若要为一个植物添加多个皮肤，添加最后一部分的_int来标记不同的皮肤，int为任意数字     

### 安装
从Release中找到 Mod.x.zip解压，选择你要安装的二创解压到你的游戏根目录   

### 部分说明
MelonLoader版本已停止维护（不适配最新版）     
如果我忘记在版本更新时发布Release或Discord，可以帮我发布最新Mod

（部分植物注册皮肤可能存在bug，需要额外代码来实现效果，由于本人没有时间和精力来对其一个个测试，所以如果有人发现皮肤存在bug可以向SkinPatch, SkinBehaviours, SkinMgr等文件添加代码提交PR，感谢您为这个项目做出的贡献）     
