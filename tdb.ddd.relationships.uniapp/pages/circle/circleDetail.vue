<!-- 人际圈详情页 -->
<template>
    <view class="container">
        <uni-section title="基本信息" type="line">
            <uni-list>
                <uni-list-item title="图标">
                    <template v-slot:footer>
                        <image class="slot-image" :src="circleImage" mode="widthFix" @click="chooseImage"></image>
                    </template>
                </uni-list-item>
                <uni-list-item title="名称" :rightText="circleInfo.Name" showArrow :clickable="true" @click="nameClick" />
                <uni-list-item title="备注" :rightText="circleInfo.Remark" showArrow ellipsis="2" :clickable="true" @click="remarkClick" />
                <uni-list-item title="邀请码" showArrow :clickable="true" @click="invitationCodeClick">
                    <template v-slot:footer>
                        <image class="qrcode-image" src="/static/img/qrcode.png" mode="widthFix"></image>
                    </template>
                </uni-list-item>
            </uni-list>
            <view style="text-align: center;">
                <button type="warn" class="btn" plain="true" size="mini" v-if="isCreator" @click="deleteCircle">解散该圈</button>
                <button type="warn" class="btn" plain="true" size="mini" v-if="!isCreator" @click="withdrawCircle">退出该圈</button>
            </view>
        </uni-section>
        <view style="height:15px;background-color:#F5F5F5;"></view>
        <uni-section title="成员" type="line">
            <view class="grid-dynamic-box">
                <uni-grid :column="4" :show-border="false" @change="change">
                    <uni-grid-item v-for="(item, index) in lstMember" :index="item.PersonnelID" :key="item.PersonnelID">
                        <view class="grid-item-box">
                            <image :src="showHeadImage(item)" class="grid-image" mode="aspectFill" />
                            <text class="text">{{ item.Name }}</text>
                        </view>
                    </uni-grid-item>
                </uni-grid>
            </view>
        </uni-section>

        <!-- 名称输入框 -->
        <view>
            <uni-popup ref="nameDialog" type="dialog">
                <uni-popup-dialog mode="input" title="人际圈名称" :value="circleInfo.Name" placeholder="请输入名称" 
                                  @confirm="updateCircleName" @close="closeNameDialog" :before-close="true"></uni-popup-dialog>
            </uni-popup>
        </view>

        <!-- 备注输入框 -->
        <view>
            <uni-popup ref="remarkDialog" type="dialog">
                <uni-popup-dialog mode="input" title="人际圈备注" :value="circleInfo.Remark"
                                  placeholder="请输入备注" @confirm="updateCircleRemark"></uni-popup-dialog>
            </uni-popup>
        </view>
    </view>
</template>

<script>
    export default {
        data() {
            return {
                // 人际圈基本信息表单数据
                circleInfo: {
                    ID: '', //人际圈ID
                    Name: '', //名称
                    ImageID: null, //图标ID
                    MaxMembers: 0, //成员数上限
                    Remark: '', //备注
                    CreatorID: '' //创建者ID
                },
                lstMember: [], //成员列表
                //添加成员虚拟信息
                memberPlus: {
                    PersonnelID: 0,
                    ImageID: 0,
                    ImageUrl: '/static/img/plus.png',
                    Name: ''
                },
                //减少成员虚拟信息
                memberReduce: {
                    PersonnelID: -1,
                    ImageID: 0,
                    ImageUrl: '/static/img/reduce.png',
                    Name: ''
                },
                // 是否修改过数据
                isChanged: false
            }
        },
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            //获取人际圈信息
            this.getCircle(option.id);
            //查询人际圈内成员信息列表
            this.queryCirclePersonnelList(option.id);
        },
        //卸载页面时
        onUnload() {
            if (this.isChanged) {
                uni.$emit('refresh.circle.list'); //刷新人际圈列表
            }
        },
        computed: {
            //人际圈图标
            circleImage() {
                if (this.circleInfo.ImageID) {
                    return this.$apiFiles.downloadImageAnonUrl(this.circleInfo.ImageID,40);
                } else {
                    return '/static/img/circle-default-head.png';
                }
            },
            //是否人际圈创建者
            isCreator() {
                if (this.circleInfo.CreatorID) {
                    //获取当前用户信息
                    let curUser = this.$storage.getCurrentUser();
                    if (curUser && curUser.ID == this.circleInfo.CreatorID) {
                        return true;
                    }
                }
                return false;
            }
        },
        methods: {
            //获取人际圈信息
            async getCircle(circleID) {
                let req = {
                    ID: circleID
                };
                //获取人际圈信息
                let res = await this.$apiCircle.getCircle(req);
                this.circleInfo = res.Data;
            },
            //选择图标
            async chooseImage() {
                let res = await this.$uniCom.chooseImage();
                if (res) {
                    if (res.tempFilePaths.length > 0) {
                        //上传图片
                        let resUpload = await this.$api.uniUploadTempImg(res);
                        if (resUpload.Code == this.$resCode.success && resUpload.Data.length > 0) {
                            this.circleInfo.ImageID = resUpload.Data[0].ID;
                            //更新人际圈
                            await this.updateCircle();
                        }
                    }
                }
            },
            //点击名称
            nameClick() {
                this.$refs.nameDialog.open();
            },
            //更新人际圈名称
            async updateCircleName(name) {
                if (!name) {
                    uni.showToast({
                        title: '名称不能为空',
                        icon: 'none'
                    });
                    return;
                }

                this.circleInfo.Name = name;
                await this.updateCircle();
                this.$refs.nameDialog.close();
            },
            //关闭名称弹框
            closeNameDialog() {
                this.$refs.nameDialog.close();
            },
            //点击备注
            remarkClick() {
                this.$refs.remarkDialog.open();
            },
            //更新人际圈备注
            async updateCircleRemark(remark) {
                this.circleInfo.Remark = remark;
                await this.updateCircle();
            },
            //更新人际圈
            async updateCircle(showToast = true) {
                let req = {
                    ID: this.circleInfo.ID,
                    Name: this.circleInfo.Name,
                    ImageID: this.circleInfo.ImageID,
                    Remark: this.circleInfo.Remark
                };
                //更新人际圈
                let res = await this.$apiCircle.updateCircle(req, showToast);
                if (res.Code == this.$resCode.success) {
                    this.isChanged = true;
                }

                return res;
            },
            //点击邀请码
            invitationCodeClick() {
                //跳转到邀请码页面
                uni.navigateTo({
                    url: '/pages/circle/qrcodeInvitation?circleID=' + this.circleInfo.ID,
                    animationType: 'pop-in',
                    animationDuration: 300
                });
            },
            //解散人际圈
            async deleteCircle() {
                let resAsk = await uni.showModal({
                    title: '解散确认',
                    content: '你确定要解散此人际圈吗？'
                });
                if (resAsk && resAsk.confirm) {
                    let req = {
                        ID: this.circleInfo.ID
                    };
                    //解散人际圈
                    let res = await this.$apiCircle.deleteCircle(req);
                    if (res.Code == this.$resCode.success) {
                        this.isChanged = true;
                        //页面跳转
                        uni.showToast({
                            title: '解散成功',
                            icon: 'none',
                            complete: () => {
                                //跳转到登录页
                                uni.navigateBack({
                                    success: function () {
                                        uni.$emit('refresh.circle.list'); //刷新人际圈列表
                                    }
                                });
                            }
                        });
                    }
                }
            },
            //查询人际圈内成员信息列表
            async queryCirclePersonnelList(circleID) {
                let req = {
                    CircleID: circleID,
                    PageNO: 1,
                    PageSize: 100000
                };
                //查询人际圈内成员信息列表
                let res = await this.$apiReport.queryCirclePersonnelList(req);
                let list = [];
                res.Data.forEach(item => {
                    list.push({
                        PersonnelID: item.PersonnelID,
                        ImageID: item.ImageID,
                        Name: item.Name
                    });
                });
                list.push(this.memberPlus);
                list.push(this.memberReduce);
                this.lstMember = list;
            },
            //显示成员头像
            showHeadImage(menberInfo) {
                if (menberInfo.ImageID) {
                    return this.$apiFiles.downloadImageAnonUrl(menberInfo.ImageID, 50);
                } else if (menberInfo.ImageUrl) {
                    return menberInfo.ImageUrl;
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
        }
    }
</script>

<style lang="scss" scoped>
    .slot-image {
        /* #ifndef APP-NVUE */
        display: block;
        /* #endif */
        margin-right: 10px;
        margin-top: -8px;
        margin-bottom: -8px;
        width: 40px;
        height: 40px;
    }
    .qrcode-image {
        /* #ifndef APP-NVUE */
        display: block;
        /* #endif */
        margin-right: 5px;
        margin-top: -2px;
        margin-bottom: -7px;
        width: 25px;
        height: 25px;
    }
    .btn {
        margin: 15px;
        width: 90%;
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
        padding: 15px 0;
    }
    .grid-image {
        width: 50px;
        height: 50px;
    }
</style>