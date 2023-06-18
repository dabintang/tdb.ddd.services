
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for authority_info
-- ----------------------------
-- DROP TABLE IF EXISTS `authority_info`;
CREATE TABLE `authority_info`  (
  `ID` bigint NOT NULL COMMENT '权限ID',
  `Name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '权限名称',
  `Remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '备注',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '权限信息' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for role_authority_config
-- ----------------------------
-- DROP TABLE IF EXISTS `role_authority_config`;
CREATE TABLE `role_authority_config`  (
  `RoleID` bigint NOT NULL COMMENT '角色ID',
  `AuthorityID` bigint NOT NULL COMMENT '权限ID',
  PRIMARY KEY (`RoleID`, `AuthorityID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色权限配置' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for role_info
-- ----------------------------
-- DROP TABLE IF EXISTS `role_info`;
CREATE TABLE `role_info`  (
  `ID` bigint NOT NULL COMMENT '角色ID',
  `Name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角色名称',
  `Remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '备注',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色信息' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for user_info
-- ----------------------------
-- DROP TABLE IF EXISTS `user_info`;
CREATE TABLE `user_info`  (
  `ID` bigint NOT NULL COMMENT '用户ID',
  `Name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '姓名',
  `NickName` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '昵称',
  `LoginName` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '登录名',
  `Password` char(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '密码(MD5)',
  `GenderCode` tinyint NOT NULL DEFAULT 0 COMMENT '性别（1：男；2：女；3：未知）',
  `Birthday` datetime NULL DEFAULT NULL COMMENT '生日',
  `HeadImgID` bigint NULL COMMENT '头像图片ID',
  `MobilePhone` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '手机号码',
  `IsMobilePhoneVerified` bit(1) NOT NULL COMMENT '手机号是否已验证',
  `Email` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '电子邮箱',
  `IsEmailVerified` bit(1) NOT NULL COMMENT '电子邮箱是否已验证',
  `StatusCode` tinyint NOT NULL COMMENT '状态（1：激活；2：禁用）',
  `IsDeleted` bit(1) NOT NULL COMMENT '是否已删除',
  `Remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '备注',
  `CreatorID` bigint NOT NULL COMMENT '创建者ID',
  `CreateTime` datetime(3) NOT NULL COMMENT '创建时间',
  `UpdaterID` bigint NOT NULL COMMENT '更新者ID',
  `UpdateTime` datetime(3) NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `Index_User_LoginName`(`LoginName`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户信息' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for user_role_config
-- ----------------------------
-- DROP TABLE IF EXISTS `user_role_config`;
CREATE TABLE `user_role_config`  (
  `UserID` bigint NOT NULL AUTO_INCREMENT COMMENT '用户ID',
  `RoleID` bigint NOT NULL COMMENT '角色ID',
  PRIMARY KEY (`UserID`, `RoleID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '用户角色配置' ROW_FORMAT = DYNAMIC;

SET FOREIGN_KEY_CHECKS = 1;
