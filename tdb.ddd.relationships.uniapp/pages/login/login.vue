<!-- 登录页 -->
<template>
	<view class="wrap">
		<view class="content">
			<image class="logo" src="/static/img/logo.png"></image>
			<uni-forms ref="loginForm" v-if="!isLoginByFingerprint" :rules="rules" :model="loginFormData" labelWidth="50px">
				<uni-forms-item label="账号" required name="LoginName">
					<uni-easyinput v-model="loginFormData.LoginName" placeholder="请输入账号" />
				</uni-forms-item>
				<uni-forms-item label="密码" required name="Password">
					<uni-easyinput type="password" v-model="loginFormData.Password" placeholder="请输入密码" />
				</uni-forms-item>
			</uni-forms>
			<button type="primary" v-if="!isLoginByFingerprint" plain="true" @click="submit('loginForm')">登录</button>
		</view>
	</view>
</template>

<script>
    import w_md5 from "@/js_sdk/zww-md5/w_md5.js"
	export default {
		data() {
			return {
				// 登录表单数据
				loginFormData: {
					LoginName: '',
					Password: '',
				},
				// 校验规则
				rules: {
					LoginName: {
						rules: [{
							required: true,
							errorMessage: '账号不能为空'
						}]
					},
					Password: {
						rules: [{
							required: true,
							errorMessage: '密码不能为空'
						}]
					}
				},
                isLoginByFingerprint: false //使用指纹登录
			}
		},
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            //是否启用指纹
            this.isLoginByFingerprint = this.$storage.getFingerprint();
			//console.log('this.isLoginByFingerprint', this.isLoginByFingerprint);
		},
		//显示页面时
        onShow() {
            if (this.isLoginByFingerprint) {
                //指纹登录
                this.loginByFingerprint();
            }
        },
		methods: {
			//登录按钮按下
			async submit(ref) {
				//登录输入框
				await this.$refs[ref].validate();
				let req = {
					LoginName: this.loginFormData.LoginName,
                    Password: w_md5.hex_md5_32Upper(this.loginFormData.Password)
				};
				//登录
				let res = await this.$apiAccount.login(req);
				if (res.Code == this.$resCode.success) {
					//保存token
					this.$storage.setToken(res.Data);

                    //删除指纹凭证
                    await this.deleteCertificate();

					//获取当前用户信息
					let resUser = await this.$apiAccount.getCurrentUserInfo();

					//创建我的人员信息
					let res2 = await this.$apiPersonnel.createMyPersonnelInfo();
					if (res2.Code == this.$resCode.success) {
                        resUser.Data.PersonnelID = res2.Data.ID;
                        this.$storage.setCurrentUser(resUser.Data);

						//页面跳转
						uni.showToast({
							title: '登录成功',
							icon: 'none',
							complete: () => {
								setTimeout(() => {
									//跳转到人际圈列表页
									uni.switchTab({
										url: '/pages/circle/circleList'
									});
								}, 300);
							}
						});
					}
				}
			},
            //指纹登录
			async loginByFingerprint() {
				//验证指纹
                let res = await this.$uniCom.startSoterAuthentication(['fingerPrint']);
				if (res.code == 0) {
                    //获取系统信息
					let sysInfo = uni.getSystemInfoSync();
					
                    let req = {
                        CertificateTypeCode: 1,
                        Credentials: sysInfo.deviceId
                    };
					//指纹登录
					let resLogin = await this.$apiAccount.certificateLogin(req);
					//console.log('指纹登录结果：', JSON.stringify(resLogin));
					if (resLogin && resLogin.Code == this.$resCode.success) {
                        //保存token
						this.$storage.setToken(resLogin.Data);

                        //获取当前用户信息
                        let resUser = await this.$apiAccount.getCurrentUserInfo();

                        //创建我的人员信息
                        let res2 = await this.$apiPersonnel.createMyPersonnelInfo();
                        if (res2.Code == this.$resCode.success) {
                            resUser.Data.PersonnelID = res2.Data.ID;
                            this.$storage.setCurrentUser(resUser.Data);

                            //页面跳转
                            uni.showToast({
                                title: '登录成功',
                                icon: 'none',
                                complete: () => {
                                    setTimeout(() => {
                                        //跳转到人际圈列表页
                                        uni.switchTab({
                                            url: '/pages/circle/circleList'
                                        });
                                    }, 300);
                                }
                            });
                        }
                    }
				} else {
                    uni.showToast({
                        title: '指纹验证失败，请用密码登录',
                        icon: 'none',
						complete: () => {
							this.isLoginByFingerprint = false;
                            //移除是否启用指纹
                            this.$storage.removeFingerprint();
                        }
                    });
                }
			},
            //删除指纹凭证
            async deleteCertificate() {
                //获取系统信息
                let sysInfo = this.$uniCom.getSystemInfoSync();

                let req = {
                    CertificateTypeCode: 1,
                    Credentials: sysInfo.deviceId
                };
                //删除凭证
                return await this.$apiAccount.deleteCertificate(req);
            }
		}
	}
</script>

<style lang="scss" scoped>
	.wrap {
		font-size: 28rpx;

		.content {
			width: 600rpx;
			margin: 80rpx auto 0;

			.logo {
				display: flex;
				height: 200rpx;
				width: 200rpx;
				margin-top: 50rpx;
				margin-left: auto;
				margin-right: auto;
				margin-bottom: 50rpx;
			}
		}
	}
</style>
