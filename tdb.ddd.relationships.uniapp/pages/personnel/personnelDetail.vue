<!-- 人员详情页 -->
<template>
    <view class="container">
        <uni-section title="基本信息" type="line">
            <uni-collapse v-model="collapseValue" :accordion="true">
                <uni-collapse-item :title="collapseText" :show-animation="true" :thumb="headImage" name="personnelInfo">
                    <uni-list>
                        <uni-list-item title="头像">
                            <template v-slot:footer>
                                <image class="slot-image" :src="headImage" mode="widthFix" @click="chooseHeadImage"></image>
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
                        <view style="text-align: center;" v-if="isCreator">
                            <button @click="submit('personnelForm')" type="warn" plain="true" size="mini" class="form-button">保 存</button>
                        </view>
                    </view>
                </uni-collapse-item>
            </uni-collapse>
        </uni-section>
        <view style="height:7px;background-color:#F5F5F5;"></view>
        <uni-section title="照片" type="line">
            <view class="grid-dynamic-box">
                <uni-grid :column="4" :show-border="false" @change="gridClick">
                    <uni-grid-item v-for="(item, index) in lstImage" :index="index" :key="index">
                        <view class="grid-item-box">
                            <image :src="showImage(item,85)" class="grid-image" mode="aspectFill" />
                        </view>
                    </uni-grid-item>
                </uni-grid>
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
                    ID: '', //[必须]人员ID
                    Name: '', //[必须]姓名
                    GenderCode: 3, //[必须]性别（1：男；2：女；3：保密）
                    HeadImgID: null, //[可选]头像图片ID
                    Birthday: null, //[可选]生日
                    MobilePhone: '', //[可选]手机号码
                    Email: '', //[可选]电子邮箱
                    Remark: '', //[可选]备注
                    LstPhotoID: [], //[可选]照片ID集合
					CreatorID: '' //创建者用户ID
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
                collapseValue: '', //折叠面板是否展开(personnelInfo)
                isChanged: false, // 是否修改过数据
                //照片信息列表
                lstImage: [],
                //添加照片虚拟信息
                imagePlus: {
                    ImageID: null,
                    ImageUrl: '/static/img/plus.png',
                    TypeCode: 'plus'
                },
				//当前登录用户信息
				curUser: {}
				
            }
        },
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
			//获取当前用户信息
			this.curUser = this.$storage.getCurrentUser();
            //获取人员信息
            this.getPersonnel(option.id);
        },
        //卸载页面时
        onUnload() {
            if (this.isChanged) {
                uni.$emit('refresh.personnel.list'); //刷新人员列表
            }
        },
        computed: {
            //人员头像
            headImage() {
                if (this.personnelInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(this.personnelInfo.HeadImgID, 40);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
            //折叠文字
            collapseText() {
                if (this.collapseValue) {
                    return this.personnelInfo.Name+'（点击收起）';
                }
                return this.personnelInfo.Name +'（点击展开）';
            },
			//当前登录用户是否人员创建者
			isCreator() {
				return this.curUser.ID==this.personnelInfo.CreatorID;
			}
        },
        methods: {
            //选择头像
            async chooseHeadImage() {
                let res = await this.$uniCom.chooseImage();
                if (res) {
                    if (res.tempFilePaths.length > 0) {
                        //上传图片
                        let resUpload = await this.$api.uniUploadTempImg(res);
                        if (resUpload.Code == this.$resCode.success && resUpload.Data.length > 0) {
                            this.personnelInfo.HeadImgID = resUpload.Data[0].ID;
                            //设置人员头像
                            await this.setHeadImg();
                        }
                    }
                }
            },
            //设置人员头像
            async setHeadImg() {
                let req = {
                    PersonnelID: this.personnelInfo.ID,
                    HeadImgID: this.personnelInfo.HeadImgID
                };
                //设置人员头像
                let res = await this.$apiPersonnel.setHeadImg(req);
                if (res.Code == this.$resCode.success) {
                    this.isChanged = true;
                }
                return res;
            },
            //获取人员信息
            async getPersonnel(personnelID) {
                let req = {
                    ID: personnelID
                };
                //获取人员信息
                let res = await this.$apiPersonnel.getPersonnel(req);
                this.personnelInfo = res.Data;

                ////照片信息列表
                let lstImg = [];
                this.personnelInfo.LstPhotoID.forEach(item => {
                    lstImg.push({
                        ImageID: item
                    });
                });
				
				//如果当前登录用户为该人员创建者，显示添加照片的按钮
				if (this.isCreator) {
					lstImg.push(this.imagePlus);
				}
                
                this.lstImage = lstImg;
            },
            //保存按钮按下
            async submit(ref) {
                //验证输入框
                await this.$refs[ref].validate();
                //更新人员
                await this.updatePersonnel();
            },
            //更新人员
            async updatePersonnel() {
                let req = {
                    ID: this.personnelInfo.ID,
                    Name: this.personnelInfo.Name,
                    GenderCode: this.personnelInfo.GenderCode,
                    Birthday: this.personnelInfo.Birthday,
                    MobilePhone: this.personnelInfo.MobilePhone,
                    Email: this.personnelInfo.Email,
                    Remark: this.personnelInfo.Remark
                };
                //更新人员
                let res = await this.$apiPersonnel.updatePersonnel(req);
                if (res.Code == this.$resCode.success) {
                    this.isChanged = true;
                    uni.showToast({
                        title: '保存成功',
                        icon: 'none'
                    });
                }
            },
            //生日控件选择事件
            changeBirthday(e) {
                this.personnelInfo.Birthday = e;
            },
            //显示照片
            showImage(imageInfo, size=0) {
                if (imageInfo.ImageID) {
                    return this.$apiFiles.downloadImageAnonUrl(imageInfo.ImageID, size);
                } else if (imageInfo.ImageUrl) {
                    return imageInfo.ImageUrl;
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
            //点击照片grid
            gridClick(e) {
                let index = e.detail.index;
                let imageInfo = this.lstImage[index];
                if (imageInfo && imageInfo.TypeCode == 'plus') {
                    //选择照片
                    this.chooseImage();
                } else {
                    //预览图片
                    this.previewImage(index);
                }
            },
            //预览图片
            previewImage(imgIndex) {
                if (this.personnelInfo.LstPhotoID.length == 0) {
                    return;
                }

                let imgUrls = [];
                this.personnelInfo.LstPhotoID.forEach(item => {
                    imgUrls.push(this.showImage({ ImageID: item }));
                });

                let _this = this;
                uni.previewImage({
                    urls: imgUrls,
                    current: imgIndex,
                    longPressActions: {
                        itemList: ['删除'],
                        success: async function (data) {
                            //删除人员照片
                            let resDelPhoto = await _this.deletePersonnelPhoto(data.index);
                            if (resDelPhoto && resDelPhoto.Code == _this.$resCode.success) {
                                let newImgIndex = imgIndex;
                                if (newImgIndex >= (_this.lstImage.length - 1)) {
                                    newImgIndex = _this.lstImage.length - 2;
                                }

                                //关闭并重新打开图片预览
                                uni.closePreviewImage({
                                    success: function (res) {
                                        if (newImgIndex >= 0) {
                                            _this.previewImage(newImgIndex);
                                        }
                                    }
                                });
                            }
                        },
                        fail: function (err) {
                            console.log('uni.previewImage.longPressActions.fail', err.errMsg);
                        }
                    }
                });
            },
            //选择照片
            async chooseImage() {
                let res = await this.$uniCom.chooseImage(8);
                if (res) {
                    if (res.tempFilePaths.length > 0) {
                        //上传图片
                        let resUpload = await this.$api.uniUploadTempImg(res);
                        //console.log('uniUploadTempImg', JSON.stringify(resUpload));
                        if (resUpload.Code == this.$resCode.success && resUpload.Data.length > 0) {
                            let lstImageID = [];
                            resUpload.Data.forEach(item => {
                                lstImageID.push(item.ID);
                            });
                            //添加人员照片
                            let resAddPhoto = await this.addPersonnelPhoto(lstImageID);
                            if (resAddPhoto.Code == this.$resCode.success) {
                                //获取人员信息
                                await this.getPersonnel(this.personnelInfo.ID);
                            }
                        }
                    }
                }
            },
            //添加人员照片
            async addPersonnelPhoto(lstImageID) {
                let req = {
                    PersonnelID: this.personnelInfo.ID,
                    LstPhotoID: lstImageID
                };
                //添加人员照片
                return await this.$apiPersonnel.addPersonnelPhoto(req);
            },
            //删除人员照片
            async deletePersonnelPhoto(imgIndex) {
                let imageInfo = this.lstImage[imgIndex];
                if (imageInfo && imageInfo.ImageID) {
                    let req = {
                        PersonnelID: this.personnelInfo.ID,
                        LstPhotoID: [imageInfo.ImageID]
                    };
                    //删除人员照片
                    let res = await this.$apiPersonnel.deletePersonnelPhoto(req);
                    if (res && res.Code == this.$resCode.success) {
                        this.lstImage.splice(imgIndex, 1);
                        this.personnelInfo.LstPhotoID.splice(imgIndex, 1);
                    }
                    return res;
                }
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
        width: 95%;
    }
    .grid-dynamic-box {
        margin-bottom: 15px;
    }
    .grid-item-box {
        flex: 1;
        // position: relative;
        /* #ifndef APP-NVUE */
        display: flex;
        /* #endif */
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 1px;
    }
    .grid-image {
        width: 85px;
        height: 85px;
    }
</style>