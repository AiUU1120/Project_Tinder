# Project_Tinder

## About

A story about tinder and igniting flames.

## Playable Kami

自制基于 Playable 的动画播放框架，可脱离 Animation Controller 使用。动态生成与卸载 Playable 节点。支持动画混合。



<img src="https://s2.loli.net/2024/09/15/4qp6J8umLngfjPG.gif" width="90%">

## Skill Master（Developing）

自制基于 UI Toolkit 的技能编辑器，目前已经开发完成动画、特效、音效部分的编辑器编辑预览与运行时播放。

### Editor

#### 总览

编辑器内采用模拟预览的方式对技能进行编辑，封装动画、判定检测、特效、音效的数据供运行时使用。

<img src="https://s2.loli.net/2024/11/09/7nJqkZpOTCjGlbV.gif" width="90%">

#### 动画

动画轨道采用单轨结构，Editor 内预览由 AnimationClip.SampleAnimation() 函数实现，支持根运动预览（对 Transform 迭代累加计算实现）。

<img src="https://s2.loli.net/2024/11/10/CeK8uEzsRro9x2f.png" width="60%">

> 动画片段选中时的 Inspector 编辑内容

目前存在的问题为部分动画设置下可能发生预览错误，以及 AnimationClip.SampleAnimation() 的局限性导致暂未支持动画混合预览，计划未来手动计算混合以完善此功能。

#### 判定（Developing）

由于判定块可能同时存在多个，采用多轨道结构同时 Tick 所有轨道。支持多种样式（盒型、球型、扇形等）判定块。

Editor 内的预览是 Gizmos 绘制的，同时通过为 SceneView.duringSceneGui 添加了含有 Handles.TransformHandle() 方法的回调，实现了对绘制的判定块进行 Scene 内调整的功能。

<img src="https://s2.loli.net/2024/11/10/mHj7QJW4eZKF6a8.png" width="80%">

<img src="https://s2.loli.net/2024/11/30/NGTa4V6UjoACreS.gif" width="80%">

<img src="https://s2.loli.net/2024/11/10/9gEHtK6IOvJkUCR.png" width="60%">

> 判定片段选中时的 Inspector 编辑内容

#### 特效

多轨道结构。Editor 内使用 ParticleSystem.Simulate() 模拟粒子效果。支持逐帧预览的同时调整特效 GO 的 Transform 以匹配动画。

<img src="https://s2.loli.net/2024/11/10/rFxvwW1ZGhSnYkp.png" width="60%">

> 特效片段选中时的 Inspector 编辑内容
>
> - Calculate Duration Time：自动计算特效片段持续时间，以 Prefab 中最长的 Particle System 为准
> - Apply Effect Model Transform：将场景中的特效 GO 的 Transform 信息计算（转为Local Transform）后应用到片段数据中

发现编辑器内无法预览子发射器的效果，尚不清楚是粒子系统设置不正确（事件的问题？）还是 Unity 的 Bug。

#### 音效

多轨道结构。编辑器内的音频播放通过反射获取 UnityEditor.AudioUtil 中的 PlayPreviewClip() 方法和 StopAllPreviewClips() 方法实现。

### Runtime

运行时下的技能播放是由专门编写的 Skill Player 组件去驱动播放的。主要是读取 Skill Master 编辑器中封装好的技能数据（Scriptable Object），在运行时进行对应的播放。换言之，Skill Master 在编辑器中的代码对于运行时几乎是没有关系的。

<img src="https://s2.loli.net/2024/11/09/n4UeOwCIpoZWxdN.gif" width="90%">
