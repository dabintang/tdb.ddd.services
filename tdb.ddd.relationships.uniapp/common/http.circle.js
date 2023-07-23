import api from '@/common/http.request';

//调人际圈服务接口
const apiCircle = {
	//获取人际圈信息
	getCircle: async (params) => api.get('/tdb.ddd.relationships/v1/Circle/GetCircle', params),
	//创建人际圈
	addCircle: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/AddCircle', params),
	//更新人际圈
	updateCircle: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/UpdateCircle', params),
	//解散人际圈
	deleteCircle: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/DeleteCircle', params),
	//添加成员
	addMember: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/AddMember', params),
	//批量添加成员
	batchAddMember: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/BatchAddMember', params),
	//移出成员
	removeMember: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/RemoveMember', params),
	//设置成员角色
	setMemberRole: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/SetMemberRole', params),
	//生成加入人际圈的邀请码
	createInvitationCode: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/CreateInvitationCode', params),
	//通过邀请码加入人际圈
	joinByInvitationCode: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/JoinByInvitationCode', params),
	//退出人际圈
	withdrawCircle: async (params) => api.post('/tdb.ddd.relationships/v1/Circle/WithdrawCircle', params),
};

export default apiCircle; 
