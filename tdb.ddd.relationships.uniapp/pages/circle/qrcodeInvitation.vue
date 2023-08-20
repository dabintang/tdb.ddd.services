<!-- 显示邀请二维码页面 -->
<template>
    <view class="container">
        <uni-card :title="cardTitle">
            <!--<div ref="qrcode" style="margin-bottom:7px"></div>-->
            <uqrcode ref="qrcode" canvas-id="qrcode" size="250" @click="createInvitationCode(circleID)" style="margin-bottom:7px"
                     :options="{errorCorrectLevel: 'L'}" :value="qrcode.Code" :start="true" :auto="true"></uqrcode>
            <text class="bottomText">请在<uni-dateformat :date="qrcode.ExpireAt" format="yyyy-MM-dd hh:mm"></uni-dateformat>前完成扫码</text>
        </uni-card>
    </view>
</template>

<script>
    export default {
        data() {
            return {
                //二维码信息
                qrcode: {
                    Code: '', //邀请码
                    CircleName: '', //人际圈名称
                    InviterName: '', //邀请人姓名
                    ExpireAt: '' //过期时间点
                },
                //人际圈ID
                circleID: ''
            }
        },
        //加载页面时
        onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            this.circleID = option.circleID;
            //生成加入人际圈的邀请码
            this.createInvitationCode(option.circleID);
        },
        computed: {
            cardTitle() {
                if (this.qrcode.InviterName) {
                    return this.qrcode.InviterName + '邀请您加入【' + this.qrcode.CircleName + '】';
                }
                return '';
            }
        },
        methods: {
            //生成加入人际圈的邀请码
            async createInvitationCode(circleID) {
                console.log('createInvitationCode', circleID);
                let req = {
                    CircleID: circleID,
                    EffectiveMinutes: 10
                };
                //生成加入人际圈的邀请码
                let res = await this.$apiCircle.createInvitationCode(req);
                console.log('生成加入人际圈的邀请码', JSON.stringify(res));
                if (res.Code == this.$resCode.success) {
                    this.qrcode = res.Data;
                }
            }
        }
    }
</script>

<style lang="scss" scoped>
    .container {
        text-align: -webkit-center;
    }

    .bottomText {
        font-size: 12px;
        color: #666;
    }
</style>