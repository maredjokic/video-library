<template>
  <div>
    <v-container class="text-center">
        <v-row>
        <v-text-field
            class='px-12'
            v-model="search"
            append-icon="mdi-magnify"
            label="Search"
            single-line
            hide-details
            style="width: 500px"
            @change="searchVideos()"
        ></v-text-field>
        <v-tooltip color="primary" left>
            <template v-slot:activator="{ on }">
            <v-icon  v-on="on" class='px-2' medium dark color='primary' large>mdi-help-circle</v-icon>
            </template>
            <span>
                <div>Search example:</div>
                <div>:fileName:name :formatLongName:format</div>
                <div>:codecName:codec :tags:tag1,tag2,tag3</div>
            </span>
        </v-tooltip>
        </v-row>
    </v-container>
    <v-container fluid>
        <v-row>
            <div v-for="video in videos" :key="video.id">
                <video-card :video=video></video-card>
            </div>
        </v-row>
    </v-container>
    <v-row>
        <v-col>
            <div class="text-center">
                <v-pagination
                v-model="page"
                :length="videoData.totalPages"
                :total-visible="7"
                @input="changePage()"
                ></v-pagination>
            </div>
        </v-col>
        <v-col>
        <v-text-field
            v-model="pageSize"
            label="Videos per page"
            max="40"
            min="5"
            step="5"
            style="width: 125px"
            type="number"
            @change="changePageSize()"
        ></v-text-field>
        </v-col>
    </v-row>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';
import VideoCard from '../components/VideoCard';

export default {
    name: 'Videos',
    computed: {
        ...mapGetters([
            'videos',
            'videoData'
        ])
    },
    data () {
        return {
            search: '',
            page: 1,
            pageSize: 10
        };
    },
    components: {
        'video-card': VideoCard
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_VIDEOS, { pageSize: 10, page: 1, silently: false })
            .then(succ => {
                if (succ) {
                    console.log('videos fetched');
                }
            });
    },
    methods: {
        changePage () {
            this.$store.dispatch(a.FETCH_VIDEOS, { pageSize: this.pageSize, page: this.page, silently: false })
                .then(succ => {
                    if (succ) {
                        console.log('videos fetched');
                    }
                });
        },
        changePageSize () {
            this.$store.dispatch(a.FETCH_VIDEOS, { pageSize: this.pageSize, page: 1, silently: false })
                .then(succ => {
                    if (succ) {
                        console.log('videos fetched');
                    }
                });
        },
        searchVideos () {
            this.$router.push('/Videos/search/' + this.search);
        }
    }
};
</script>

<style>
</style>
