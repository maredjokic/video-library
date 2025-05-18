<template>
   <video-player class="video-player-box"
        ref="videoPlayer"
        :options="playerOptions"
        :playsinline="true"
        customEventName="customstatechangedeventname"
    >
   </video-player>
</template>

<script>
export default {
    props: ['videoSource'],
    data () {
        return {
            playerOptions: {
                autoplay: true,
                controls: true,
                muted: false,
                language: 'en',
                controlBar: {
                    timeDivider: false,
                    durationDisplay: false
                },
                playbackRates: [0.7, 1.0, 1.5, 2.0],
                sources: [{
                    type: 'video/mp4',
                    src: this.videoSource
                }],
                poster: '/static/images/author.jpg'
            }
        };
    },
    mounted () {
    },
    computed: {
        player () {
            return this.$refs.videoPlayer.player;
        }
    }
};
</script>

<style>
.video-player {
    width: 60%;
    height: 60%;
    min-width: 850px;
}
</style>
