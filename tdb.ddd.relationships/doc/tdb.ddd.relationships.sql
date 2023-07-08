
-- ----------------------------
-- Table structure for circle_info
-- ----------------------------
--DROP TABLE IF EXISTS `circle_info`;
CREATE TABLE `circle_info`  (
  `ID` bigint NOT NULL COMMENT '人际圈ID',
  `Name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '名称',
  `MaxMembers` int NOT NULL COMMENT '成员数上限',
  `Remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '备注',
  `CreatorID` bigint NOT NULL COMMENT '创建者ID',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  `UpdaterID` bigint NOT NULL COMMENT '更新者ID',
  `UpdateTime` datetime NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '人际圈信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for circle_member_info
-- ----------------------------
--DROP TABLE IF EXISTS `circle_member_info`;
CREATE TABLE `circle_member_info`  (
  `ID` bigint NOT NULL COMMENT '人际圈成员ID',
  `CircleID` bigint NOT NULL COMMENT '人际圈ID',
  `PersonnelID` bigint NOT NULL COMMENT '人员ID',
  `RoleCode` smallint NOT NULL COMMENT '角色编码(1：普通成员；2：管理员)',
  `Identity` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '圈内身份',
  `CreatorID` bigint NOT NULL COMMENT '创建者ID',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE INDEX `Index_CircleMember_CircleID_PersonnelID`(`CircleID`, `PersonnelID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '人际圈成员信息表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for personnel_info
-- ----------------------------
--DROP TABLE IF EXISTS `personnel_info`;
CREATE TABLE `personnel_info`  (
  `ID` bigint NOT NULL COMMENT '人员ID',
  `Name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '姓名',
  `GenderCode` tinyint NOT NULL COMMENT '性别（1：男；2：女；3：未知）',
  `HeadImgID` bigint NULL DEFAULT NULL COMMENT '头像图片ID',
  `Birthday` datetime NULL DEFAULT NULL COMMENT '生日',
  `MobilePhone` varchar(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '手机号码',
  `Email` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '电子邮箱',
  `Remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '' COMMENT '备注',
  `UserID` bigint NULL DEFAULT NULL COMMENT '用户账户ID',
  `CreatorID` bigint NOT NULL COMMENT '创建者ID',
  `CreateTime` datetime(3) NOT NULL COMMENT '创建时间',
  `UpdaterID` bigint NOT NULL COMMENT '更新者ID',
  `UpdateTime` datetime(3) NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `Index_Personnel_UserID`(`UserID`) USING BTREE,
  INDEX `Index_Personnel_CreatorID`(`CreatorID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '人员信息表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Table structure for photo_info
-- ----------------------------
--DROP TABLE IF EXISTS `photo_info`;
CREATE TABLE `photo_info`  (
  `ID` bigint NOT NULL COMMENT '照片文件ID',
  `PersonnelID` bigint NOT NULL COMMENT '人员ID',
  `CreatorID` bigint NOT NULL COMMENT '创建者ID',
  `CreateTime` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`ID`) USING BTREE,
  INDEX `Index_Photo_PersonnelID`(`PersonnelID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '照片信息表' ROW_FORMAT = Dynamic;
