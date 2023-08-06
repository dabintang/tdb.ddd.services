<!-- 成员选择页 -->
<template>
    <view class="container">
        <uni-indexed-list :options="list" :show-select="true" @click="onClick"/>
    </view>
</template>

<script>
    import { pinyin } from 'pinyin-pro';
    import groupBy from "@/common/groupBy";
    import Enumerable from "linq";
    export default {
        data() {
            return {
                list: [], //人员列表
                lstSelectedID: [], //已选中的人员ID列表
                action: '' //行为(batch.add.circle.member、)
            }
        },
        //挂载到实例上去之后调用
        mounted() {
            //生成显示用的list
            this.createList();
        },
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            this.action = option.action;

            //生成显示用的list
            this.createList();
        },
        //导航栏按钮事件
        onNavigationBarButtonTap(e) {
            let _this = this;
            //跳转回上一页
            uni.navigateBack({
                success: function () {
                    uni.$emit(_this.action, _this.lstSelectedID); //刷新人际圈列表
                }
            });
        },
        methods: {
            //生成显示用的list
            createList() {
                //获取人员选择列表缓存
                let lstPersonnel = this.$storage.getPersonnelSelectList();
                if (!lstPersonnel) lstPersonnel = [];

                let lstTemp = [];
                lstPersonnel.forEach(item => {
                    let namePinyin = pinyin(item.Name, { toneType: 'none' }).toUpperCase();
                    lstTemp.push({
                        letter: namePinyin.charAt(0),
                        namePinyin: namePinyin,
                        id: item.PersonnelID,
                        name: item.Name,
                        pic: this.getPersonnelHeadImage(item)
                    });
                });

                //按namePinyin排序
                lstTemp = Enumerable.from(lstTemp).orderBy("x=>x.namePinyin").toArray();

                let lstTemp2 = [];
                //按首字母分组
                let group = groupBy(lstTemp, (item) => {
                    return item.letter;
                });
                for (var key in group) {
                    lstTemp2.push({
                        letter: key,
                        data: group[key]
                    });
                }

                this.list = lstTemp2;
            },
            //获取人员头像地址
            getPersonnelHeadImage(personnelInfo) {
                if (personnelInfo.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(personnelInfo.HeadImgID, 40);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
            //点击列表项
            onClick(e) {
                this.lstSelectedID = [];
                if (e && e.select) {
                    e.select.forEach(item => {
                        this.lstSelectedID.push(item.id);
                    });
                }
            }
        }
    }
</script>

<style>
</style>