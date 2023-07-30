<!-- 登录页 -->
<template>
	<view class="wrap">
		<view class="content">
			<image class="logo" src="/static/img/logo.png"></image>
			<uni-forms ref="loginForm" :rules="rules" :model="loginFormData" labelWidth="50px">
				<uni-forms-item label="账号" required name="LoginName">
					<uni-easyinput v-model="loginFormData.LoginName" placeholder="请输入账号" />
				</uni-forms-item>
				<uni-forms-item label="密码" required name="Password">
					<uni-easyinput type="password" v-model="loginFormData.Password" placeholder="请输入密码" />
				</uni-forms-item>
				<view style="text-align: center;">
					<button type="primary" plain="true" @click="submit('loginForm')">登录</button>
				</view>
			</uni-forms>
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
				}
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

					//获取当前用户信息
					let resUser = await this.$apiAccount.getCurrentUserInfo();
					this.$storage.setCurrentUser(resUser.Data);

					//创建我的人员信息
					let res2 = await this.$apiPersonnel.createMyPersonnelInfo();
					if (res2.Code == this.$resCode.success) {
						//页面跳转
						uni.showToast({
							title: '登录成功',
							icon: 'none',
							complete: () => {
								setTimeout(() => {
									//跳转到登录页
									uni.switchTab({
										url: '/pages/circle/circleList'
									});
								}, 300);
							}
						});
					}
				}
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
