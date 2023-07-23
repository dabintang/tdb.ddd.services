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
    }
};

export default uniCom;