<!-- 成员详情页 -->
<template>
    <view class="container">
        <uni-section title="基本信息" type="line">
            <uni-list>
                <uni-list-item title="姓名" :thumb="headImage" thumb-size="lg" :rightText="personnelInfo.Name" />
                <uni-list-item title="性别" :rightText="genderText" />
                <uni-list-item title="生日" :rightText="birthdayText" />
                <uni-list-item title="手机" :rightText="personnelInfo.MobilePhone" />
                <uni-list-item title="邮箱" :rightText="personnelInfo.Email" />
                <uni-list-item title="备注" :rightText="personnelInfo.Remark" ellipsis="2" />
                <uni-list-item title="角色" :rightText="roleCodeText" v-if="personnelInfo.UserID" :show-switch="isCreator" :switchChecked="isAdmin" @switchChange="roleCodeChange"></uni-list-item>
            </uni-list>
        </uni-section>
        <view style="height:7px;background-color:#F5F5F5;"></view>
        <uni-section title="照片" type="line">
            <view class="grid-dynamic-box">
                <uni-grid :column="4" :show-border="false" @change="gridClick">
                    <uni-grid-item v-for="(item, index) in lstImage" :index="index" :key="index">
                        <view class="grid-item-box">
                            <image :src="showImage(item,100)" class="grid-image" mode="aspectFill" />
                        </view>
                    </uni-grid-item>
                </uni-grid>
            </view>
        </uni-section>
    </view>
</template>

<script>
    import util from "@/common/util";
    export default {
        data() {
            return {
                // 人员基本信息
                personnelInfo: {
                    ID: '', //人员ID
                    Name: '', //姓名
                    GenderCode: 3, //性别（1：男；2：女；3：保密）
                    HeadImgID: null, //头像图片ID
                    Birthday: null, //生日
                    MobilePhone: '', //手机号码
                    Email: '', //电子邮箱
                    Remark: '', //备注
                    LstPhotoID: [], //照片ID集合
                    UserID: null, //人员关联的用户ID
                    CreatorID: '' //创建者用户ID
                },
                lstImage: [], //照片信息列表
                circleID: '', //人际圈ID
                isAdmin: false, //是否管理员
                isCreator: false, //当前登录用户是否人际圈创建者
            }
        },
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            this.circleID = option.circleID; //人际圈ID
            this.isCreator = option.isCreator; //当前登录用户是否人际圈创建者
            this.isAdmin = option.roleCode == 2; //成员角色（1：普通成员；2；管理员）

            //获取人员信息
            this.getPersonnel(option.id);
        },
        computed: {
            //人员头像
            headImage() {
                if (this.personnelInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(this.personnelInfo.HeadImgID, 100);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
            //性别描述
            genderText() {
                switch (this.personnelInfo.GenderCode) {
                    case 1:
                        return '男';
                    case 2:
                        return '女';
                    default:
                        return '保密';
                }
            },
            //生日文本
            birthdayText() {
                if (!this.personnelInfo.Birthday) {
                    return '';
                }
                return util.Fun.formatDate(this.personnelInfo.Birthday);
            },
            //角色文本
            roleCodeText() {
                if (this.isAdmin) {
                    return '管理员';
                }
                return '普通成员';
            },
        },
        methods: {
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
                this.lstImage = lstImg;
            },
            //显示照片
            showImage(imageInfo, size = 0) {
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
                if (imageInfo) {
                    //预览图片
                    this.previewImage(index);
                }
            },
            //预览图片
            previewImage(imgIndex) {
                if (this.personnelInfo.LstPhotoID.length == 0) {
                    return;
                }
                //图片urls
                let imgUrls = [];
                this.personnelInfo.LstPhotoID.forEach(item => {
                    imgUrls.push(this.showImage({ ImageID: item }));
                });
                //预览图片
                uni.previewImage({
                    urls: imgUrls,
                    current: imgIndex
                });
            },
            //角色修改
            async roleCodeChange(e) {
                let oldValue = this.isAdmin;
                this.isAdmin = e.value;
                //设置成员角色
                let res = await this.setMemberRole();
                if (!res || res.Code != this.$resCode.success) {
                    this.isAdmin = oldValue;
                }
            },
            //设置成员角色
            async setMemberRole() {
                let req = {
                    CircleID: this.circleID,
                    PersonnelID: this.personnelInfo.ID,
                    RoleCode: this.isAdmin?2:1
                };
                //设置成员角色
                let res = await this.$apiCircle.setMemberRole(req);
                if (res && res.Code == this.$resCode.success) {
                    uni.$emit('set.member.role', req);
                }

                return res;
            }
        }
    }
</script>

<style lang="scss" scoped>
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