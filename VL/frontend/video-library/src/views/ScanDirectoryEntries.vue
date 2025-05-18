<template>
  <div>
    <v-container fluid>
    <v-card>
        <v-card-title>
            Entries
        </v-card-title>
        <v-data-table
                :headers="headers"
                :items='scandirectoryEntries'
                :items-per-page="100000000"
                :loading='entriesFetchingStatus'
                hide-default-footer
                class="elevation-2"
                >
                <template v-slot:item.status="{ item }">
                    <v-chip :color="getColor(item.status)" dark>{{ item.status }}</v-chip>
                </template>
                <template v-slot:item.videoId="{ item }">
                    <a><router-link v-bind:to="'/Video/' + item.videoId" exact>
                        <v-icon  dark  color='primary' large>mdi-file-video</v-icon>
                    </router-link></a>
                </template>
        </v-data-table>
    </v-card>
    </v-container>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';

export default {
    name: 'ScanDirectoryEntries',
    data () {
        return {
            hash: this.$route.params.hash,
            headers: [
                {
                    text: 'Status', value: 'status'
                },
                {
                    text: 'Path', value: 'filePath'
                },
                {
                    text: 'Video', value: 'videoId'
                }
            ]
        };
    },
    methods: {
        getColor (status) {
            if (status === 'finished') return 'success';
            else if (status === 'failed') return 'error';
            else return 'warning';
        }
    },
    computed: {
        ...mapGetters([
            'scandirectoryEntries',
            'entriesFetchingStatus'
        ])
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_SCANDIR_ENTRIES, this.$route.params.hash)
            .then(succ => {
                if (succ) {
                    console.log('videos fetched');
                }
            });
    }
};
</script>

<style>
</style>
