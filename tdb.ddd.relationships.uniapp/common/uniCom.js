import permision from "@/common/permission.js";

//封装一些uni的方法
const uniCom = {
	//选择图片
	chooseImage: async (count = 1, sourceType = ['camera', 'album'], sizeType = ['compressed', 'original']) => {
		let promise = new Promise((resolve, rejct) => {
			uni.chooseImage({
				sourceType: sourceType,
				sizeType: sizeType,
				count: count,
				success: async (res) => {
					resolve(res);
				},
				fail: (err) => {
					console.log("选择图片fail: ", err, JSON.stringify(err));
					// #ifdef APP-PLUS
					if (err['code'] && err.code !== 0) {
						uniCom.checkPermission(err.code);
					}
					// #endif
					// #ifdef MP
					if (err.errMsg.indexOf('cancel') !== '-1') {
						return;
					}
					uni.getSetting({
						success: (res) => {
							let authStatus = res.authSetting['scope.album'] && res.authSetting['scope.camera'];
							if (!authStatus) {
								uni.showModal({
									title: '授权失败',
									content: '请在设置界面打开相关权限',
									success: (res) => {
										if (res.confirm) {
											uni.openSetting()
										}
									}
								});
							}
						}
					});
					// #endif
				}
			});
		});

		return promise;
    },
    //检查选择照片的权限
    checkPermission: async (code) => {
		let type = code ? code - 1 : 2;
		let status = permision.isIOS ? await permision.requestIOS(['camera', 'album']) :
			await permision.requestAndroid(type === 0 ? 'android.permission.CAMERA' :
				'android.permission.READ_EXTERNAL_STORAGE');

		if (status === null || status === 1) {
			status = 1;
		} else {
			uni.showModal({
				content: "没有开启权限",
				confirmText: "设置",
				success: function (res) {
					if (res.confirm) {
						permision.gotoAppSetting();
					}
				}
			})
		}

		return status;
	},
	//检查设备是否支持指定的生物认证方式（supportMode支持：指纹识别fingerPrint）
	checkIsSupportSoterAuthentication: async (supportMode) => {
		try {
			let res = await uni.checkIsSupportSoterAuthentication();
			console.log('uni.checkIsSupportSoterAuthentication.success', JSON.stringify(res));

			if (res.supportMode.indexOf(supportMode) >= 0) {
				return true;
			}
			return false;
		} catch (err) {
			console.log('uni.checkIsSupportSoterAuthentication.error', JSON.stringify(err));
			return false;
        }
	},
	//检查是否已录入指定的生物认证方式（checkAuthMode支持：指纹识别fingerPrint）
	checkIsSoterEnrolledInDevice: async (checkAuthMode) => {
		try {
			let res = await uni.checkIsSoterEnrolledInDevice({ checkAuthMode: checkAuthMode });
			console.log('uni.checkIsSoterEnrolledInDevice.success', JSON.stringify(res));

			return res.isEnrolled;
		} catch (err) {
			console.log('uni.checkIsSoterEnrolledInDevice.error', JSON.stringify(err));
			return false;
		}
	},
	//生物认证
	//（入参requestAuthModes数组支持：指纹识别fingerPrint，如：['fingerPrint']）
    //（出参[code:msg]：[0:认证成功]、[1:设备不支持]、[2:未授权]、[3:认证失败]）
	startSoterAuthentication: async (requestAuthModes) => {
		try {
			let res = await uni.startSoterAuthentication({
				requestAuthModes: requestAuthModes,
				challenge: '123456',
				authContent: ''
			});
			console.log('uni.startSoterAuthentication.success', JSON.stringify(res));

			switch (res.errCode) {
				case 0: //0:识别成功
					return { code: 0, msg:'识别成功'};
				case 90001: //90001:本设备不支持生物认证
				case 90003: //90003:请求使用的生物认证方式不支持
					return { code: 1, msg: '设备不支持' };
				case 90002: //90002:用户未授权使用该生物认证接口
				case 90008: //90008:用户取消授权
					return { code: 2, msg: '未授权' };
				case 90004: //90004:未传入challenge或challenge长度过长（最长512字符）
				case 90005: //90005:auth_content长度超过限制（最长42个字符）
				case 90007: //90007:内部错误
				case 90009: //90009:识别失败
				case 90010: //90010:重试次数过多被冻结
				case 90011: //90011:用户未录入所选识别方式
					return { code: 3, msg: '认证失败' };
            }
			return { code: 3, msg: '认证失败' };
		} catch (err) {
			console.log('uni.startSoterAuthentication.error', JSON.stringify(err));
			return { code: 3, msg: '认证失败' };
		}
	},
	//获取系统信息{deviceId:123,...}
	getSystemInfoSync: () => {
		//获取系统信息
		return uni.getSystemInfoSync();
	},
	//扫码（返回值{result:'',...}，如果result有值则是扫码成功，否则扫码失败）
	scanCode: async () => {
		// #ifdef APP-PLUS
		let status = await uniCom.checkScanCodePermission();
		if (status !== 1) {
			return;
		}
		// #endif

		try {
			let res = await uni.scanCode();
			console.log('uni.scanCode.success', JSON.stringify(res));
			return res;
		} catch (err) {
			console.log('uni.scanCode.error', JSON.stringify(err));
			return err;
		}
	},
	//检查扫码权限
	checkScanCodePermission: async () => {
		let status = permision.isIOS ? await permision.requestIOS('camera') : await permision.requestAndroid('android.permission.CAMERA');
		if (status === null || status === 1) {
			status = 1;
		} else {
			uni.showModal({
				content: "需要相机权限",
				confirmText: "设置",
				success: function (res) {
					if (res.confirm) {
						permision.gotoAppSetting();
					}
				}
			})
		}
		return status;
	},
};

export default uniCom;