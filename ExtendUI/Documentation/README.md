EXTEND UI
-----------------------------
Unity UGUI项目扩展插件
<!-- TOC -->

- [ExtendImage](#extendimage)
- [ExtendtTexT](#extendttext)
- [SymbolText](#symboltext)
- [UIParticle](#uiparticle)
- [SuperScrollView](#superscrollview)

<!-- /TOC -->
-----------------------------
#### ExtendImage
简单地实现了图片置灰及对`simple`类型图片镜像的功能,在不使用镜像和置灰时和普通图片无异。
- 镜像实现：重载了`OnPopulateMesh`网格生成，有镜像时会额外生成网格([代码参考](https://github.com/codingriver/UnityProjectTest/blob/master/MirrorImage/Assets/Script/MirrorImage.cs))。

- 置灰实现：置灰时使用了灰色shader材质。
- 本地化：TODO

#### ExtendtTexT
简单地实现了本地化，语言变化时替换为对应语言的字体。

#### SymbolText
表情与超链接的富文本组件([原地址](https://github.com/wuxiongbin/uHyperText)),屏蔽了其他多余功能的解析，主要富文本格式：
- 表情：#表情id，例：#41
- 富文本:#h文本内容{超链接内容}#h，例：这是一个#h超链接{abcd:efgh}#h

此组件与UGUI的Text不一致，Text的某些字段是不生效的，例如：overflow设置
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
各种样式地`ScrollView`,目前应该只用到`ListView`[插件手册.pdf](../Runtime/SuperScrollView/DocumentV2_4.pdf)
