# Unity 卡牌肉鸽游戏

这是一个基于 Unity 的卡牌战斗游戏原型。目前包含卡牌打出、单位召唤、战斗伤害结算以及指向性法术系统。

## 📅 更新日志 (Changelog)

### [v0.4.0] - 2025-12-18 (当前版本)

- **✨ 新增功能 (Features)**

  - **大地图与探索 (Map & Exploration)**
    - **随机地图生成**：实现了基于层级的节点生成算法 (MapGenerator)，修复了路径死胡同和孤岛节点的问题（双向连通性保证）。
    - **视觉表现**：
      - 实现了可滚动的地图界面，根据层数动态计算容器高度。
      - 实现了节点间的连线绘制 (MapLine)。
      - 实现了基于配置表的节点图标系统（战斗、精英、事件、休息、商店）。
      - 实现了节点呼吸动画及状态颜色区分（已访问、可到达、锁定）。
    - **事件系统 (EventScene)**：
      - 建立了基于 ScriptableObject 的事件配置流程。
      - 实现了事件场景 UI，支持分支选项、回血、扣血及获取卡牌奖励。
      - 修复了事件场景无法加载及字体显示问题。
  - **卡牌构筑系统 (Deck Building)**
    - **玩家收藏库 (PlayerCollection)**：实现了卡牌的库存管理，区分“已拥有卡牌 (Owned)”和“当前携带卡牌 (Current)”。
    - **构筑 UI (DeckBuilderManager):**：
      - 实现了左侧（卡池）与右侧（当前卡组）的交互界面。
      - 支持点击添加/移除卡牌。
      - 新增了“返回地图”按钮并绑定了数据保存逻辑。
    - **获取机制**: 优化了卡牌获取接口，支持通过参数控制是否允许获取重复卡牌（allowDuplicates）。

- **🐛 修复 (Fixes)**

  - **字体修复**：解决了 TextMeshPro 中文显示方块的问题（SDF 动态生成设置）。

- **📦 资源 (Assets)**

  - **全局管理器 (GameManager)**：实现了跨场景单例，负责管理玩家全局血量 (PlayerCurrentHP)、总卡组 (MasterDeck) 以及地图状态的持久化。
  - **双向数据同步:**：
    - **进战斗**：战斗开始时自动从全局读取血量和卡组，并将卡牌自动分拣为“手牌（法术）”和“场上随从（Unit）”。
    - **进构筑**: 进入构筑界面前，自动将 MasterDeck 数据同步至 PlayerCollection。
    - **退构筑**: 离开构筑界面时，自动将玩家调整后的卡组回写至 GameManager，并去重/分类。
    - **战斗结算**: 战斗胜利后，将剩余血量回写至全局，并解锁下一层地图节点。
  - **敌方卡组设计**：现在敌方 AI 会使用法术卡了。
  - 新增卡牌数据：“突袭”。

---

### [v0.3.1] - 2025-12-17

- **✨ 新增功能 (Features)**

  - **攻击模式变更**：从“点击怪兽直接攻击”改为 “先选中我方怪兽 -> 再点击敌方实体” 的二段式操作，为未来多敌人战斗做准备。
  - **敌人实体化**：给敌人 (Enemy) 添加了 UI 交互脚本 (EnemyUnitUI)，现在敌人可以被点击选中了。
  - **召唤限制**：新增规则 “每回合只能召唤一次怪兽”，防止铺场过快。

- **🐛 修复 (Fixes)**

  - **装备牌生命周期修复**：装备卡使用后不再直接进弃牌堆，而是正确地依附在怪兽身上。当怪兽阵亡 (KillUnit) 时，其身上的所有装备牌会自动进入弃牌堆，实现了资源循环。
  - **战斗逻辑修复**：先手怪兽可以战斗/进化后可以重新战斗。
  - **取对象类型卡牌逻辑修复**：“往日种种”回血满血时会被消耗掉/进化时没有装备也可以进化。
  - **不取对象类型卡牌逻辑修复**：“死者苏生”没有目标时会直接消失。

- **📦 资源 (Assets)**

  - **管理器拆分**：BattleManager 进一步瘦身，具体的 UI 交互逻辑（如敌人点击、怪兽点击）分发给了 EnemyUnitUI 和 BattleManager 的新状态机。
  - **DeckManager 增强**：新增 RemoveCardFromHand 方法，支持“移出手牌但不进弃牌堆”的操作（用于装备/召唤）。

---

### [v0.3.0] - 2025-12-16

- **✨ 新增功能 (Features)**

  - 墓地系统：新增 Graveyard 列表，单位阵亡后进入墓地。
  - 墓地查看面板：添加了 UI 界面，点击按钮可查看墓地中的卡牌（只读状态）。

- **🐛 修复 (Fixes)**

  - **生命值机制调整**：装备增加最大生命值时，现在会同步增加等量的当前生命值（起到回血作用）。
  - **UI 刷新修复**：修复了“万具武”进化后，卡面数值（ATK/HP）没有立刻更新的问题。

- **📦 资源 (Assets)**

  - **管理器拆分**：将臃肿的 BattleManager 拆分为 DeckManager (牌堆/手牌)、UnitManager (单位/墓地)、EnemyManager (敌人)、CombatManager (数值计算) 和 UIManager (界面显示)。
  - **运行时数据分离**：引入 RuntimeCard 和 RuntimeUnit 类，将静态配置 (CardData) 与运行时状态（如当前血量、唯一 ID）分离，解决了同名卡数据同步修改的 Bug。
  - **效果系统多态**：移除 switch-case 结构，采用工厂模式 (EffectFactory) 和策略模式 (EffectLogic) 处理卡牌效果，现在新增效果只需添加新类，无需修改管理器代码。

---

### [v0.2.0] - 2025-12-15

**主要更新：战斗系统重构与 UI 修复**

- **✨ 新增功能 (Features)**

  - **指向性法术系统**：新增 `isTargetingMode`，支持使用“往日种种”治疗卡牌点击场上单位进行互动。
  - **动态字体支持**：引入 `msyh SDF` (微软雅黑)，解决中文显示乱码问题。
  - **自动滚动日志**：战斗日志现在使用 `ScrollView`，并会在有新消息时自动滚动到底部。
  - **Layout Element**：给怪兽预制体添加了布局元素，防止在父容器中大小异常。

- **🐛 修复 (Fixes)**

  - 修复了 `FieldUnitUI` 的 Raycast Target 遮挡 Button 点击的问题。
  - 修复了手牌区 (HandPanel) 与备战区 (FieldUnitPanel) 的布局重叠。
  - 修复了 Start 阶段 UI 初始化顺序导致的报错。
  - 解决了 `.gitignore` 未忽略 `.vs` 文件夹的问题。

- **📦 资源 (Assets)**

  - 新增卡牌数据：“往日种种”。

---

### [v0.1.0] - 2025-12-14

- 项目初始化。
- 实现了基础的 BattleManager 回合制逻辑。
- 实现了基础的抽牌逻辑。

## 🎮 如何运行

1. 使用 Unity 6000.2.5f1 打开项目。
2. 打开 `Assets/MainEntry` 场景。
3. 点击 Play 即可开始测试。

## 🛠️ 如何利用现有接口快速制作剧情事件

日期：2025/12/18
模块：Event System (事件系统)

### 1. 核心设计理念

我们的事件系统基于 数据驱动 (Data-Driven)。
这意味着“逻辑”（代码）已经写好了（在 EventSceneManager.cs 和 GameManager.cs 中），你只需要制作“数据文件”（ScriptableObject），游戏就会自动读取并生成对应的 UI 和交互结果。

### 2. 制作步骤 (3 步走)

**第 1 步：创建事件配置文件**

我们不需要写代码，只需要在 Unity 的 Project 窗口中创建一个数据文件。
进入文件夹：Assets/scripts/Data/Event/ (或者任何你存放数据的文件夹)。
右键点击 -> Create -> Data -> Event Profile。
将新文件重命名。
示例命名：Event_CursedStatue (被诅咒的雕像)。

**第 2 步：配置文案与表现**

选中刚才创建的文件，在 Inspector 窗口中填写基础信息：
Event ID: Statue_01 (用于程序内部识别，随意填)。
Title: 被诅咒的雕像 (显示在界面顶部的标题)。
Description: (支持长文本)
你在路边看到一个造型诡异的雕像，雕像的手中握着一把散发着紫光的匕首。你感觉到一股寒意，但同时也感受到了力量的诱惑。
Event Image: 拖入一张 Sprite 图片 (用于显示事件插图)。

**第 3 步：配置选项 (接口核心)**

这是最关键的一步，决定了玩家能做什么。在 Options 列表中点击 + 号添加选项。

✅ 案例 A：设计一个“扣血换卡”的选项
我们想让玩家失去 10 点血，获得一张强力的“诅咒匕首”卡。
Option Text: [拿走匕首] 失去 10 点生命，获得“诅咒匕首”
Result Text: 你抓住了匕首，剧痛传遍全身，但你感觉自己变强了。
Effect: 选择 Damage (扣血效果)。
Value: 10 (扣除的数值)。
Target Card: 拖入你之前做好的 CardData (比如一张攻击力很高的卡)。
原理：后台会自动调用 GameManager.AddCardToDeck 接口，并自动处理去重/允许重复逻辑。

✅ 案例 B：设计一个“回血”选项
Option Text: [向雕像祈祷] 恢复 15 点生命
Result Text: 雕像似乎回应了你的祈祷，暖流涌过全身。
Effect: 选择 Heal。
Value: 15。
Target Card: (留空)。

✅ 案例 C：设计一个“离开”选项
Option Text: [离开] 什么都不做
Result Text: 你决定不招惹这个鬼东西，匆匆离开了。
Effect: 选择 Leave。
Value: 0。
Target Card: (留空)。

### 3. 如何测试新事件？

配置好数据后，有两种方法查看效果：
方法一：快速调试 (推荐)
打开场景 EventScene。
选中 Hierarchy 中的 EventManager 物体。
将你刚刚做好的 Event_CursedStatue 拖入 Inspector 中的 Test Profile 槽位。
点击运行。
结果：场景会直接加载你的文案和按钮，点击按钮会触发对应的 Log 和逻辑。
方法二：实装进游戏
确保你的事件文件放在 Assets/Resources/Events/ 文件夹下（如果使用 Resources.Load）。
或者，在 GameManager 的代码逻辑中（比如生成地图时），将这个 Profile 加入到随机池中。
当前开发阶段：修改 GameManager.SelectNode 方法，将 CurrentEventProfile 指向你的新文件即可测试全流程。

### 4. 接口扩展性说明

如果你需要新的效果（比如“获得金币”或“升级卡牌”），请联系程序进行以下两步简单的扩展，之后你就可以像上面一样使用了：
在 EventProfile.cs 的 EventEffectType 枚举中添加新类型（如 GainGold）。
在 EventSceneManager.cs 的 ApplyEffect 方法中添加对应的 case 逻辑。
总结：只需右键创建 Event Profile，填空，拖拽卡牌数据，即可在 1 分钟内完成一个包含剧情分支、数值变化和卡牌奖励的完整事件。

## 📂 项目文件结构与功能标注

```
Assets/scripts/
│
├── 📂 Data/  (数据定义与持久化)
│   │
│   ├── 📂 Event/
│   │   └── 📄 EventProfile.cs        [配置] 剧情事件的数据模版（标题/文案/选项/奖励）
│   │
│   ├── 📂 Map/
│   │   ├── 📄 MapConfig.cs           [配置] 定义地图层数、节点密度
│   │   ├── 📄 MapData.cs             [数据] 定义节点(MapNode)结构、连线关系
│   │   └── 📄 MapIconsConfig.cs      [配置] 定义不同类型节点对应的 Sprite 图标
│   │
│   ├── 📄 CardData.cs                [配置] 卡牌/随从的基础属性 (ScriptableObject)
│   ├── 📄 PlayerCollection.cs        [单例] 玩家库存管理 (已拥有卡牌 vs 当前构筑卡牌)
│   ├── 📄 RuntimeCard.cs             [逻辑] 战斗中的卡牌实例 (处理消耗/临时状态)
│   └── 📄 RuntimeUnit.cs             [逻辑] 战斗中的单位实例 (处理Buff/实时血量)
│
├── 📂 Effects/  (卡牌效果 - 命令模式)
│   │
│   ├── 📄 EffectBase.cs              [基类] 所有卡牌效果的父类
│   ├── 📄 EffectFactory.cs           [工厂] 根据枚举生成具体效果实例
│   ├── 📄 EffectLogic.cs             [工具] 具体的数值运算逻辑库
│   ├── 📄 DamageEffect.cs            [实现] 造成伤害
│   ├── 📄 HealEffect.cs              [实现] 治疗单位/玩家
│   └── 📄 BuffEffect.cs              [实现] 施加状态效果
│
├── 📂 Managers/  (游戏逻辑控制器)
│   │
│   ├── 📂 Global/  (全局跨场景)
│   │   ├── 📄 GameManager.cs         [核心单例] 总控：血量/总卡组/地图状态/场景切换
│   │   ├── 📄 MapGenerator.cs        [算法] 纯逻辑：生成双向连通的网状地图
│   │   └── 📄 EventSceneManager.cs   [场景控] 剧情场景：解析EventProfile并生成UI
│   │
│   ├── 📄 BattleManager.cs           [场景控] 战斗场景总控：流程/回合/胜负判定
│   ├── 📄 DeckBuilderManager.cs      [场景控] 构筑场景总控：UI交互/数据同步
│   ├── 📄 CombatManager.cs           [逻辑] 伤害结算与攻击流程处理
│   ├── 📄 DeckManager.cs             [逻辑] 战斗内牌堆管理 (抽牌/弃牌/洗牌)
│   ├── 📄 EnemyManager.cs            [逻辑] 敌人生成与AI行为控制
│   ├── 📄 UnitManager.cs             [逻辑] 场上单位的槽位管理与点击交互
│   └── 📄 Unit.cs                    [组件] 挂载于物体：维护血量数值与血条显示
│
└── 📂 UI/  (界面表现与交互)
    │
    ├── 📂 Map/
    │   ├── 📄 MapSceneManager.cs     [生成] 实例化地图节点UI、计算高度、画线
    │   ├── 📄 MapNodeUI.cs           [组件] 单个节点的交互、图标更换、呼吸动画
    │   └── 📄 MapUIController.cs     [交互] 地图界面的辅助按钮 (如打开卡组构筑)
    │
    ├── 📄 BattleUIManager.cs         [组件] 战斗日志(Log)、回合按钮、失败面板控制
    ├── 📄 CardUI.cs                  [组件] 手牌的显示、拖拽、点击事件
    ├── 📄 FieldUnitUI.cs             [组件] 我方随从的头像与状态显示
    ├── 📄 EnemyUnitUI.cs             [组件] 敌方单位的意图显示与点击检测
    └── 📄 StartGameDebug.cs          [调试] 快速启动游戏并初始化全局数据
```
