<script>
	export default {
		onLaunch: function() {
			console.log('App Launch')
            // 禁止横屏
            // #ifdef APP-PLUS
				plus.screen.lockOrientation("portrait-primary")
			// #endif
		},
		onShow: function() {
			console.log('App Show')
		},
		onHide: function() {
			console.log('App Hide')
		}
	}
</script>

<style>
	/*每个页面公共css */
  /*page {
		background-color: #F8F8F8;
	}*/
</style>
