<template>
  <div>
    <v-card>
        <v-card-title>
            Videos in processing
        </v-card-title>
        <v-data-table
                :headers="headers"
                :items=processingVideos
                :items-per-page="100000000"
                :loading='fetchingProcessingVideos'
                hide-default-footer
                class="elevation-2"
                >
                    <template v-slot:item.thumbnail="{ item }">
                        <div class="p-1 ma-2">
                        <v-img :src="item.thumbnail" :alt="item.thumbnail" width="90px" height="70px"></v-img>
                        </div>
                    </template>
                    <template
                            v-slot:item.delete="{ item }">
                            <v-icon v-if="item" dark  color='primary' large>mdi-delete-empty</v-icon>
                    </template>
        </v-data-table>
    </v-card>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';

export default {
    name: 'ProcessingVideos',
    computed: {
        ...mapGetters([
            'processingVideos',
            'fetchingProcessingVideos'
        ])
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_PROCESSING_VIDEOS)
            .then(succ => {
                if (succ) {
                    console.log('processing videos fetched');
                }
            });
    },
    data () {
        return {
            search: '',
            headers: [
                {
                    text: 'Video', value: 'thumbnail', sortable: false
                },
                {
                    text: 'Hash/Id', value: 'id', sortable: false
                },
                {
                    text: 'Video name', value: 'fileName'
                },
                {
                    text: 'Processes left', value: 'processesLeft'
                },
                {
                    text: 'Size in bytes', value: 'size'
                },
                {
                    text: 'Delete', value: 'delete', sortable: false
                }
            ]
        };
    }
};
</script>

<style>
</style>
