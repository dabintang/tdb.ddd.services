import api from '@/common/http.request';
import storage from '@/common/storage.js';

//调文件服务接口
const apiFiles = {
	//上传为临时文件
	uploadTempFiles: async (params) => api.post('/tdb.ddd.files/v1/Files/UploadTempFiles', params),
	//匿名下载图片url
	downloadImageAnonUrl: (id,width=0) => {
		//api地址
		return storage.getApiRoot() + '/tdb.ddd.files/v1/Files/DownloadImageAnon?ID=' + id + '&Width=' + width;
    },
};

export default apiFiles;