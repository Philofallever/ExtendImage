EXTEND UI
-----------------------------
Unity UGUI项目扩展插件
<!-- TOC -->

- [ExtendImage(继承自Image)](#extendimage继承自image)
- [ExtendtTexT(继承自Text)](#extendttext继承自text)
- [SymbolText(继承自Text)](#symboltext继承自text)
- [UIParticle](#uiparticle)
- [SuperScrollView](#superscrollview)
- [其他注意事项](#其他注意事项)

<!-- /TOC -->
-----------------------------
#### ExtendImage(继承自Image)
简单地实现了图片置灰、本地化及对`simple`类型图片镜像的功能,在不使用镜像和置灰时和普通图片无异。在不需要以上功能时,建议用普通图片代替。
- 镜像实现：重载了`OnPopulateMesh`网格生成，有镜像时会额外生成网格([代码参考](https://github.com/codingriver/UnityProjectTest/blob/master/MirrorImage/Assets/Script/MirrorImage.cs))。

- 置灰实现：置灰时使用了灰色shader材质。
- 本地化：如果勾选了本地化，在Awake的时候此组件会尝试本地化，且将自己注册到本地化功能类中，当系统语言变化时，再次尝试本地化，在Destroy的时候将自己从本地化功能类反注册掉。
 
组件新增选项说明：
- Localizable:本地化设置
- Grey:Grey设置 
- Mirror:图片是Simple类型时，可选的镜像模式。
  - None:不使用镜像。
  - Horizontal:水平镜像，提供左半侧图片即可。
  - Vertical：竖直镜像，提供下半侧图片即可。

组件新增API:
- property
  + Grey:获取、更改Gey值

#### ExtendtTexT(继承自Text)
简单地实现了本地化，语言变化时替换为对应语言的字体，所有的文本都应该使用ExtendText或SymbolText组件。
- 本地化：同上

#### SymbolText(继承自Text)
表情与超链接的富文本组件([原地址](https://github.com/wuxiongbin/uHyperText)),屏蔽了其他多余功能的解析，主要富文本格式：
- 表情：#表情id，例：#41
- 富文本:#h文本内容{超链接内容}#h，例：这是一个#h超链接{abcd:efgh}#h。超链点击需要添加SymbolTextEvent。
- 本地化：同上

组件新增选项说明：
- LineAlignment:行对齐。一般bottom即可。
- ElementSegment:单词分割设置。一般选None，选default会将一行显示不下的整个英文单词显示在下一行。
- MinLineHeight:最小行高
- isCheckFontY:

__此组件与UGUI的Text不一致，Text的某些字段是不生效的，例如：overflow设置__
#### UIParticle
UI粒子特效组件([原地址](https://github.com/mob-sakai/ParticleEffectForUGUI)),可使用Mask和RectMask2D组件来进行裁剪[插件README](../Runtime/UIParticle/README.md)。

使用步骤：
  - 将粒子特效放到UI上
  - (可选)如果需要裁剪，将材质的Shader设置为`UI/UIAdditive`
  - 添加`UIParticle`组件到粒子特效的根物体。
  - 如果该粒子特效由多个特效组成点击`Fix`
  - 调整`UIParticle`的`Scale`特效到合适的大小

备注：
 - RectMask2D裁剪时，粒子显示仍是3D
 - Mask裁剪时，粒子会被裁剪成片

#### SuperScrollView
各种样式地`ScrollView`,目前使用了`ListView`和`GridView`[插件手册.pdf](../Runtime/SuperScrollView/DocumentV2_4.pdf)
- ListView：竖直或水平排列的列表
- GridView: 固定列数但不固定行数 或 固定行数但不固定列数的格子状列表

ListView组件选项说明：
- ItemPrefabList:列表item预设设置
  - ItemPrefab: 列表item预设
  - ItemPadding:列表item之间的间距
  - XPosOffset：列表item对齐偏移
  - InitCreateCount:列表Item缓存池，初始时实例化的Item数量

- SupportScrollBar:勾选后支持滑动条（所在的ScrollRect要有对应ScrollBar才行
- ItemSnapEnable:勾选后Item在划动后总是和列表中心对齐
- ArrangeType:Item 排列方式

GridView组件选项说明
- ItemPrefabList:同上
- GridFixedType：固定行或固定列
  - RowCount/ColumnCount: 固定的行数/列数
- Padding:Grid布局范围与可见Rect之间的间距
- ItemSize：每个格子的大小
- ItemPading: 格子之间间距
- RecycleDistance：回收距离，保持默认即可
- ItemSnapEnable: 勾选后Item在划动后总是和列表中心对齐
- ArrangeType:Item 排列方式，默认TopLeftToBottomRight即可

#### 其他注意事项
- 对于不响应点击事件的`Image`和`Text`等，去掉`RaycastTarget`勾选，对于响应点击的组合空间，只用其中一个勾选`RaycastTarget`,例如：一个拥有背景图和文本的Btn,通常只用勾选背景图的`RaycastTarget`即可。
- 不需要富文本的`Text`，去掉`RichText`勾选。
- `Text`组件尽量不使用`BestFit`。界面字体大小应该统一成几种字号。
- `完全透明的`图片可勾选`CanvasRenderer`的`CullTransparentMesh`
- 在能使用`RectMask2D`的情况下，使用`RectMask2D`代替`Mask`。区别：`RectMask2D`是用几何尺寸进行遮罩计算，`Mask`使用图片的alpha进行遮罩计算。
