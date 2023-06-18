SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for operation_record
-- ----------------------------
--DROP TABLE IF EXISTS `operation_record`;
CREATE TABLE `operation_record`  (
  `ID` bigint NOT NULL COMMENT '记录ID',
  `OperationTypeCode` smallint NOT NULL COMMENT '操作类型编号',
  `Content` varchar(2048) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '操作内容',
  `OperatorID` bigint NOT NULL COMMENT '操作人ID',
  `OperationTime` datetime NOT NULL COMMENT '操作时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `Index_OperationRecord1`(`OperationTime`, `OperationTypeCode`, `OperatorID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;