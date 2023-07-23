import storage from '@/common/storage.js';
import resCode from '@/common/responseCode.js';

uni.addInterceptor('request', {
  invoke(args) {
	//显示遮罩层
	if (args.data.showToast) {
		console.log('显示加载中');
		uni.showLoading({
			title: '加载中...'
		});
	}
	
	//url拼接上api根路径
	if (args.url.indexOf('http://')==-1 && 
		args.url.indexOf('https://')==-1 && 
		args.url.indexOf('ftp://')==-1) {
		args.url = storage.getApiRoot()+args.url;
	}
	
	//token
	let token = storage.getToken();
	if (token && token.AccessToken) {
		if (args.header) {
			args.header.Authorization = 'Bearer ' + token.AccessToken;
		} else {
			args.header = {
				Authorization: 'Bearer ' + token.AccessToken
			}
		}
	}
	
	console.log('interceptor-invoke',args);
  },
  success(args) {
	console.log('interceptor-success',args)
	if (args.statusCode == 401) {
		//弹框提示
		uni.showToast({
		    title: '登录超时',
		    icon: 'none',
			complete: () => {
				setTimeout(() => {
					//跳转到登录页
					uni.redirectTo({
						url: '/pages/login/login'
					});
				}, 2000)
			}
		});
	}
	else if (args.statusCode != 200) {
		//弹框提示
		uni.showToast({
		    title: '服务器忙',
		    icon: 'none',
			complete: () => {
				setTimeout(() => {
					//跳转到登录页
					uni.redirectTo({
						url: '/pages/error/error'
					});
				}, 2000)
			}
		});
	} else if (args.data.Code != resCode.success) {
		//弹框提示
		uni.showToast({
		    title: args.data.Msg,
		    icon: 'none'
		});
	}
  }, 
  fail(err) {
    console.log('interceptor-fail',err)
	//跳转到错误页
	uni.redirectTo({
		url: '/pages/error/error'
	});
  }, 
  complete(res) {
	  console.log('interceptor-complete',res)
	  //隐藏遮罩层
	  uni.hideLoading();
  },
  returnValue(args) {
	  console.log('interceptor-returnValue',args)
	  if (!(!!args && (typeof args === "object" || typeof args === "function") && typeof args.then === "function")) {
	    return args;
	  }
	  
	  // 只返回 data 字段
	  return new Promise((resolve, reject) => {
		args.then((res) => res.data ? resolve(res.data) : reject(res));
	  });
    }
})
