import * as m from '../mutation_types';
import * as a from '../action_types';
import { backend } from '../../backend/index';
import store from '..';

const state = {
    scandirectories: [],
    dirInfosFetchingStatus: false,
    scandirectoryEntries: [],
    dirEntriesFetchingStatus: false,
    actionStatuses: new Map(),
    deleteStatuses: new Map()
};

const getters = {
    scandirectories: state => state.scandirectories,
    infosFetchingStatus: state => state.dirInfosFetchingStatus,
    scandirectoryEntries: state => state.scandirectoryEntries,
    entriesFetchingStatus: state => state.dirEntriesFetchingStatus,
    actionStatuses: state => state.actionStatuses,
    deleteStatuses: state => state.deleteStatuses
};

const mutations = {
    [m.SCANDIR_INFOS_FETCH_REQUEST] (state) {
        state.infosFetchingStatus = true;
    },
    [m.SCANDIR_INFOS_FETCH_SUCCESS] (state, scandir) {
        state.scandirectories = scandir;
        state.infosFetchingStatus = false;
    },
    [m.SCANDIR_INFOS_FETCH_FAILED] (state) {
        state.infosFetchingStatus = false;
    },
    [m.SCANDIR_ENTRIES_FETCH_REQUEST] (state) {
        state.entriesFetchingStatus = true;
    },
    [m.SCANDIR_ENTRIES_FETCH_SUCCESS] (state, entries) {
        state.scandirectoryEntries = entries;
        state.entriesFetchingStatus = false;
    },
    [m.SCANDIR_ENTRIES_FETCH_FAILED] (state) {
        state.entriesFetchingStatus = false;
    },
    [m.SCANDIR_ACTION_REQUEST] (state, path) {
        state.actionStatuses.set(path, true);
    },
    [m.SCANDIR_ACTION_SUCCESS] (state, path) {
        state.actionStatuses.delete(path);
        store.dispatch(a.FETCH_SCANDIR_INFOS);
    },
    [m.SCANDIR_ACTION_FAILED] (state, path) {
        state.actionStatuses.delete(path);
    },
    [m.SCANDIR_DELETE_REQUEST] (state, hash) {
        state.deleteStatuses.set(hash, true);
    },
    [m.SCANDIR_DELETE_SUCCESS] (state, hash) {
        state.deleteStatuses.delete(hash);
        store.dispatch(a.FETCH_SCANDIR_INFOS);
    },
    [m.SCANDIR_DELETE_FAILED] (state, hash) {
        state.deleteStatuses.delete(hash);
    }
};

const actions = {
    [a.FETCH_SCANDIR_INFOS] ({ commit }) {
        commit(m.SCANDIR_INFOS_FETCH_REQUEST);
        return backend.scandirectory.fetchAllScanDirectories()
            .then(response => {
                commit(m.SCANDIR_INFOS_FETCH_SUCCESS, response);
            })
            .catch(err => {
                commit(m.SCANDIR_INFOS_FETCH_FAILED);
                throw err;
            });
    },
    [a.FETCH_SCANDIR_ENTRIES] ({ commit }, hash) {
        commit(m.SCANDIR_ENTRIES_FETCH_REQUEST);
        return backend.scandirectory.fetchEntries(hash)
            .then(response => {
                commit(m.SCANDIR_ENTRIES_FETCH_SUCCESS, response);
            })
            .catch(err => {
                commit(m.SCANDIR_ENTRIES_FETCH_FAILED);
                throw err;
            });
    },
    [a.DELETE_SCANDIR] ({ commit }, hash) {
        console.log(hash);
        commit(m.SCANDIR_DELETE_REQUEST, hash);
        return backend.scandirectory.delete(hash)
            .then(response => {
                commit(m.SCANDIR_DELETE_SUCCESS, hash);
                return response;
            })
            .catch(err => {
                commit(m.SCANDIR_DELETE_FAILED, hash);
                throw err;
            });
    },
    [a.SCAN_SCANDIR] ({ commit }, path) {
        commit(m.SCANDIR_ACTION_REQUEST, path);
        return backend.scandirectory.startProcessing(path)
            .then(response => {
                commit(m.SCANDIR_ACTION_SUCCESS, path);
                console.log(response);
                return response;
            })
            .catch(err => {
                commit(m.SCANDIR_ACTION_FAILED, path);
                throw err;
            });
    },
    [a.PAUSE_SCANDIR] ({ commit }, path) {
        commit(m.SCANDIR_ACTION_REQUEST, path);
        return backend.scandirectory.pause(path)
            .then(response => {
                commit(m.SCANDIR_ACTION_SUCCESS, path);
                return response;
            })
            .catch(err => {
                commit(m.SCANDIR_ACTION_FAILED, path);
                throw err;
            });
    },
    [a.RESUME_SCANDIR] ({ commit }, path) {
        commit(m.SCANDIR_ACTION_REQUEST, path);
        return backend.scandirectory.resume(path)
            .then(response => {
                commit(m.SCANDIR_ACTION_SUCCESS, path);
                return response;
            })
            .catch(err => {
                commit(m.SCANDIR_ACTION_FAILED, path);
                throw err;
            });
    },
    [a.CLEANUP_SCANDIR] ({ commit }, path) {
        commit(m.SCANDIR_ACTION_REQUEST, path);
        return backend.scandirectory.cleanup(path)
            .then(response => {
                commit(m.SCANDIR_ACTION_SUCCESS, path);
                return response;
            })
            .catch(err => {
                commit(m.SCANDIR_ACTION_FAILED, path);
                throw err;
            });
    }
};

export default {
    state,
    actions,
    mutations,
    getters
};
