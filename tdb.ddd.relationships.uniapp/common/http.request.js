import storage from '@/common/storage.js';

//接口调用简单封装
const api = {
	//post请求
	post: async (url,params,showToast=true) => {
		params.showToast=showToast;
		return uni.request({
					url: url,
					dataType: 'json',
					method: 'POST',
					data: params
				});
	},
	//get请求
	get: async (url,params,showToast=true) => {
		params.showToast=showToast;
		return uni.request({
					url: url,
					dataType: 'json',
					method: 'GET',
					data: params
				});
	},
	//使用uni.uploadFile方法上传临时文件，参数：uni.chooseImage返回值
	uniUploadTempImg: async (chooseImageRes) => {
		//多文件
		let fileDatas = [];
		for (let i = 0; i < chooseImageRes.tempFilePaths.length; i++) {
			fileDatas.push({ name: chooseImageRes.tempFiles[i].name, uri: chooseImageRes.tempFilePaths[i] });
		}

		//api地址
		let apiUrl = storage.getApiRoot() + '/tdb.ddd.files/v1/Files/UploadTempFiles';

		//token
		let token = storage.getToken();
		let authorization = '';
		if (token && token.AccessToken) {
			authorization = 'Bearer ' + token.AccessToken;
		}
		
		//上传图片
		let uploadRes = uni.uploadFile({
					url: apiUrl,
					files: fileDatas,
					formData: {
						'AccessLevelCode': 3
					},
					header: {
						'Authorization': authorization
					}
		});

		// 只返回 data 字段
		return new Promise((resolve, reject) => {
			uploadRes.then((res) => res.data ? resolve(JSON.parse(res.data)) : reject(res));
		});
    }
}

export default api