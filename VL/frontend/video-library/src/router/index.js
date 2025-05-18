import Vue from 'vue';
import VueRouter from 'vue-router';
import Videos from '../views/Videos.vue';
import SelectedVideo from '../views/SelectedVideo.vue';
import Feeds from '../views/Feeds.vue';
import StartFeed from '../views/StartFeed.vue';
import ProcessingVideos from '../views/ProcessingVideos.vue';
import ScanDirectory from '../views/ScanDirectory.vue';
import ScanDirectoryEntries from '../views/ScanDirectoryEntries.vue';
import SearchVideos from '../views/SearchVideos.vue';

Vue.use(VueRouter);

const routes = [
    {
        path: '/',
        name: 'Vidoes',
        component: Videos
    },
    {
        path: '/video/:id',
        name: SelectedVideo,
        component: SelectedVideo
    },
    {
        path: '/feeds',
        name: 'Feeds',
        component: Feeds

    },
    {
        path: '/startFeed/:id',
        name: StartFeed,
        component: StartFeed
    },
    {
        path: '/processing',
        name: 'Processing',
        component: ProcessingVideos
    },
    {
        path: '/scandir',
        name: ScanDirectory,
        component: ScanDirectory
    },
    {
        path: '/scandir/:hash',
        name: ScanDirectoryEntries,
        component: ScanDirectoryEntries
    },
    {
        path: '/Videos/search/:name',
        name: 'SearchVideos',
        component: SearchVideos
    }
//   {
//     path: '/about',
//     name: 'About',
//     // route level code-splitting
//     // this generates a separate chunk (about.[hash].js) for this route
//     // which is lazy-loaded when the route is visited.
//     component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
//   }
];

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes
});

export default router;
