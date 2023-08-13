<!-- 人员列表页 -->
<template>
    <view class="container">
        <uni-indexed-list :options="LstPersonnel" :show-select="false" @click="goDetailPage" @longpress="onLongpress" />
    </view>
</template>

<script>
    import { pinyin } from 'pinyin-pro';
    import Enumerable from "linq";
    import util from "@/common/util";
    export default {
        data() {
            return {
                LstPersonnel: [] //人员列表
            }
        },
        //挂载到实例上去之后调用
        mounted() {
            //监听刷新人员列表事件
            uni.$on('refresh.personnel.list', this.getMyPersonnelList);

            //查询我创建的人员信息列表
            this.getMyPersonnelList();
        },
        //导航栏按钮事件
        onNavigationBarButtonTap(e) {
            //跳到新增人员页面
            uni.navigateTo({
                url: '/pages/personnel/personnelAdd',
                animationType: 'pop-in',
                animationDuration: 300
            });
        },
        //下拉刷新
        async onPullDownRefresh() {
            //查询我创建的人员信息列表
            await this.getMyPersonnelList(false);
            //关闭刷新动画
            uni.stopPullDownRefresh();
        },
        methods: {
            //查询我创建的人员信息列表
            async getMyPersonnelList(showToast = true) {
                let req = {
                    PageNO: 1,
                    PageSize: 100000
                }
                //查询我创建的人员信息列表
                let res = await this.$apiReport.queryMyPersonnelList(req, showToast);
                let list = [];
                if (res.Code == this.$resCode.success) {
                    res.Data.forEach(item => {
                        let namePinyin = pinyin(item.Name, { toneType: 'none' }).toUpperCase();
                        list.push({
                            letter: namePinyin.charAt(0),
                            namePinyin: namePinyin,
                            id: item.ID,
                            name: item.Name,
                            pic: this.getPersonnelHeadImage(item)
                        });
                    });
                }

                //按namePinyin排序
                list = Enumerable.from(list).orderBy("x=>x.namePinyin").toArray();

                let list2 = [];
                //按首字母分组
                let group = util.Fun.groupBy(list, (item) => {
                    return item.letter;
                });
                for (var key in group) {
                    list2.push({
                        letter: key,
                        data: group[key]
                    });
                }

                this.LstPersonnel = list2;
            },
            //获取人员头像地址
            getPersonnelHeadImage(personnelInfo) {
                if (personnelInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(personnelInfo.HeadImgID, 40);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
            //跳转到详情页
            goDetailPage(e) {
                //console.log('点击item，返回数据：' + JSON.stringify(e))
                uni.navigateTo({
                    url: '/pages/personnel/personnelDetail?id=' + e.item.id,
                    animationType: 'pop-in',
                    animationDuration: 300
                });
            },
            //长按事件
            onLongpress(item) {
                let _this = this;
                uni.showActionSheet({
                    itemList: ['删除'],
                    success: async function (res) {
                        let resDel = await _this.deletePersonnel(item);
                        if (resDel && resDel.Code == _this.$resCode.success) {
                            //查询我创建的人员信息列表
                            await _this.getMyPersonnelList();
                        }
                    },
                    fail: function (err) {
                        console.log('personnelList.onLongpress.fail', JSON.stringify(err));
                    }
                });
            },
            //删除人员
            async deletePersonnel(e) {
                let req = {
                    ID: e.item.id
                };
                //删除人员
                return await this.$apiPersonnel.deletePersonnel(req);
            }
        }
    }
</script>

<style>
</style>
