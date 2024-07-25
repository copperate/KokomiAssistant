# 小心海助手

### 项目介绍

一个集合了米家游戏启动器（当前仅限PC端游戏）与社区功能的应用。
该应用为UWP应用。

### 最新更新(0.4.0 beta)

- 紧急更新内容(2024/07/25 21:30)

  · 由于最近事件，修改了评论列表内的内容最大高度（之前为无限制）。
  
  · 由于最近事件，将“流浪者”等关键词默认加入屏蔽功能，且该功能默认开启。

- 修改了什么

  · 评论最大高度
  
  · 默认主页与界面样式
  
  · 用户页评论样式更新

- 添加了什么

  · 用户页，该页可访问推送、点赞通知、回复通知与私信。
  
  · “未定事件簿”、“大别野”等社区页

- 移除了什么

  · 化海月助手（移动端应用）的链接功能

### 已实现功能

- 游戏启动器

  启动游戏并显示概况。默认展示体力/树脂/开拓力，可在设置中调整。
  ![主页预览](/preview/preview_1.png "主页预览")
  
- 社区浏览器

  浏览社区内的帖子、内容、评论与用户详情。当前因账号登入问题，不支持评论任何帖子或回复任何评论。

> 帖子列表

![列表预览](/preview/preview_2.png "列表预览")

> 帖子详情

![帖子详情预览](/preview/preview_3.png "帖子详情预览")

> 用户界面

![预览](/preview/preview_4.png "预览")

> 搜索页面

![预览](/preview/preview_4_1.png "预览")

- 游戏工具箱

  集合游戏工具面板。
![预览](/preview/preview_5.png "预览")

### 计划内功能

* [ ] 账号登入与切换

  仍在解析登入流程。目前仅能通过写入Cookie登入账号，且仅限一个。
![预览](/preview/preview_6.png "预览")

* [ ] 助手工具箱

  目前仅有抽卡分析可用。
![预览](/preview/preview_7.png "预览")

* [ ] 助手移动版
* [ ] 战绩查询

  同样因登入问题暂时搁置。账号登入后才能查询战绩。

* [ ] 关键词屏蔽功能（new）

  基于用户反馈添加的功能。默认提供一个关键词合集，可根据需要增加或删除。启用后，含该关键词的帖子、评论、分区、用户名称（包括由官方发布的帖子，同时在部分命中关键词下会包括头像）不会在小心海助手的界面内出现；同时，亦无法主动搜索、打开包含关键词的帖子。
![功能预览](/preview/preview10.png "功能预览")
![屏蔽效果](/preview/preview_block.png "屏蔽效果")

### 已下线功能

~~- 别野聊天室~~

因服务器下线，此功能已关闭。
![预览](/preview/preview_8.png "预览")
