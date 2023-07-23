import api from '@/common/http.request';

//调人员服务接口
const apiPersonnel = {
	//获取人员信息
	getPersonnel: async (params) => api.get('/tdb.ddd.relationships/v1/Personnel/GetPersonnel', params),
	//创建我的人员信息
	createMyPersonnelInfo: async (params={}) => api.post('/tdb.ddd.relationships/v1/Personnel/CreateMyPersonnelInfo', params),
	//添加人员
	addPersonnel: async (params={}) => api.post('/tdb.ddd.relationships/v1/Personnel/AddPersonnel', params),
	//更新人员
	updatePersonnel: async (params={}) => api.post('/tdb.ddd.relationships/v1/Personnel/UpdatePersonnel', params),
	//删除人员
	deletePersonnel: async (params={}) => api.post('/tdb.ddd.relationships/v1/Personnel/DeletePersonnel', params),
	//添加人员照片
	addPersonnelPhoto: async (params={}) => api.post('/tdb.ddd.relationships/v1/Personnel/AddPersonnelPhoto', params),
	//设置人员头像照片
	setHeadImg: async (params={}) => api.post('/tdb.ddd.relationships/v1/Personnel/SetHeadImg', params),
};

export default apiPersonnel;
