<template>
  <div v-if=!fetchingVideoById>
    <div v-if="video !== undefined">
        <div v-if="video.fileName !== null">
            <p class="subtitle-1 text-center">Video file name: {{video.fileName}}</p>
        </div>
   </div>
   <div class="text-center">
    <a><router-link v-bind:to="'/startFeed/' + video.id" exact>
            <v-btn class="text-end mx-4" dark large color="primary">
                Start stream
                <v-icon dark>mdi-play-speed</v-icon>
            </v-btn>
    </router-link></a>
    <v-btn @click="deleteVideo()" class="text-end mx-4" dark large color="primary">
                Delete video
                <v-icon dark>mdi-delete</v-icon>
            </v-btn>
    </div>
    <div>
        <v-container fluid v-if="video !== undefined">
            <v-row>
            <video-player class="mr-2" v-if="video !== undefined" :videoSource=video.transcodedVideo></video-player>
            <vue-map v-if="geolocation !== undefined" :geolocation=geolocation class="mx-2"></vue-map>
            </v-row>
        </v-container>
    </div>
    <v-container fluid>
        <v-row v-if="allTags">
            <div v-for="(oneTag, i) in allTags.data" :key="i">
                <v-chip
                outlined
                >
                    {{oneTag.name}}
                </v-chip>
            </div>
        </v-row>
    </v-container>
    <v-container class="mx-auto">
        <v-row>
            <v-text-field v-model="newTagName" style="width: 200px" class="mx-6" label="Tag name"></v-text-field>
            <v-btn @click="addNewTag()" class="text-end mx-6" dark large color="primary">
                Add new tag
                <v-icon dark>mdi-tag-multiple</v-icon>
            </v-btn>
        </v-row>
    </v-container>
    <v-container fluid>
        <video-metadata :video=video></video-metadata>
    </v-container>
    <v-snackbar
        v-model="snackbar"
        :vertical=true
        >
        {{snackbarText}}
        <v-btn
            color="primary"
            @click="snackbar = false"
        >
            Close
        </v-btn>
        </v-snackbar>
  </div>
</template>

<script>
import Map from '../components/selectedVideoComponents/Map';
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';
import VideoPlayer from '../components/selectedVideoComponents/VideoPlayer';
import Metadata from '../components/selectedVideoComponents/Metadata';
export default {
    name: 'SelectedVideo',
    data () {
        return {
            id: this.$route.params.id,
            newTagName: '',
            snackbar: false,
            snackbarText: ''
        };
    },
    components: {
        'video-player': VideoPlayer,
        'vue-map': Map,
        'video-metadata': Metadata
    },
    computed: {
        ...mapGetters([
            'videos',
            'tags',
            'geolocations',
            'video',
            'fetchingVideoById'
        ]),
        allTags: function () {
            return this.tags.find(t => this.id === t.id);
        },
        geolocation: function () {
            return this.geolocations.find(g => this.id === g.videoId);
        }
    },
    methods: {
        deleteVideo () {
            this.$store.dispatch(a.DELETE_VIDEO, this.id)
                .then(succ => {
                    if (succ) {
                        this.$router.push('/');
                    }
                })
                .catch(err => {
                    this.snackbarText = 'Error!';
                    this.snackbar = true;
                    throw err;
                });
        },
        addNewTag () {
            if (this.newTagName !== '') {
                this.$store.dispatch(a.ADD_NEW_TAG, { tagName: this.newTagName, videoId: this.id })
                    .then(succ => {
                        if (succ) {
                            this.snackbarText = 'The tag -' + this.newTagName + '- was added successfully!';
                            this.snackbar = true;
                        }
                        this.$store.dispatch(a.FETCH_TAGS, { id: this.id, number: 100 })
                            .then(succ => {
                                if (succ) {
                                    console.log('tags fetched');
                                }
                            });
                    })
                    .catch(err => {
                        this.snackbarText = 'Error!';
                        this.snackbar = true;
                        throw err;
                    });
            } else {
                this.snackbarText = 'First fill in the field Tag name!';
                this.snackbar = true;
            }
            this.$store.dispatch(a.FETCH_TAGS, { id: this.id, number: 100 })
                .then(succ => {
                    if (succ) {
                        console.log('tags fetched');
                    }
                });
        }
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_VIDEO_BY_ID, this.id)
            .then(succ => {
                if (succ) {
                    console.log('video fetched');
                }
            });
        this.$store.dispatch(a.FETCH_TAGS, { id: this.id, number: 100 })
            .then(succ => {
                if (succ) {
                    console.log('tags fetched');
                }
            });
        this.$store.dispatch(a.FETCH_GEOLOCATION, this.id)
            .then(succ => {
                if (succ) {
                    console.log('geolocation fetched');
                }
            });
    }
};
</script>
