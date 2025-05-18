import * as m from '../mutation_types';
import * as a from '../action_types';
import { backend } from '../../backend/index';

// initial state
const state = {
    feeds: [],
    fetchingStatus: false,
    deletingStatuses: new Map(),
    restartingStatuses: new Map(),
    startingStatuses: new Map()
};

const getters = {
    feeds: state => state.feeds,
    feedsFetchingStatus: state => state.fetchingStatus,
    feedsDeletingStatuses: state => state.deletingStatuses,
    feedsRestartingStatuses: state => state.restartingStatuses,
    feedsStartingStatuses: state => state.startingStatuses
};

const mutations = {
    [m.FEEDS_FETCH_REQUEST] (state) {
        state.fetchingStatus = true;
    },
    [m.FEEDS_FETCH_SUCCESS] (state, allFeeds) {
        state.feeds = allFeeds;
        state.fetchingStatus = false;
    },
    [m.FEEDS_FETCH_FAILED] (state) {
        state.fetchingStatus = false;
    },
    [m.FEED_DELETE_REQUEST] (state, id) {
        state.deletingStatuses.set(id, true);
    },
    [m.FEED_DELETE_SUCCESS] (state, id) {
        state.deletingStatuses.delete(id);
    },
    [m.FEED_DELETE_FAILED] (state, id) {
        state.deletingStatuses.delete(id);
    },
    [m.FEED_RESTART_REQUEST] (state, id) {
        state.restartingStatuses.set(id, true);
    },
    [m.FEED_RESTART_SUCCESS] (state, id) {
        state.restartingStatuses.delete(id);
    },
    [m.FEED_RESTART_FAILED] (state, id) {
        state.restartingStatuses.delete(id);
    },
    [m.FEED_START_REQUEST] (state, id) {
        state.startingStatuses.set(id, true);
    },
    [m.FEED_START_SUCCESS] (state, id) {
        state.startingStatuses.delete(id);
    },
    [m.FEED_START_FAILED] (state, id) {
        state.startingStatuses.delete(id);
    }
};

const actions = {
    [a.FETCH_FEEDS] ({ commit }) {
        commit(m.FEEDS_FETCH_REQUEST);
        return backend.feeds.fetchAll()
            .then(response => {
                commit(m.FEEDS_FETCH_SUCCESS, response);
            })
            .catch(err => {
                commit(m.FEEDS_FETCH_FAILED);
                throw err;
            });
    },
    [a.DELETE_FEED] ({ commit, dispatch }, id) {
        commit(m.FEED_DELETE_REQUEST);
        return backend.feeds.delete(id)
            .then(response => {
                commit(m.FEED_DELETE_SUCCESS);
                dispatch(a.FETCH_FEEDS);
            })
            .catch(err => {
                commit(m.FEED_DELETE_FAILED);
                throw err;
            });
    },
    [a.RESTART_FEED] ({ commit, dispatch }, id) {
        commit(m.FEED_RESTART_REQUEST);
        return backend.feeds.startagain(id)
            .then(response => {
                commit(m.FEED_RESTART_SUCCESS);
                dispatch(a.FETCH_FEEDS);
            })
            .catch(err => {
                commit(m.FEED_RESTART_FAILED);
                throw err;
            });
    },
    [a.START_FEED] ({ commit, dispatch }, feed) {
        commit(m.FEED_START_SUCCESS);
        console.log(feed);
        return backend.feeds.add(feed)
            .then(response => {
                commit(m.FEED_START_SUCCESS);
                dispatch(a.FETCH_FEEDS);
                return response;
            })
            .catch(err => {
                commit(m.FEED_START_FAILED);
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
