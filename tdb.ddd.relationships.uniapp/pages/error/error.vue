<!-- 错误页 -->
<template>
	<view class="wrap">
		<view class="content">
			<image class="logo" src="/static/img/error.png" @longtap="save"></image>
			<text class="tip">服务出了点小问题</text>
			<button type="primary" plain="true" @click="login">重新登录</button>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
			}
		},
		onLoad() {
		},
		methods: {
			//登录
			login(e) {
				//跳转到登录页
				uni.redirectTo({
					url: '/pages/login/login'
				});
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
			text-align: center;
	
			.logo {
				display: flex;
				height: 470rpx;
				width: 470rpx;
				margin-top: 150rpx;
				margin-left: auto;
				margin-right: auto;
				margin-bottom: 25rpx;
			}
				
			.tip {
				display: block;
				margin-bottom: 25rpx;
			}
		}
	}
	
</style>
