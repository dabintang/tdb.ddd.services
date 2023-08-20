import api from '@/common/http.request';
import resCode from '@/common/responseCode.js';
import storage from '@/common/storage.js';

//调账户服务接口
const apiAccount = {
	//密码登录
	login: async (params) => {
		//密码登录
		let res = await api.post('/tdb.ddd.account/v1/User/Login', params);
		if (res && res.Code == resCode.success) {
			//刷新访问令牌间隔时间（毫秒）
			let intervalTime = (res.Data.AccessTokenValidSeconds - 60)*1000;
			if (intervalTime > 0) {
				setTimeout(async function () {
					let req = {
						RefreshToken: res.Data.RefreshToken
					};
					//刷新用户访问令牌
					let resFefresh = await apiAccount.refreshAccessToken(req);
					if (resFefresh && resFefresh.Code == resCode.success) {
						//保存token
						storage.setToken(resFefresh.Data);
                    }
				}, intervalTime);
            }
        }

		return res;
	},
	//刷新用户访问令牌
	refreshAccessToken: async (params) => {
		let res = await api.post('/tdb.ddd.account/v1/User/RefreshAccessToken', params, false);
		if (res && res.Code == resCode.success) {
			//刷新访问令牌间隔时间（毫秒）
			let intervalTime = (res.Data.AccessTokenValidSeconds - 60) * 1000;
			console.log('setTimeout.intervalTime',intervalTime);
			if (intervalTime > 0) {
				setTimeout(async function () {
					let req = {
						RefreshToken: res.Data.RefreshToken
					};
					//刷新用户访问令牌
					let resFefresh = await apiAccount.refreshAccessToken(req);
					console.log('setTimeout.resFefresh',JSON.stringify(resFefresh));
					if (resFefresh && resFefresh.Code == resCode.success) {
						//保存token
						storage.setToken(resFefresh.Data);
					}
				}, intervalTime);
			}
		}

		return res;
	},
	//获取当前用户信息
	getCurrentUserInfo: async () => api.get('/tdb.ddd.account/v1/User/GetCurrentUserInfo', {}),
	//凭证登录
	certificateLogin: async (params) => api.post('/tdb.ddd.account/v1/Certificate/Login', params),
	//添加凭证
	addCertificate: async (params) => api.post('/tdb.ddd.account/v1/Certificate/AddCertificate', params),
	//删除凭证
	deleteCertificate: async (params) => api.post('/tdb.ddd.account/v1/Certificate/DeleteCertificate', params),
}

export default apiAccount
