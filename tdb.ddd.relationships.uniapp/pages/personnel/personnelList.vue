<!-- 人员列表页 -->
<template>
    <view class="container">
        <!--<uni-list>
        <uni-list-item v-for="(item, index) in LstPersonnel" :title="item.Name"
                       showArrow :thumb="getPersonnelHeadImage(item)" thumb-size="lg" />
    </uni-list>-->
        <uni-indexed-list :options="LstPersonnel" :show-select="false" @click="bindClick" />
    </view>
</template>

<script>
    import { pinyin } from 'pinyin-pro';
    import groupBy from "@/common/groupBy";
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
        methods: {
            //查询我创建的人员信息列表
            async getMyPersonnelList(showToast = true) {
                let req = {
                    PageNO: 1,
                    PageSize: 100000,
                    LstSortItem: [{ FieldCode: 'Name', SortCode: 1}]
                }
                //查询我创建的人员信息列表
                let res = await this.$apiReport.queryMyPersonnelList(req, showToast);
                let list = [];
                if (res.Code == this.$resCode.success) {
                    res.Data.forEach(item => {
                        list.push({
                            letter: pinyin(item.Name.charAt(0), { pattern: 'first' }).toUpperCase(),
                            id: item.ID,
                            name: item.Name,
                            pic: this.getPersonnelHeadImage(item)
                        });
                    });
                }

                let list2 = [];
                //按首字母分组
                let group = groupBy(list, (item) => {
                    return item.letter;
                });
                for (var key in group) {
                    list2.push({
                        letter: key,
                        data: group[key]
                    });
                }

                this.LstPersonnel = list2;

                console.log('list2', JSON.stringify(list2));
            },
            //获取人员头像地址
            getPersonnelHeadImage(personnelInfo) {
                if (personnelInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(personnelInfo.HeadImgID, 40);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            }
        }
    }
</script>

<style>
</style>