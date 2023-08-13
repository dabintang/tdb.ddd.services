import storage from '@/common/storage.js';
import resCode from '@/common/responseCode.js';
import qs from 'qs';

var isNeedShowLoading = false; //是否需要显示加载框
var delayShowLoading = 300; //延迟300ms显示加载框
uni.addInterceptor('request', {
  invoke(args) {
	//显示遮罩层
		if (args.data.showToast) {
			isNeedShowLoading = true;
			setTimeout(function () {
				//延迟300ms，如果接口还没返回再显示加载框
				if (isNeedShowLoading) {
					uni.showLoading({
						title: '加载中...'
					});
                }
			}, delayShowLoading);
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

		//用户qs处理get方法中的数组参数
	if (args.method == 'GET' && args.data) {
		//args.data = qs.stringify(args.data, { arrayFormat: 'repeat' });
		let newData = qs.stringify(args.data, { arrayFormat: 'repeat' });
		delete args.data;
		args.url = `${args.url}?${newData}`;
    }

	console.log('interceptor-invoke', JSON.stringify(args)); 
  },
  success(args) {
	//console.log('interceptor-success',args)
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
    //console.log('interceptor-fail',err)
	//跳转到错误页
	uni.redirectTo({
		url: '/pages/error/error'
	});
  }, 
  complete(res) {
	  console.log('interceptor-complete', JSON.stringify(res))
	  //隐藏遮罩层
	  isNeedShowLoading = false;
	  uni.hideLoading();
  },
  returnValue(args) {
	  //console.log('interceptor-returnValue',args)
	  if (!(!!args && (typeof args === "object" || typeof args === "function") && typeof args.then === "function")) {
	    return args;
	  }
	  
	  // 只返回 data 字段
	  return new Promise((resolve, reject) => {
		args.then((res) => res.data ? resolve(res.data) : reject(res));
	  });
    }
})
