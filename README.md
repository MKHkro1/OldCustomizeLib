# CustomizeLib（PVZRHCustomization）

> **植物大战僵尸融合版二创植物与僵尸通用前置库（梧萱梦汐X专用分支）**

---

## 一、这是什么

| 项                           | 值                                                       |
| --------------------------- | ------------------------------------------------------- |
| 作者                          | [@Infinite75](https://space.bilibili.com/672619350)     |
| 依托框架                        | [BepInEx](https://github.com/BepInEx/BepInEx) 6（IL2CPP） |
| **适配游戏版本**                  | **4.0**                                                 |
| 目标框架                        | .NET 6（`net6.0`）                                        |
| 程序集名                        | `CustomizeLib.BepInEx`                                  |
| BepInPlugin GUID（暂定，未来可能独立） | `salmon.inf75.pvzcustomization`                         |
| BepInPlugin 名称              | `PVZCustomization`                                      |
| 体量                          | 56 个 `.cs`，12,703 行                                     |

**用途**：给二创模组提供「注册自定义植物 / 僵尸 / 子弹 / 词条 / 关卡 / 皮肤」的统一接口，
并负责把自定义内容接进游戏原生的图鉴、卡片、种子库、合成与伤害管线。

> **授权**：
> 
> 在遵守开源协议的前提下默认授权使用

已构建版本下载可见 [夸克网盘](https://pan.quark.cn/s/958659c83f52)。




---

## 二、⚠ 要 fork 请先看这三条

改名前必须处理，否则**无法与本库共存**：

| #   | 项                                                         | 不改会怎样                           |
| --- | --------------------------------------------------------- | ------------------------------- |
| 1   | **程序集名 / 文件名**（`CustomizeLib.BepInEx` → 你的名字）             | 两份同名程序集在同一个加载上下文里冲突，第二个加载失败或被顶掉 |
| 2   | **`[BepInPlugin]` GUID**（`salmon.inf75.pvzcustomization`） | BepInEx **拒绝加载**第二个同 GUID 的插件   |
| 3   | **命名空间前缀**（`CustomizeLib.BepInEx.*` → 你的）                 | IL2CPP 类型注入按类型全名注册，同名会撞         |

> 另：两库若同时存在，**绝对不要都去钩 `Plant.Update` / `Plant.FixedUpdate` / `SavePlantData..ctor`** ——
> 本库用 dobby 原生钩子占了这三个入口（`Extra\PlantExtra\IPlantEvent\PlantPatch.cs:183` / `:225`、
> `SavePlantDataPatch.cs:43`），dobby 是**替换函数入口**，两库都替换同一地址会互相调回自己 ⇒ **无限递归爆栈闪退**。
> 自己的库需要每帧逻辑，请改用注入组件的 `Update()`（Unity 按方法名原生派发，零 detour）。

---

## 三、源码位置与构建方式

### 3.1 本仓库（`OldCustomizeLib`）是什么

4.0 版的**归档快照**，用于追溯与对照，不是现行开发目录。

### 3.2 现行开发版

```
D:\Web\二创\插件项目Code\前置库\
├── CustomizeLib.BepInEx.csproj
├── Directory.Build.props     ← 本库专用：重建完整引用集 + 存在性守卫
├── CustomCore.cs             ← 主入口（BepInPlugin 在此）
├── Patch\                    ← 53 个 Harmony 补丁类（PatchCore 3201 行 / PatchMgr 879 行）
├── Extra\PlantExtra\IPlantEvent\  ← 每株植物的 Update/FixedUpdate 事件系统（依赖 dobby 钩子）
├── Hook\                     ← LibNativeHook（dobby 封装）
├── Skin\                     ← 皮肤注册
├── ExtensionData\            ← 给 Unity 组件挂自定义数据
└── UnmanagedTools\           ← IL2CPP 反射调用
```

⚠ **刻意不建 `前置库\Directory.Build.targets`** —— 建了会顶掉仓库根目录那份，
而统一构建的产物落盘正靠根目录那份的 `CopyMainDllByMap`。

### 3.3 已并入统一构建

本库是 `D:\Web\二创\插件项目Code\` 统一构建的一个分组（`前置库`），映射到**两个**目标：

| 目标                                                       | 作用                               |
| -------------------------------------------------------- | -------------------------------- |
| `.release\.【置顶】MOD前置文件（注意！需要装里面的前置文件！）\BepInEx\plugins\` | 整包可分发                            |
| `libs插件依赖\`                                              | **编译期引用**（全仓 1000+ 个 csproj 引用它） |

构建命令见 `D:\Web\二创\插件项目Code\agent统一构建skill.md`。
注意：**全量构建（`quick`/`clean`）默认不编前置库**，要连它一起编用 `统一构建.bat prebuild`，
只编它用 `统一构建.bat group "前置库"`。

### 3.4 单独编译

```powershell
# 必须用 pwsh（7.x），不要用 powershell.exe（5.1）
cd D:\Web\二创\插件项目Code\前置库
dotnet build CustomizeLib.BepInEx.csproj -c Release
```

引用集来自**游戏实例的 BepInEx**（`Directory.Build.props` 里的 `GameBepInExPath`，
默认指向 4.0 游戏目录）。该 props 带一层 `<Error>` 守卫：路径不对会**立刻给出可操作报错**，
而不是静默丢引用后表现为上千个 `CS0012`。换机器 / 游戏升版本后记得改 `GameBepInExPath`。

---

## 四、本快照相对上游的本地改动（2026-09-28）

> 归档说明：本仓库的源码**不是**上游原版，含以下本仓库的改动。对照上游请以 Release 页为准。

| #   | 改动                                                                                           | 位置                                               |
| --- | -------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| 1   | `IsMethodTrigger` 按 `MethodInfo` 缓存 `TriggerOnceAttribute` 判定（稳态 0 反射 0 分配）                  | `Extra\PlantExtra\IPlantEvent\IPlantEvent.cs`    |
| 2   | `ExtDataRef.val` 单次取值；`GetCachedComps` 原生调用 6 → 1；`DataComponent.GetData` 改 `TryGetValue`    | `ExtensionData\*`                                |
| 3   | 原生钩子去掉恒真的 `bool notNull = plant != null`                                                     | `Extra\PlantExtra\IPlantEvent\PlantPatch.cs`     |
| 4   | `OnPlantUpdate` / `OnPlantFixedUpdate` 加 try/catch 异常隔离（用户代码异常不再吃掉原生 Update）                 | 同上                                               |
| 5   | 错误日志**限频**（同消息 10 秒一次 + 抑制计数），替代原「加 `board == null` 守卫」方案                                    | `IPlantEvent.cs`                                 |
| 6   | 148 个注册表属性由 `{ get; set; }` 改为 `{ get; }`（移除 149 个 public setter）                            | `CustomCore.cs`                                  |
| 7   | `Patch\PatchCore.cs` 由 GBK 转码为 UTF-8（带 BOM）                                                  | `Patch\PatchCore.cs`                             |
| 8   | `ExtensionDataComponent` 改为转发到 `DataComponent`（两套并行存储合一，公开 API 与类名保留）                        | `ExtensionData\ExtensionData.cs`                 |
| 9   | 3 处 `[HideFromIl2Cpp]`：消除 `Assembly CustomizeLib.BepInEx.dll is not registered in il2cpp` 报错 | `Extra\PlantExtra\IPlantEvent\MouseBehaviour.cs` |
| 10  | `CustomCore.Load()` 首行加一次性自检 `DiagnoseBaseBuffConstraint`，启动时打印 `BaseBuff<T>` 泛型约束的责任方       | `CustomCore.cs`                                  |
| 11  | 引用集存在性守卫（见 §3.4）                                                                             | `Directory.Build.props`                          |

---

## 五、使用与安装

### 5.1 模组侧引用

```xml
<Reference Include="CustomizeLib.BepInEx">
  <HintPath>$(PVZLibsDir)\CustomizeLib.BepInEx.dll</HintPath>
  <Private>True</Private>
</Reference>
```

⚠ **两个静默坑**（会导致"编译通过但用的是旧版前置库"）：

1. 工程目录里残留的 `libs\*.dll` 会**抢先于** `<HintPath>` 命中
   （.NET SDK 默认 `{CandidateAssemblyFiles}` 排在 `{HintPathFromItem}` 之前）；
2. `PackageReference` 提供的同名程序集**权威覆盖** HintPath，不受 `AssemblySearchPaths` 影响。

修法是加：

```xml
<PropertyGroup>
  <AssemblySearchPaths>{HintPathFromItem};{TargetFrameworkDirectory};{RawFileName}</AssemblySearchPaths>
</PropertyGroup>
```

判据用 `dotnet msbuild <csproj> -t:ResolveReferences -getItem:ReferencePath` 看 `Identity` 指向哪
（**别比程序集版本号**，本库新旧版本都是 `1.0.0.0`）。

### 5.2 安装到游戏

1. **先关游戏** —— `BepInEx\plugins\` 里的 DLL 被进程占用时覆盖会失败；
2. 把 `CustomizeLib.BepInEx.dll` 放进 `游戏目录\BepInEx\plugins\`；
3. 启动游戏，日志出现 `PVZCustomization` 加载信息即成功。

---

## 六、注册皮肤

> 以下规则经 4.0 源码核实仍然有效（`Patch\PatchCore.cs:153`、`Patch\PatchMgr.cs:362`）。

1. 把要注册皮肤的植物**预览预制体**和**植物预制体**分别以 `Prefab` 和 `Preview` 命名；
2. 打包为 AssetBundle，放到 `游戏目录\BepInEx\plugins\Skin\` 下（没有 `Skin` 目录请自建）；
3. 文件名重命名为 **`skin_植物ID[_序号]`**：
   - `_序号` 可选，用于给同一株植物注册多个皮肤（任意数字）；
   - 匹配用的正则是 `^skin_(\d+)(?!\d).*$`（忽略大小写）。

> 原作者备注：部分植物注册皮肤可能存在 bug，需要额外代码实现效果；
> 若发现皮肤相关问题，可向 `SkinPatch` / `SkinBehaviours` / `SkinMgr` 等文件提交 PR。

---

## 七、已知不一致与遗留

| 项                       | 说明                                                                                                                                                                   |
| ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **版本号自相矛盾**             | `[BepInPlugin]` 的版本字符串仍写 **`"3.9"`**，而实际适配的是 **4.0**。这是上游遗留，本仓库沿用未改（BepInEx 只用它做显示，不影响功能）                                                                            |
| README 原稿版本号            | 旧版 README 中文段写「适配游戏版本 3.7」、英文段写「4.0」，**已统一为 4.0**（见本次修订记录）                                                                                                           |
| MelonLoader 分支          | 原作者声明「MelonLoader 版本已停止维护（不适配最新版）」；**本快照不含任何 MelonLoader 代码**                                                                                                        |
| `.gitignore` 末行 `/lib/` | 指向旧版外部引用目录 `lib\BepInEx\libs\*.dll`（作者机路径，本机不存在），对当前构建已无意义，保留仅为兼容                                                                                                    |
| `BaseBuff<T>` 泛型约束      | 约束为 `Il2CppSystem.Enum`（在 `Il2Cppmscorlib.dll`），而实际派生类继承 `System.Enum` ⇒ 枚举是密封值类型，**永远无法满足**。全库无一处引用 `BaseBuff<>`。要真修必须改游戏 interop 程序集。`Load()` 首行的一次性自检会在启动日志里写明责任方 |

---

## 八、致谢

融合版制作组：

- [@蓝飘飘fly](https://space.bilibili.com/3546619314178489) — 请在此处下载游戏本体
- [@机鱼吐司](https://space.bilibili.com/85881762)
- [@梦珞呀](https://space.bilibili.com/270840380)
- [@蓝蝶蝶Starryfly](https://space.bilibili.com/27033629)

技术支持：

- [@理科疯子](https://space.bilibili.com/237491236)（Github: [@likefengzi](https://github.com/likefengzi)）
- [@高数带我飞](https://space.bilibili.com/1117414477)（Github: [@LibraHp](https://github.com/LibraHp/)）
- [@鲑鱼-Salmon](https://space.bilibili.com/3493077316536784)（Github: [@Salmon](https://github.com/SalmonCN-RH)）

贴图替换部分使用了 [PvZ-Fusion-Blooms](https://github.com/Dynamixus/PvZ-Fusion-Blooms) 的代码，感谢 Blooms 开发组。

基于 [BepInEx](https://github.com/BepInEx/BepInEx) 开发。

---

## English summary

**PVZ Fusion Customized Plants and Zombies** — a shared prerequisite library (BepInEx 6 / IL2CPP)
providing unified APIs for registering custom plants, zombies, bullets, buffs, levels and skins,
and wiring them into the game's native almanac / card / seed-bank / fusion / damage systems.

- **Game version: 4.0**　·　**.NET 6**　·　assembly `CustomizeLib.BepInEx`
- BepInPlugin GUID `salmon.inf75.pvzcustomization`, name `PVZCustomization`
- This repository is the **4.0 archive snapshot**; the actively developed copy lives at
  `D:\Web\二创\插件项目Code\前置库\` and is wired into the repo's unified build.
- Built binaries: see the upstream [Release](https://github.com/Infinite75/PVZRHCustomization/releases).
- Forks **must** rename the assembly, the BepInPlugin GUID and the root namespace, and must not
  hook `Plant.Update` / `Plant.FixedUpdate` / `SavePlantData..ctor` with dobby (this library already
  occupies those three entry points — two native hooks on the same address cause infinite recursion).

---

## 修订记录

| 日期         | 修订人 | 内容                                                                                                                                                                                                                                                                 |
| ---------- | --- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 2026-09-28 | 本仓库 | **规范化重写**：① 统一版本号（原文中文 3.7 / 英文 4.0 → 全部 4.0）；② 消除中英重复段；③ 新增「fork 前必看」三条改名/避雷清单；④ 新增源码位置、已并入统一构建、单独编译方式；⑤ 新增「本快照相对上游的本地改动」11 项；⑥ 皮肤机制补上核实到的正则；⑦ 新增「已知不一致与遗留」表（版本串 3.9、MelonLoader、`/lib/`、`BaseBuff<T>`）；⑧ 补引用静默坑与判据；⑨ 完整保留原作者署名、授权与致谢名单。**原作者原文均予保留，仅做结构化与事实校正。** |
