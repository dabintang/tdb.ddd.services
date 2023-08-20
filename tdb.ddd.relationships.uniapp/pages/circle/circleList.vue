<!-- 人际圈列表页 -->
<template>
	<view class="container">
		<uni-list>
			<uni-list-item v-for="(item, index) in LstCircle" :title="item.CircleName" :note="formatMembersCount(item)"
						   showArrow :thumb="showCircleImage(item)" thumb-size="lg" :clickable="true" @click="goDetailPage(item)" />
		</uni-list>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				//人际圈列表
				LstCircle: []
			}
		},
		//挂载到实例上去之后调用
		mounted() {
			//监听刷新人际圈列表事件
			uni.$on('refresh.circle.list', this.getListCircle);

			//获取我加入的人际圈列表
			this.getListCircle();
		},
		//导航栏按钮事件
		onNavigationBarButtonTap(e) {
			//跳到新增人际圈页面
			uni.navigateTo({
				url: '/pages/circle/circleAdd',
				animationType: 'pop-in',
				animationDuration: 300
			});
		},
		//下拉刷新
		async onPullDownRefresh() {
			//获取我加入的人际圈列表
			await this.getListCircle(false);
			//关闭刷新动画
			uni.stopPullDownRefresh();
		},
		methods: {
			//获取我加入的人际圈列表
			async getListCircle(showToast = true) {
				let req = {
					PageNO: 1,
					PageSize: 100000
				}
				//查询我加入了的人际圈列表
				let res = await this.$apiReport.queryMyCircleList(req, showToast);
				this.LstCircle = res.Data;
			},
			//显示人际圈人数
			formatMembersCount(circleInfo) {
				return '人数：' + circleInfo.MembersCount + '/' + circleInfo.MaxMembers;
			},
			//显示人际圈图标
			showCircleImage(circleInfo) {
				if (circleInfo.ImageID) {
					return this.$apiFiles.downloadImageAnonUrl(circleInfo.ImageID, 100);
				} else {
					return '/static/img/circle-default-head.png';
				}
			},
			//跳转到详情页
			goDetailPage(circleInfo) {
				uni.navigateTo({
					url: '/pages/circle/circleDetail?id=' + circleInfo.ID,
					animationType: 'pop-in',
					animationDuration: 300
				});
			}
		}
	}
</script>

<style>
</style>
