import api from '@/common/http.request';

//调账户服务接口
const apiAccount = {
	//密码登录
	login: async (params) => api.post('/tdb.ddd.account/v1/User/Login', params)
}

export default apiAccount
