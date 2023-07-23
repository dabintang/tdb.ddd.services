//存储
const storage = {
	//保存api根路径
	setApiRoot: (val) => uni.setStorageSync('apiRoot', val),
	//获取api根路径
	getApiRoot: () => {
		let apiRoot = uni.getStorageSync('apiRoot');
		if (!apiRoot) {
			apiRoot = 'http://139.9.64.181:30000';
			storage.setApiRoot(apiRoot);
		}
		return apiRoot;
	},
	
	//保存是否启用指纹
	setFingerprint: (val) => uni.setStorageSync('fingerprint', val),
	//获取是否启用指纹
	getFingerprint: () => { 
		let fingerprint = uni.getStorageSync('fingerprint');
		if (!fingerprint) {
			return false;
		}
		return fingerprint;
	},
	
	//保存token
	setToken: (val) => uni.setStorageSync('token', val),
	//获取token
	getToken: () => { return uni.getStorageSync('token'); },
	//移除token
	removeToken: () => uni.removeStorageSync('token'),
};

export default storage;