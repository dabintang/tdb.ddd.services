<template>
	<view class="container">
		<uni-list>
			<uni-list-item v-for="(item, index) in LstCircle" :title="item.CircleName" :note="formatMembersCount(item)" 
				showArrow thumb="/static/img/circle-default-head.png" thumb-size="lg" />
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
		//组件创建
		created() {
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
		onPullDownRefresh() {
			//获取我加入的人际圈列表
			this.getListCircle(false);
			//关闭刷新动画
			uni.stopPullDownRefresh();
		},
		methods: {
			//获取我加入的人际圈列表
			async getListCircle(showToast=true) {
				let req = {
					PageNO: 1,
					PageSize: 100000
				}
				//查询我加入了的人际圈列表
				let res = await this.$apiReport.queryMyCircleList(req,showToast);
				this.LstCircle = res.Data;
			},
			//显示人数
			formatMembersCount(circleInfo) {
				return '人数：'+circleInfo.MembersCount+'/'+circleInfo.MaxMembers;
			},
			// //跳转到详情页
			// goDetailPage(itemDetail) {
			// 	this.$u.route('/pages/password/detail', {
			// 		id: itemDetail.ID
			// 	});
			// },
			// //删除
			// async deleteItem(itemDetail) {
			// 	let req = {ID: itemDetail.ID};
			// 	//删除
			// 	await this.$u.apiPwd.deleteInfo(req);
				
			// 	//刷新页面
			// 	this.getList();
			// }
		}
	}
</script>

<style>
</style>