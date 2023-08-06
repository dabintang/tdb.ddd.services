<!-- 人际圈新增页 -->
<template>
	<view class="container">
		<uni-section title="基本信息" type="line">
			<uni-list>
				<uni-list-item title="图标">
					<template v-slot:footer>
						<image class="slot-image" :src="circleImage" mode="widthFix" @click="chooseImage"></image>
					</template>
				</uni-list-item>
			</uni-list>
			<view class="baseInfo">
				<uni-forms ref="circleForm" :rules="rules" :model="circleInfo" labelWidth="50px">
					<uni-forms-item label="名称" required name="Name">
						<uni-easyinput v-model="circleInfo.Name" placeholder="请输入人际圈名称" />
					</uni-forms-item>
					<uni-forms-item label="备注" name="Remark">
						<uni-easyinput type="textarea" v-model="circleInfo.Remark" placeholder="请输入备注" />
					</uni-forms-item>
				</uni-forms>
				<view style="text-align: center;">
					<button @click="submit('circleForm')" type="warn" plain="true" size="mini" class="form-button">提 交</button>
					<button @click="cancel" plain="true" size="mini" class="form-button">取 消</button>
				</view>
			</view>
		</uni-section>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				// 人际圈基本信息表单数据
				circleInfo: {
					Name: '', //名称
					ImageID: null, //图标ID
					Remark: '', //备注
				},
				// 校验规则
				rules: {
					Name: {
						rules: [{
							required: true,
							errorMessage: '人际圈名称不能为空'
						}]
					}
				}
			}
		},
		computed: {
			//人际圈图标
			circleImage() {
				if (this.circleInfo.ImageID) {
					return this.$apiFiles.downloadImageAnonUrl(this.circleInfo.ImageID, 40);
				} else {
					return '/static/img/circle-default-head.png';
				}
			}
		},
		methods: {
			//选择图标
			async chooseImage() {
				let res = await this.$uniCom.chooseImage();
				if (res) {
					if (res.tempFilePaths.length > 0) {
						//上传图片
						let resUpload = await this.$api.uniUploadTempImg(res);
						if (resUpload.Code == this.$resCode.success && resUpload.Data.length > 0) {
							this.circleInfo.ImageID = resUpload.Data[0].ID;
						}
					}
				}
			},
			//保存按钮按下
			async submit(ref) {
				//验证输入框
				await this.$refs[ref].validate();
				//新增人际圈
				let res = await this.$apiCircle.addCircle(this.circleInfo);
				if (res.Code == this.$resCode.success) {
					//页面跳转
					uni.showToast({
						title: '保存成功',
						icon: 'none',
						complete: () => {
							//跳转回上一页
							uni.navigateBack({
								success: function () {
									uni.$emit('refresh.circle.list'); //刷新人际圈列表
								}
							});
						}
					});
				}
			},
			//取消按钮按下
			cancel() {
                //跳转回上一页
                uni.navigateBack();
            }
		}
	}
</script>

<style lang="scss" scoped>
	.baseInfo {
		padding: 15px;
	}
	.slot-image {
		/* #ifndef APP-NVUE */
		display: block;
		/* #endif */
		margin: -7px 10px -7px 0px;
		width: 40px;
		height: 40px;
	}
    .form-button {
        margin: 0 5px;
		width: 30%;
    }
</style>
