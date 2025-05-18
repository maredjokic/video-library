import Vue from 'vue';
import Vuex from 'vuex';
import videos from './modules/videos';
import feeds from './modules/feeds';
import scandirectory from './modules/scandirectory';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
    },
    mutations: {
    },
    actions: {
    },
    modules: {
        videos,
        feeds,
        scandirectory
    }
});
