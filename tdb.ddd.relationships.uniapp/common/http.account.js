import api from '@/common/http.request';

//调账户服务接口
const apiAccount = {
	//密码登录
	login: async (params) => api.post('/tdb.ddd.account/v1/User/Login', params),
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
