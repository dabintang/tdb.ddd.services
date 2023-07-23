import App from './App'
import '@/common/http.interceptor.js';
import resCode from '@/common/responseCode.js';
import storage from '@/common/storage.js';
import uniCom from '@/common/uniCom.js';
import api from '@/common/http.request';
import apiAccount from '@/common/http.account.js';
import apiReport from '@/common/http.report.js';
import apiPersonnel from '@/common/http.personnel.js';
import apiCircle from '@/common/http.circle.js';
import apiFiles from '@/common/http.files.js';

import { createSSRApp } from 'vue'
export function createApp() {
  const app = createSSRApp(App)
  
  app.config.globalProperties.$resCode = resCode;
  app.config.globalProperties.$storage = storage;
  app.config.globalProperties.$uniCom = uniCom;
  app.config.globalProperties.$api = api;
  app.config.globalProperties.$apiAccount = apiAccount;
  app.config.globalProperties.$apiReport = apiReport;
  app.config.globalProperties.$apiPersonnel = apiPersonnel;
  app.config.globalProperties.$apiCircle = apiCircle;
  app.config.globalProperties.$apiFiles = apiFiles;

  return {
    app
  }
}
