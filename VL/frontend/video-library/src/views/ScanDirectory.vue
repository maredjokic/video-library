<template>
    <div>
        <v-card>
            <v-container
            >
                <v-form
                    ref="form"
                    lazy-validation
                    :class="`d-flex justify-center mt-0`"
                >
                    <v-row>
                        <v-text-field
                        v-model="newDirectory"
                        label="Enter new directory path"
                        outlined
                        dense
                        >
                        </v-text-field>
                        <v-btn
                            color="primary"
                            class="ml-4 mt-1"
                            @click="startScanning()">
                            Start scan
                        </v-btn>
                    </v-row>
                </v-form>
            </v-container>
            <v-card-title>
                Scan directories informations
            </v-card-title>
            <v-data-table
                    :headers="headers"
                    :items='scandirectories'
                    :items-per-page="100000000"
                    :loading='infosFetchingStatus'
                    hide-default-footer
                    class="elevation-0"
                    >
                        <template v-slot:item.status="{ item }">
                            <v-chip :color="getColor(item.status)" dark>{{ item.status }}</v-chip>
                        </template>
                        <template
                                v-slot:item.path="{ item }">
                            <a><router-link v-bind:to="'/scandir/' + item.directoryHash" exact>
                                {{item.path}}
                            </router-link></a>
                        </template>
                        <template
                                v-slot:item.actions="{ item }">
                                <v-icon v-if="item.status === 'processing'" @click="pause(item)" dark  color='primary' large>mdi-pause-circle-outline</v-icon>
                                <v-icon v-if="item.status === 'paused' && notProcessing()" @click="resume(item)" dark  color='primary' large>mdi-play-outline</v-icon>
                                <v-icon v-if="item.status === 'finished' && notProcessing()" @click="cleanup(item)" dark  color='primary' large>mdi-broom</v-icon>
                                <v-icon
                                    v-if="item && deleteStatus(item.directoryHash) && item.status !== 'processing'"
                                    @click="deleteDirectory(item.directoryHash)"
                                    dark
                                    color='primary'
                                    large
                                >mdi-delete-empty</v-icon>
                        </template>
            </v-data-table>
        </v-card>
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
import { mapGetters } from 'vuex';
import * as a from '../store/action_types';

export default {
    name: 'ScanDirectory',
    computed: {
        ...mapGetters([
            'scandirectories',
            'infosFetchingStatus',
            'actionStatuse',
            'deleteStatuses'
        ])
    },
    methods: {
        notProcessing () {
            var notProc = true;
            this.scandirectories.forEach((item) => {
                if (item.status === 'processing') {
                    notProc = false;
                }
            });
            return notProc;
        },
        getColor (status) {
            if (status === 'finished') return 'success';
            else if (status === 'failed') return 'error';
            else return 'warning';
        },
        deleteDirectory (hash) {
            this.$store.dispatch(a.DELETE_SCANDIR, hash)
                .then(succ => {
                    console.log(succ);
                    this.snackbarText = 'The directory data have been deleted!';
                    this.snackbar = true;
                })
                .catch(err => {
                    console.log(err);
                    this.snackbarText = 'Deleting error!';
                    this.snackbar = true;
                });
        },
        deleteStatus (hash) {
            return !this.deleteStatuses.has(hash);
        },
        startScanning () {
            console.log('Try scanning dir: ' + this.newDirectory);
            this.$store.dispatch(a.SCAN_SCANDIR, { path: this.newDirectory })
                .then(succ => {
                    console.log(succ);
                    if (succ.status === 200) {
                        this.snackbarText = this.newDirectory + ' directory processing started!';
                        this.snackbar = true;
                    } else {
                        this.snackbarText = 'Scan error!';
                        this.snackbar = true;
                    }
                })
                .catch(err => {
                    console.log(err);
                    this.snackbarText = 'Scan error!';
                    this.snackbar = true;
                });
            this.snackbar = true;
        },
        pause (entry) {
            console.log('pause: ' + entry.path);
            this.$store.dispatch(a.PAUSE_SCANDIR, { path: entry.path })
                .then(succ => {
                    console.log(succ);
                    this.snackbarText = entry.path + ' directory processing paused!';
                    this.snackbar = true;
                })
                .catch(err => {
                    console.log(err);
                    this.snackbarText = 'Pause error!';
                    this.snackbar = true;
                });
        },
        resume (entry) {
            console.log('resume: ' + entry.path);
            this.$store.dispatch(a.RESUME_SCANDIR, { path: entry.path })
                .then(succ => {
                    console.log(succ);
                    this.snackbarText = entry.path + ' directory processing has been resumed!';
                    this.snackbar = true;
                })
                .catch(err => {
                    console.log(err);
                    this.snackbarText = 'Resume error!';
                    this.snackbar = true;
                });
        },
        cleanup (entry) {
            console.log('cleanup: ' + entry.path);
            this.$store.dispatch(a.CLEANUP_SCANDIR, { path: entry.path })
                .then(succ => {
                    console.log(succ);
                    this.snackbarText = entry.path + ' directory cleanup is started!';
                    this.snackbar = true;
                })
                .catch(err => {
                    console.log(err);
                    this.snackbarText = 'Cleanup error!';
                    this.snackbar = true;
                });
        }
    },
    mounted: function () {
        this.$store.dispatch(a.FETCH_SCANDIR_INFOS)
            .then(succ => {
                if (succ) {
                    console.log('Scan dir infos fetched');
                }
            });
    },
    data () {
        return {
            snackbar: false,
            snackbarText: '',
            newDirectory: '',
            search: '',
            headers: [
                {
                    text: 'Status', value: 'status'
                },
                {
                    text: 'Path', value: 'path'
                },
                {
                    text: 'Finished', value: 'finishedEntries'
                },
                {
                    text: 'Total', value: 'totalEntries'
                },
                {
                    text: 'Actions', value: 'actions', sortable: false
                }
            ]
        };
    }
};
</script>

<style>
</style>
