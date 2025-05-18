<template>
    <div>
         <v-card>
            <v-card-title>
                Streams
            <v-spacer></v-spacer>
                <v-text-field
                    v-model="search"
                    append-icon="mdi-magnify"
                    label="Search"
                    single-line
                    hide-details
                ></v-text-field>
                </v-card-title>
                <v-data-table
                v-if='videos !== undefined || feeds !== undefined'
                :headers="headers"
                :items=allFeeds
                :items-per-page="100000000"
                :loading='feedsFetchingStatus'
                hide-default-footer
                class="elevation-2"
                :search=search
                >
                    <template
                            v-slot:item.active="{item}">
                            <v-icon dark :color="item.active ? 'success' : 'error'" large>mdi-circle-medium</v-icon>
                    </template>
                    <template
                            v-slot:item.restart="{item}">
                            <v-icon v-if="item.active === false && !restartingStatus(item.feedId)" @click="restartFeed(item.feedId)" dark color='primary' large>mdi-play-speed</v-icon>
                    </template>
                    <template
                            v-slot:item.stop="{item}">
                            <v-icon v-if="!deletingStatus(item.feedId)" dark @click="deleteFeed(item.feedId)" color='primary' large>mdi-stop-circle-outline</v-icon>
                    </template>
                    <template
                            v-slot:item.loop="{item}">
                            <v-icon dark small :color="item.loop ? 'success' : 'error'">mdi-loop</v-icon>
                    </template>
                    <template v-slot:item.thumbnail="{ item }">
                        <div class="p-1 ma-2">
                        <v-img :src="item.thumbnail" :alt="item.thumbnail" width="90px" height="70px"></v-img>
                        </div>
                    </template>
                    <template
                            v-slot:item.url="{item}">
                            <a v-bind:href="item.url">{{ item.url }}</a>
                    </template>
                </v-data-table>
        </v-card>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';
export default {
    name: 'Feeds',
    computed: {
        ...mapGetters([
            'videos',
            'feeds',
            'video',
            'feedsFetchingStatus',
            'feedsDeletingStatuses',
            'feedsRestartingStatuses'
        ]),
        allFeeds: function () {
            var newFeeds = this.feeds;
            for (var i = 0; i < newFeeds.length; i++) {
                newFeeds[i].videoName = this.video(newFeeds[i].videoId).fileName;
                newFeeds[i].thumbnail = this.video(newFeeds[i].videoId).thumbnail;
            }
            return newFeeds;
        }
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_FEEDS)
            .then(succ => {
                if (succ) {
                    console.log('feeds fetched');
                }
            });
    },
    methods: {
        deleteFeed (id) {
            console.log(id);
            this.$store.dispatch(a.DELETE_FEED, id)
                .then(succ => {
                    if (succ) {
                        console.log('feed deleted');
                    }
                });
        },
        restartFeed (id) {
            console.log(id);
            this.$store.dispatch(a.RESTART_FEED, id)
                .then(succ => {
                    if (succ) {
                        console.log('feed restart');
                    }
                });
        },
        deletingStatus (id) {
            return this.feedsDeletingStatuses.has(id);
        },
        restartingStatus (id) {
            return this.feedsRestartingStatuses.has(id);
        }
    },
    data () {
        return {
            search: '',
            headers: [
                {
                    text: 'Active', value: 'active'
                },
                {
                    text: 'Loop', value: 'loop'
                },
                {
                    text: 'Video', value: 'thumbnail', sortable: false
                },
                {
                    text: 'Video name',
                    align: 'start',
                    sortable: true,
                    value: 'videoName'
                },
                {
                    text: 'Feed id', value: 'feedId'
                },
                {
                    text: 'URL', value: 'url'
                },
                {
                    text: 'Restart', value: 'restart', sortable: false
                },
                {
                    text: 'Stop', value: 'stop', sortable: false
                }
            ]
        };
    }
};
</script>

<style>
</style>
