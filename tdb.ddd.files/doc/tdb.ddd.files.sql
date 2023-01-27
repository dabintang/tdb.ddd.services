
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for file_info
-- ----------------------------
CREATE TABLE `file_info`  (
  `ID` bigint NOT NULL COMMENT '文件ID',
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '文件名（含后缀）',
  `Address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '文件地址(本地路径或url)',
  `StorageTypeCode` tinyint NOT NULL COMMENT '存储类型（1：本地磁盘）',
  `Size` bigint NOT NULL COMMENT '字节数',
  `AccessLevelCode` tinyint NOT NULL COMMENT '访问级别(1：仅创建者；2：授权；3：公开)',
  `FileStatusCode` tinyint NOT NULL COMMENT '文件状态（1：临时文件；2：正式文件）',
  `Remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '备注',
  `CreatorID` bigint NOT NULL COMMENT '创建者ID',
  `CreateTime` datetime(3) NOT NULL COMMENT '创建时间',
  `UpdaterID` bigint NOT NULL COMMENT '更新者ID',
  `UpdateTime` datetime(3) NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '文件信息' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
