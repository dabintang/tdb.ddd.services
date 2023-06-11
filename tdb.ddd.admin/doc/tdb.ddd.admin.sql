
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for operation_record
-- ----------------------------
--DROP TABLE IF EXISTS `operation_record`;
CREATE TABLE `operation_record`  (
  `ID` bigint NOT NULL AUTO_INCREMENT COMMENT '自增ID',
  `OperationTypeCode` smallint NOT NULL COMMENT '操作类型编号',
  `Version` tinyint NOT NULL COMMENT '版本号（同一操作不能版本，内容结构可能不一样）',
  `Content` json NOT NULL COMMENT '操作内容',
  `OperatorID` bigint NOT NULL COMMENT '操作人ID',
  `OperationTime` datetime NOT NULL COMMENT '操作时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `Index_OperationRecord1`(`OperationTime`, `OperationTypeCode`, `OperatorID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
