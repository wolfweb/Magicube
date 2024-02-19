# magicube 
源自于 magic 和 cube 两个单词的组合 意为：可变化的超级魔方 最初为0代码平台设计
-
magicube具有以下特性 
- 可以插件式开发
- 可以以常规方式开发

## 模块
### 常规模块
* Magicube.AutoMap.AutoMapper
  > 封装automapper 自动映射模块
* Magicube.AutoMap.Mapster
  > 封装mapster 自动映射模块
* Magicube.Core
  > 框架核心模块 扩展di容器 运行时 增强反射 提供统一加密处理等
* Magicube.Data.Abstractions
  > 数据仓储抽象核心 提供有DynamicEntity 动态数据实体 基于此可以实现数据实体数据化 实现0代码平台
* Magicube.Data.EFCore.Abstractions
  > 数据仓储EF的实现
* Magicube.Data.GraphQL
  > 提供GraphQL的扩展 可以对DynamicEntity 动态数据实体实现GraphQL的查询
* Magicube.Data.LiteDb
  > LiteDb的数据仓储实现
* Magicube.Data.Migration
  > 数据实体迁移模块
* Magicube.Data.Mongodb
  > Mongo的数据仓储实现
* Magicube.Data.MySql
  > MySql的数据仓储实现
* Magicube.Data.Neo4j
  > Neo4j的数据仓储实现  Neo4j是一个图数据 可用于数据关系挖掘
* Magicube.Data.PostgreSql
  > PSql的数据仓储实现
* Magicube.Data.Sqlite
  > Sqlite的数据仓储实现
* Magicube.Data.SqlServer
  > SqlServer的数据仓储实现
* Magicube.ElasticSearch
  > ElasticSearch的基础模块
* Magicube.ElasticSearch.Web
  > ElasticSearch的Web应用模块
* Magicube.ElasticSearch7
  > ElasticSearch7的实现模块
* Magicube.Eventbus
  > 事件总线模块
* Magicube.Logging
  > 日志模块
* Magicube.MessageService
  > 消息服务基础模块
* Magicube.MessageService.Kafka
  > 基于kafka的消息服务模块的实现
* Magicube.MessageService.MQTT
  > 基于Mqtt的消息服务模块的实现
* Magicube.MessageService.RabbitMQ
  > 基于RabbitMQ的消息服务模块的实现
* Magicube.MessageService.Redis
  > 基于Redis的消息服务模块的实现
* Magicube.Net
  > 网络请求处理模块和Email模块
* Magicube.Text
  > Unicode字符集处理模块
