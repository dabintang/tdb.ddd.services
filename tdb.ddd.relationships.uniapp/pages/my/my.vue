<!-- 个人中心页 -->
<template>
    <view>
        <uni-card :title="curUser.Name" :thumbnail="headImage">
            <uni-list>
                <uni-list-item title="启用指纹登录" :rightText="supportFingerPrintText" thumb="/static/img/fingerprint.png" thumb-size="sm"
                               :show-switch="isSupportFingerPrint" :switchChecked="fingerprintChecked" @switchChange="fingerprintChange" />
            </uni-list>
        </uni-card>
        <button class="btn" @click="logout" type="warn" plain="true">退出登录</button>
    </view>
</template>

<script>
	export default {
		data() {
			return {
                //当前登录用户信息
				curUser: {
					ID: '', //用户ID
					Name: '', //姓名
					HeadImgID: null, //头像ID
                    PersonnelID: '', //人员ID
				},
                isSupportFingerPrint: false, //
                supportFingerPrintText: '', //设备支持指纹识别情况描述
                fingerprintChecked: false //是否启用指纹
			}
		},
		//加载页面时
		async onLoad(option) { //option为object类型，会序列化上个页面传递的参数
            //获取当前用户信息
			this.curUser = this.$storage.getCurrentUser();
            //是否启用指纹
			this.fingerprintChecked = this.$storage.getFingerprint();

			//检查设备是否支持指纹识别
            await this.checkFingerPrint();
        },
        //导航栏按钮事件
        async onNavigationBarButtonTap(e) {
            let res = await this.$uniCom.scanCode();
            console.log('扫码结果：', JSON.stringify(res));
            if (res && res.result) {
                //通过邀请码加入人际圈
                await this.joinByInvitationCode(res.result);
            } else {
                uni.showToast({
                    title: '未识别到有效二维码',
                    icon: 'none',
                    duration: 5000
                });
            }
        },
        //下拉刷新
        async onPullDownRefresh() {
            //检查设备是否支持指纹识别
			await this.checkFingerPrint();
            //关闭刷新动画
            uni.stopPullDownRefresh();
        },
        computed: {
            //人员头像
            headImage() {
                if (this.curUser.HeadImgID) {
                    return this.$apiFiles.downloadImageAnonUrl(this.curUser.HeadImgID, 100);
                } else {
                    return '/static/img/personnel-default-head.png';
                }
            },
        },
		methods: {
            //检查设备是否支持指纹识别及是否已录入指纹
            async checkFingerPrint() {
                //检查设备是否支持指纹识别
                let isSupport = await this.$uniCom.checkIsSupportSoterAuthentication('fingerPrint');
                if (!isSupport) {
                    this.isSupportFingerPrint = false;
                    this.supportFingerPrintText = '设备不支持指纹识别';
                    return;
                }

                //检查是否已录入指纹
                isSupport = await this.$uniCom.checkIsSoterEnrolledInDevice('fingerPrint');
                if (!isSupport) {
                    this.isSupportFingerPrint = false;
                    this.supportFingerPrintText = '设备尚未录入指纹';
                    return;
                }

                this.isSupportFingerPrint = true;
            },
            //是否启用指纹登录修改
			async fingerprintChange(e) {
                this.fingerprintChecked = e.value;
                if (e.value && this.isSupportFingerPrint) {
                    let res = await this.$uniCom.startSoterAuthentication(['fingerPrint']);
					if (res.code == 0) {
                        //添加指纹凭证
                        let resAdd = await this.addCertificateFingerprint();
                        if (!resAdd || resAdd.Code != this.$resCode.success) {
                            this.fingerprintChecked = false;
                        } else {
							this.$storage.setFingerprint(true);
						}
					} else {
                        this.fingerprintChecked = false;
                        uni.showToast({
                            title: res.msg,
                            icon: 'none'
                        });
                    }
                } else {
					//删除指纹凭证
					await this.deleteCertificate();
				}
            },
            //添加指纹凭证
            async addCertificateFingerprint() {
                //获取系统信息
                let sysInfo = this.$uniCom.getSystemInfoSync();

                let req = {
                    CertificateTypeCode: 1,
                    Credentials: sysInfo.deviceId
                };
                //添加凭证
                return await this.$apiAccount.addCertificate(req);
            },
            //删除指纹凭证
            async deleteCertificate() {
                //获取系统信息
                let sysInfo = this.$uniCom.getSystemInfoSync();

                let req = {
                    CertificateTypeCode: 1,
                    Credentials: sysInfo.deviceId
                };
                //删除凭证
                return await this.$apiAccount.deleteCertificate(req);
            },
			//退出登录
            async logout() {
                let resAsk = await uni.showModal({
                    title: '退出确认',
                    content: '你确定要退出登录吗？'
                });
                if (resAsk && resAsk.confirm) {
                    //删除指纹凭证
                    await this.deleteCertificate();
                    //移除是否启用指纹
                    this.$storage.removeFingerprint();
                    //移除token
                    this.$storage.removeToken();
                    //移除当前用户信息
                    this.$storage.removeCurrentUser();

                    //弹框提示
                    uni.showToast({
                        title: '退出成功',
                        icon: 'none',
                        complete: () => {
                            setTimeout(() => {
                                //跳转到登录页
                                uni.reLaunch({
                                    url: '/pages/login/login'
                                });
                            }, 1000);
                        }
                    });
                }
            },
            //通过邀请码加入人际圈
            async joinByInvitationCode(code) {
                let req = {
                    Code: code
                };
                //通过邀请码加入人际圈
                let res = this.$apiCircle.joinByInvitationCode(req);
                if (res.Code == this.$resCode.success) {
                    uni.showToast({
                        title: '欢迎加入“' + res.CircleName+'”,',
                        icon: 'none'
                    });
                }
            },
		}
	}
</script>

<style lang="scss" scoped>
    .btn {
        margin: 15px;
        width: 90%;
    }
</style>
