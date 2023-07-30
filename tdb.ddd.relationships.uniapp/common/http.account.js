import api from '@/common/http.request';

//调账户服务接口
const apiAccount = {
	//密码登录
	login: async (params) => api.post('/tdb.ddd.account/v1/User/Login', params),
	//获取当前用户信息
	getCurrentUserInfo: async () => api.get('/tdb.ddd.account/v1/User/GetCurrentUserInfo', {}),
}

export default apiAccount
