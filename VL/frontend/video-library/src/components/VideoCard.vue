<template>
  <div>
      <template>
  <v-card
    class="ma-2"
    max-width="200"
    width="200"
    height="270"
  >
  <a><router-link v-bind:to="'/Video/' + video.id" exact>
    <div class="video" @mouseover="hover = false"
      @mouseleave="hover = true">
        <v-img v-show=hover
        class="align-end"
        height="150px"
        v-bind:src=video.thumbnail
        >
        <v-container class="font-weight-light" fluid>
            <v-row v-if="allTags">
                <div v-for="(oneTag, i) in allTags.data" :key="i">
                    <div v-if="i < 4">
                      <v-chip small
                      outlined
                      >
                          {{oneTag.name}}
                      </v-chip>
                    </div>
                </div>
            </v-row>
        </v-container>
          <h3 class="text-right mr-2">{{toHHMMSS(video.duration)}}</h3>
        </v-img>
        <v-img v-show=!hover
        class="align-end"
        height="150px"
        v-bind:src=video.preview
        >
        <v-container class=".font-italic font-weight-light" fluid>
            <v-row v-if="allTags">
                <div v-for="(oneTag, i) in allTags.data" :key="i">
                    <div v-if="i < 4">
                      <v-chip small
                      outlined
                      >
                          {{oneTag.name}}
                      </v-chip>
                    </div>
                </div>
            </v-row>
        </v-container>
       <h3 class="text-right mr-2">{{toHHMMSS(video.duration)}}</h3>
        </v-img>
    </div>
    <v-tooltip color="primary" v-model="show" top>
        <template v-slot:activator="{ on }">
            <v-card-subtitle v-on="on" class="pb-0">
                <span
                    class="d-inline-block text-truncate"
                    style="max-width: 160px;"
                    >
                    {{video.fileName}}
                </span>
            </v-card-subtitle>
        </template>
        <span>{{video.fileName}}</span>
    </v-tooltip>
    <v-card-text class="text--primary font-weight-light">
        <div>{{video.height}} x {{video.width}}</div>
        <v-container fluid>
            <v-row>
                <div>
                    <a><router-link v-bind:to="'/startFeed/' + video.id" exact>
                        <v-btn dark small color="primary">
                            <v-icon dark>mdi-play-speed</v-icon>
                        </v-btn>
                    </router-link></a>
                </div>
                <div class="ma-1">
                </div>
                <div v-if="hasFeed(video.id)" class="ml-2 text-left font-weight-light">
                    <p class="success--text">Streaming[{{countFeeds(video.id)}}]</p>
                </div>
            </v-row>
        </v-container>
    </v-card-text>
  </router-link></a>
  </v-card>
</template>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';
export default {
    name: 'VideoCard',
    props: ['video'],
    data () {
        return {
            hover: true,
            show: false
        };
    },
    computed: {
        ...mapGetters([
            'tags',
            'feeds'
        ]),
        allTags: function () {
            return this.tags.find(t => this.video.id === t.id);
        }
    },
    methods: {
        toHHMMSS: function (secondsDuration) {
            var secNum = parseInt(secondsDuration, 10);
            var hours = Math.floor(secNum / 3600);
            var minutes = Math.floor((secNum - (hours * 3600)) / 60);
            var seconds = secNum - (hours * 3600) - (minutes * 60);
            if (hours < 10) {
                hours = '0' + hours;
            }
            if (hours < 1) {
                hours = '';
            } else {
                hours = hours + ':';
            }
            if (minutes < 10) {
                minutes = '0' + minutes;
            }
            if (seconds < 10) {
                seconds = '0' + seconds;
            }
            return hours + minutes + ':' + seconds;
        },
        hasFeed: function (id) {
            if (this.feeds.find(f => id === f.videoId && f.active === true) !== undefined) {
                return true;
            } else {
                return false;
            }
        },
        countFeeds: function (id) {
            var number = 0;
            this.feeds.forEach(element => {
                if (element.videoId === id && element.active === true) {
                    number++;
                }
            }
            );
            return number;
        }
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_TAGS, { id: this.video.id, number: 100 })
            .then(succ => {
                if (succ) {
                    console.log('tags fetched');
                }
            });
        this.$store.dispatch(a.FETCH_FEEDS)
            .then(succ => {
                if (succ) {
                    console.log('feeds fetched');
                }
            });
    }
};
</script>

<style>
a {
  text-decoration: none;
}
</style>
