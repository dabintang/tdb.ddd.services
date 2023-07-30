import api from '@/common/http.request';
import storage from '@/common/storage.js';

//���ļ�����ӿ�
const apiFiles = {
	//�ϴ�Ϊ��ʱ�ļ�
	uploadTempFiles: async (params) => api.post('/tdb.ddd.files/v1/Files/UploadTempFiles', params),
	//��������ͼƬurl
	downloadImageAnonUrl: (id,width=0) => {
		//api��ַ
		return storage.getApiRoot() + '/tdb.ddd.files/v1/Files/DownloadImageAnon?ID=' + id + '&Width=' + width;
    },
};

export default apiFiles;