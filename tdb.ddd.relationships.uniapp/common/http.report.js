import api from '@/common/http.request';

//调报表服务接口
const apiReport = {
	 //查询人际圈列表
	 queryCircleList: async (params) => api.get('/tdb.ddd.relationships/v1/Report/QueryCircleList', params),
	 //查询我加入了的人际圈列表
	 queryMyCircleList: async (params,showToast=true) => api.get('/tdb.ddd.relationships/v1/Report/QueryMyCircleList', params,showToast),
	 //查询人际圈内成员信息列表 
	queryCirclePersonnelList: async (params, showToast = true) => api.get('/tdb.ddd.relationships/v1/Report/QueryCirclePersonnelList', params, showToast),
	 //查询我创建的人员信息列表 
	queryMyPersonnelList: async (params, showToast = true) => api.get('/tdb.ddd.relationships/v1/Report/QueryMyPersonnelList', params, showToast),
 };
 
 export default apiReport