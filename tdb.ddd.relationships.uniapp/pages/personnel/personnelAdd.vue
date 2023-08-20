<!-- 人员新增页 -->
<template>
    <view class="container">
        <uni-section title="基本信息" type="line">
            <uni-list>
                <uni-list-item title="头像">
                    <template v-slot:footer>
                        <image class="slot-image" :src="headImage" mode="widthFix" @click="chooseImage"></image>
                    </template>
                </uni-list-item>
            </uni-list>
            <view class="baseInfo">
                <uni-forms ref="personnelForm" :rules="rules" :model="personnelInfo" labelWidth="50px">
                    <uni-forms-item label="姓名" required name="Name">
                        <uni-easyinput v-model="personnelInfo.Name" placeholder="请输入姓名" />
                    </uni-forms-item>
                    <uni-forms-item label="性别" required>
                        <uni-data-checkbox v-model="personnelInfo.GenderCode" :localdata="genders" />
                    </uni-forms-item>
                    <uni-forms-item label="生日">
                        <uni-datetime-picker type="date" return-type="string" @change="changeBirthday"
                                             v-model="personnelInfo.Birthday" />
                    </uni-forms-item>
                    <uni-forms-item label="手机">
                        <uni-easyinput v-model="personnelInfo.MobilePhone" placeholder="请输入手机号码" />
                    </uni-forms-item>
                    <uni-forms-item label="邮箱" name="Email">
                        <uni-easyinput v-model="personnelInfo.Email" placeholder="请输入电子邮箱" />
                    </uni-forms-item>
                    <uni-forms-item label="备注" name="Remark">
                        <uni-easyinput type="textarea" v-model="personnelInfo.Remark" placeholder="请输入备注" />
                    </uni-forms-item>
                </uni-forms>
                <view style="text-align: center;">
                    <button @click="submit('personnelForm')" type="warn" plain="true" size="mini" class="form-button">提 交</button>
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
                // 人员基本信息表单数据
                personnelInfo: {
                    Name: '', //[必须]姓名
                    GenderCode: 3, //[必须]性别（1：男；2：女；3：保密）
                    HeadImgID: null, //[可选]头像图片ID
                    Birthday: null, //[可选]生日
                    MobilePhone: '', //[可选]手机号码
                    Email: '', //[可选]电子邮箱
                    Remark: '', //[可选]备注
                },
                // 校验规则
                rules: {
                    Name: {
                        rules: [{
                            required: true,
                            errorMessage: '人员名称不能为空'
                        }]
                    },
                    Email: {
                        rules: [{
                            format: 'email',
                            errorMessage: '电子邮箱格式不正确'
                        }]
                    }
                },
                // 性别选项数据源
                genders: [{
                    text: '男',
                    value: 1
                }, {
                    text: '女',
                    value: 2
                }, {
                    text: '保密',
                    value: 3
                }],
            }
        },
        computed: {
            //人员头像
            headImage() {
                if (this.personnelInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(this.personnelInfo.HeadImgID, 100);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            }
        },
        methods: {
            //选择头像
            async chooseImage() {
                let res = await this.$uniCom.chooseImage();
                if (res) {
                    if (res.tempFilePaths.length > 0) {
                        //上传图片
                        let resUpload = await this.$api.uniUploadTempImg(res);
                        if (resUpload.Code == this.$resCode.success && resUpload.Data.length > 0) {
                            this.personnelInfo.HeadImgID = resUpload.Data[0].ID;
                        }
                    }
                }
            },
            //保存按钮按下
            async submit(ref) {
                //验证输入框
                await this.$refs[ref].validate();
                //新增人员
                let res = await this.$apiPersonnel.addPersonnel(this.personnelInfo);
                if (res.Code == this.$resCode.success) {
                    //页面跳转
                    uni.showToast({
                        title: '保存成功',
                        icon: 'none',
                        complete: () => {
                            //跳转回上一页
                            uni.navigateBack({
                                success: function () {
                                    uni.$emit('refresh.personnel.list'); //刷新人员列表
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
            },
            //生日控件选择事件
            changeBirthday(e) {
                this.personnelInfo.Birthday = e;
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