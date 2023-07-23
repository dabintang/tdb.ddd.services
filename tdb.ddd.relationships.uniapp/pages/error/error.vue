<!-- 错误页 -->
<template>
	<view class="about">
		<view class="content">
			<view class="qrcode">
				<image src="/static/img/error.png" @longtap="save"></image>
				<text class="tip">服务出了点小问题</text>
			</view>
			<button type="primary" @click="login">重现登录</button>
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

<style>
	page,
	view {
		display: flex;
	}

	page {
		min-height: 100%;
		background-color: #FFFFFF;
	}

	image {
		width: 360rpx;
		height: 360rpx;
	}

	.about {
		flex-direction: column;
		flex: 1;
	}

	.content {
		flex: 1;
		padding: 30rpx;
		flex-direction: column;
		justify-content: center;
	}

	.qrcode {
		display: flex;
		align-items: center;
		flex-direction: column;
	}

	.qrcode .tip {
		margin-top: 20rpx;
	}

	button {
		width: 90%;
		margin-top: 40rpx;
	}
	
</style>
