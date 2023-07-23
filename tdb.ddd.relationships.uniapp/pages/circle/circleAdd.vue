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
			</view>
		</uni-section>
		<uni-section title="选择成员" type="line">
			<!-- uni-grid -->
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
                    return this.$apiFiles.downloadImageAnonUrl(this.circleInfo.ImageID);
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
                        console.log("resUpload：", typeof resUpload);
                        if (resUpload.Code == this.$resCode.success && resUpload.Data.length > 0) {
                            this.circleInfo.ImageID = resUpload.Data[0].ID;
                            console.log("this.circleInfo.ImageID：", this.circleInfo.ImageID);
                        }
                    }
                }
			},
		}
	}
</script>

<style lang="scss" scoped>
	.baseInfo {
		padding: 15px;
		background-color: #fff;
	}

	.slot-image {
		/* #ifndef APP-NVUE */
		display: block;
		/* #endif */
		margin-right: 10px;
		width: 30px;
		height: 30px;
	}
</style>
