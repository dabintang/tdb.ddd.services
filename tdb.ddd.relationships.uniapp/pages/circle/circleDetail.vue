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
        <view style="height:7px;background-color:#F5F5F5;"></view>
        <uni-section title="成员" type="line">
            <view class="grid-dynamic-box">
                <uni-grid :column="4" :show-border="false" @change="gridClick">
                    <uni-grid-item v-for="(item, index) in lstMember" :index="index" :key="index">
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
    import Enumerable from "linq";
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
                    Name: '',
                    HeadImgID: null,
                    ImageUrl: '/static/img/plus.png',
                    TypeCode: 'plus'
                },
                //减少成员虚拟信息
                memberReduce: {
                    PersonnelID: 0,
                    Name: '',
                    HeadImgID: null,
                    ImageUrl: '/static/img/reduce.png',
                    TypeCode: 'reduce'
                },
                //当前登录用户信息
                curUser: {
                    ID: '', //用户ID
                    Name: '', //姓名
                    HeadImgID: null, //头像ID
                    PersonnelID: '', //人员ID
                },
                // 是否修改过数据
                isChanged: false
            }
        },
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            //监听批量添加成员事件
            uni.$on('batch.add.circle.member', (lstSelectedID) => { this.batchAddMember(lstSelectedID) });
            //监听批量移除成员事件
            uni.$on('batch.remove.circle.member', (lstSelectedID) => { this.batchRemoveMember(lstSelectedID) });
            //设置成员角色
            uni.$on('set.member.role', (e) => {
                if (this.circleInfo.ID != e.CircleID) {
                    return;
                }

                this.lstMember.forEach(item => {
                    if (item.PersonnelID == e.PersonnelID) {
                        item.RoleCode = e.RoleCode;
                    }
                });
            });

            //获取当前用户信息
            this.curUser = this.$storage.getCurrentUser();
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
                    return this.$apiFiles.downloadImageAnonUrl(this.circleInfo.ImageID,100);
                } else {
                    return '/static/img/circle-default-head.png';
                }
            },
            //是否人际圈创建者
            isCreator() {
                if (this.circleInfo.CreatorID) {
                    //获取当前用户信息
                    if (this.curUser && this.curUser.ID == this.circleInfo.CreatorID) {
                        return true;
                    }
                }
                return false;
            },
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
            //退出该人际圈
            async withdrawCircle() {
                let resAsk = await uni.showModal({
                    title: '退出确认',
                    content: '你确定要退出该人际圈吗？'
                });
                if (resAsk && resAsk.confirm) {
                    let req = {
                        CircleID: this.circleInfo.ID
                    };
                    //解散人际圈
                    let res = await this.$apiCircle.withdrawCircle(req);
                    if (res.Code == this.$resCode.success) {
                        this.isChanged = true;
                        //页面跳转
                        uni.showToast({
                            title: '退出成功',
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
                let isAdmin = false; //当前登录用户是否人际圈的管理员
                res.Data.forEach(item => {
                    list.push({
                        PersonnelID: item.PersonnelID,
                        HeadImgID: item.HeadImgID,
                        Name: item.Name,
                        PersonnelCreatorID: item.PersonnelCreatorID,
                        RoleCode: item.RoleCode
                    });
                    if (item.PersonnelID == this.curUser.PersonnelID && item.RoleCode == 2) {
                        isAdmin = true;
                    }
                });

				//如果当前登录用户为人际圈管理员，显示加减成员的按钮
                if (isAdmin) {
					list.push(this.memberPlus);
					list.push(this.memberReduce);
				}
               
                this.lstMember = list;
            },
            //显示成员头像
            showHeadImage(menberInfo) {
                if (menberInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(menberInfo.HeadImgID, 100);
                } else if (menberInfo.ImageUrl) {
                    return menberInfo.ImageUrl;
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
            //点击成员grid
            gridClick(e) {
                let index = e.detail.index;
                let memberInfo = this.lstMember[index];
                if (memberInfo) {
                    if (memberInfo.TypeCode == 'plus') {
                        //显示添加成员选择页面
                        this.showPlusMemberPage();
                    } else if (memberInfo.TypeCode == 'reduce') {
                        //显示移除成员选择页面
                        this.showReduceMemberPage();
                    } else {
                        //跳转到成员详情页面
                        uni.navigateTo({
                            url: '/pages/circle/memberDetail?id=' + memberInfo.PersonnelID + '&circleID=' + this.circleInfo.ID + '&roleCode=' + memberInfo.RoleCode + '&isCreator=' + this.isCreator,
                            animationType: 'pop-in',
                            animationDuration: 300
                        });
                    }
                }
            },
            //显示添加成员选择页面
            async showPlusMemberPage() {
                let req = {
                    PageNO: 1,
                    PageSize: 100000
                }
                //查询我创建的人员信息列表
                let res = await this.$apiReport.queryMyPersonnelList(req);
                let list = [];
                if (res.Code == this.$resCode.success) {
                    res.Data.forEach(item => {
                        let arr = Enumerable.from(this.lstMember).where(function (x) { return x.PersonnelID == item.ID }).toArray();
                        if (!arr || arr.length <= 0) {
                            list.push({
                                PersonnelID: item.ID,
                                HeadImgID: item.HeadImgID,
                                Name: item.Name
                            });
                        }
                    });
                }

                //缓存可选人员列表
                this.$storage.setPersonnelSelectList(list);

                //跳转到成员选择页面
                uni.navigateTo({
                    url: '/pages/personnel/personnelSelectList?action=batch.add.circle.member',
                    animationType: 'pop-in',
                    animationDuration: 300
                });
            },
            //批量添加成员
            async batchAddMember(lstSelectedID) {
                if (!lstSelectedID || lstSelectedID.length == 0) {
                    return;
                }

                let req = {
                    CircleID: this.circleInfo.ID,
                    LstPersonnelID: lstSelectedID
                };
                //批量添加成员
                let res = await this.$apiCircle.batchAddMember(req);
                if (res && res.Code == this.$resCode.success) {
                    //查询人际圈内成员信息列表
                    await this.queryCirclePersonnelList(this.circleInfo.ID);
					this.isChanged = true;
                }
            },
            //显示移除成员选择页面
            showReduceMemberPage() {
                let list = [];
                this.lstMember.forEach(item => {
                    console.log('item', JSON.stringify(item));
                    if (item.PersonnelID && item.PersonnelID != this.curUser.PersonnelID && item.PersonnelCreatorID == this.curUser.ID) {
                        list.push({
                            PersonnelID: item.PersonnelID,
                            HeadImgID: item.HeadImgID,
                            Name: item.Name
                        });
                    }
                });

                //缓存可选人员列表
                this.$storage.setPersonnelSelectList(list);

                //跳转到成员选择页面
                uni.navigateTo({
                    url: '/pages/personnel/personnelSelectList?action=batch.remove.circle.member',
                    animationType: 'pop-in',
                    animationDuration: 300
                });
            },
            //批量移除成员
            async batchRemoveMember(lstSelectedID) {
                //console.log('batchRemoveMember', JSON.stringify(lstSelectedID));
                if (!lstSelectedID || lstSelectedID.length == 0) {
                    return;
                }

                let req = {
                   CircleID: this.circleInfo.ID,
                   LstPersonnelID: lstSelectedID
                };
                //批量移除成员
                let res = await this.$apiCircle.batchRemoveMember(req);
                if (res && res.Code == this.$resCode.success) {
                   //查询人际圈内成员信息列表
                   await this.queryCirclePersonnelList(this.circleInfo.ID);
				   this.isChanged = true;
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
        width: 75px;
        height: 75px;
    }
</style>