<template>
    <div>
        <div v-if="video !== undefined">
            <h3 class="subtitle-3 text-center">Video: {{video.fileName}}</h3>
            <v-container fluid>
                <v-text-field
                    v-model="url"
                    label="Add address"
                    placeholder="(Example 233.3.3.3:1234)"
                    outlined
                    prefix="udp://"
                    name=udpAddress
                ></v-text-field>
                <v-checkbox
                    v-model="loop"
                    label="Loop"
                    color="primary"
                    ></v-checkbox>
            </v-container>
            <div class="text-center">
                <v-btn @click="startStream(url, loop)" color="primary" dark>Start stream</v-btn>
            </div>
             <v-snackbar
                v-model="snackbar"
                :vertical="vertical"
                >
                {{ text }}
                <v-btn
                    color="indigo"
                    text
                    @click="snackbar = false"
                >
                    Close
                </v-btn>
            </v-snackbar>
        </div>
    </div>
</template>

<script>

import { mapGetters } from 'vuex';
import * as a from '../store/action_types';
export default {
    name: 'StartFeed',
    data () {
        return {
            id: this.$route.params.id,
            loop: true,
            url: '',
            snackbar: false,
            text: 'You started the stream successfully!',
            vertical: true
        };
    },
    computed: {
        ...mapGetters([
            'videos',
            'feeds'
        ]),
        video: function () {
            return this.videos === undefined ? undefined : this.videos.find(v => v.id === this.id);
        }
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_VIDEOS)
            .then(succ => {
                if (succ) {
                    console.log('videos fetched');
                }
            });
    },
    methods: {
        startStream: function (url, loop) {
            console.log(url);
            console.log(loop);
            var urlall = 'udp://' + url.toString();
            console.log({ videoId: this.id, url: urlall, loop: loop });
            this.$store.dispatch(a.START_FEED, { videoId: this.id, url: urlall, loop: loop })
                .then(succ => {
                    console.log(succ);
                    if (succ) {
                        this.text = 'You started the stream successfully!';
                        this.snackbar = true;
                        console.log('feed start');
                        this.$router.push('/feeds');
                    }
                })
                .catch(err => {
                    console.log(err);
                    this.text = 'URL addres is already taken or not valid! Try again!';
                    this.snackbar = true;
                });
        }
    }
};
</script>
