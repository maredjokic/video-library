import * as m from '../mutation_types';
import * as a from '../action_types';
import * as s from '../../settings';
import { backend } from '../../backend/index';

// initial state
const state = {
    videos: {},
    fetchingVideos: false,
    video: null,
    fetchingVideoById: false,
    uploadingTasks: [],
    tags: [],
    geolocations: [],
    processingVideos: {},
    fetchingProcessingVideos: false,
    deletingVideo: false,
    addingTag: false
};

const getters = {
    videos: state => state.videos.data,
    video: state => state.video,
    videoData: state => state.videos,
    fetchingVideos: state => state.fetchingVideos,
    fetchingVideoById: state => state.fetchingVideoById,
    deletingVideo: state => state.deletingVideo,
    tags: state => state.tags,
    geolocations: state => state.geolocations,
    processingVideos: state => state.processingVideos.data,
    fetchingProcessingVideos: state => state.fetchingProcessingVideos,
    addingTag: state => state.addingTag
};

const mutations = {
    [m.VIDEOS_REQUEST] (state) {
        state.fetchingVideos = true;
    },
    [m.VIDEOS_SUCCESS] (state, videos) {
        state.fetchingVideos = false;
        state.videos = videos;
    },
    [m.VIDEOS_FAILED] (state) {
        state.fetchingVideos = false;
        state.videos = null;
    },
    [m.VIDEO_BY_ID_REQUEST] (state) {
        state.fetchingVideoById = true;
    },
    [m.VIDEO_BY_ID_SUCCESS] (state, video) {
        state.fetchingVideoById = false;
        state.video = video;
    },
    [m.VIDEO_BY_ID_FAILED] (state) {
        state.fetchingVideoById = false;
        state.video = null;
    },
    [m.DELETE_VIDEO_REQUEST] (state) {
        state.deletingVideo = true;
    },
    [m.DELETE_VIDEO_SUCCESS] (state) {
        state.deletingVideo = false;
    },
    [m.DELETE_VIDEO_FAILED] (state) {
        state.deletingVideo = false;
    },
    [m.UPLOAD_START] (state, name) {
        if (state.uploadingTasks === null) {
            state.uploadingTasks = [];
        }

        const index = state.uploadingTasks.length + 1;

        state.uploadingTasks.push({
            index,
            name,
            progress: 0,
            status: '0%'
        });
    },
    [m.UPLOAD_PROGRESS] (state, { name, progress }) {
        if (!state.uploadingTasks) {
            console.log('[VIDEOS] UPLOAD_PROGRESS assert: uploadingTasks === null!');
            return;
        }

        const task = state.uploadingTasks.find(t => t.name === name);
        if (!task) {
            console.log('[VIDEOS] UPLOAD_PROGRESS assert: task not found: ' + name);
            return;
        }

        task.progress = progress;
        task.status = Math.trunc(progress) + '%';
    },
    [m.UPLOAD_SUCCESS] (state, name) {
        if (!state.uploadingTasks) {
            console.log('[VIDEOS] UPLOAD_FINISHED assert: uploadingTasks === null!');
            return;
        }

        const task = state.uploadingTasks.find(t => t.name === name);
        if (!task) {
            console.log('[VIDEOS] UPLOAD_FINISHED assert: task not found: ' + name);
            return;
        }

        const index = state.uploadingTasks.indexOf(task);
        state.uploadingTasks.splice(index, 1);
    },
    [m.UPLOAD_FAILED] (state, name) {
        if (!state.uploadingTasks) {
            console.log('[VIDEOS] UPLOAD_FAILED assert: uploadingTasks === null!');
            return;
        }

        const task = state.uploadingTasks.find(t => t.name === name);
        if (!task) {
            console.log('[VIDEOS] UPLOAD_FAILED assert: task not found: ' + name);
            return;
        }

        task.failed = true;
    },
    [m.TAGS_SET] (state, thistag) {
        if (state.tags.find(x => x.id === thistag.id)) {
            state.tags.find(x => x.id === thistag.id).data = thistag.data;
        } else {
            state.tags.push(thistag);
        }
    },
    [m.GEOLOCATION_SET] (state, thisgeolocations) {
        state.geolocations.push(thisgeolocations);
    },
    [m.PROCESSING_VIDEOS_REQUEST] (state) {
        state.fetchingProcessingVideos = true;
    },
    [m.PROCESSING_VIDEOS_SUCCESS] (state, videos) {
        state.fetchingProcessingVideos = false;
        state.processingVideos = videos;
    },
    [m.PROCESSING_VIDEOS_FAILED] (state) {
        state.fetchingProcessingVideos = false;
        state.processingVideos = null;
    },
    [m.ADD_TAG_REQUEST] (state) {
        state.addingTag = true;
    },
    [m.ADD_TAG_SUCCESS] (state) {
        state.addingTag = false;
    },
    [m.ADD_TAG_FAILED] (state) {
        state.addingTag = false;
    }
};

const actions = {
    [a.FETCH_VIDEOS] ({ commit }, { pageSize, page, silently, fileName, codecName, formatLongName, tags }) {
        if (!silently) {
            commit(m.VIDEOS_REQUEST);
        }
        var fn = '';
        if (codecName) {
            fn = fn + '&codecName=' + codecName;
        }
        if (fileName) {
            fn = fn + '&fileName=' + fileName;
        }
        if (formatLongName) {
            fn = fn + '&formatLongName=' + formatLongName;
        }
        if (tags && tags.length > 0) {
            fn = fn + '&tags=' + tags;
        }
        return backend.videos.fetchAll('pageSize=' + pageSize + '&page=' + page + fn)
            .then(videos => {
                commit(m.VIDEOS_SUCCESS, videos);
            })
            .catch(err => {
                console.log('[ERR] fetchAll videos');
                console.log(err);
                commit(m.VIDEOS_FAILED);
                throw err;
            });
    },
    [a.UPLOAD_VIDEO] ({ commit }, { file, fileName }) {
        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            commit(m.UPLOAD_START, fileName);
            const data = new FormData();
            data.append('file', file);

            xhr.upload.addEventListener('progress', function (e) {
                const progress = parseFloat((e.loaded / e.total).toFixed(2)) * 100;
                commit(m.UPLOAD_PROGRESS, { name: fileName, progress });
            }, false);
            xhr.addEventListener('load', function (e) {
                commit(m.UPLOAD_SUCCESS, fileName);
                resolve();
            }, false);
            xhr.addEventListener('error', function (e) {
                commit(m.UPLOAD_FAILED, fileName);
                console.log('[ERR] Upload failed');
                console.log(e);
                reject(new Error('UPLOAD_FAILED'));
            }, false);
            xhr.addEventListener('abort', function (e) {
                console.log('Upload canceled');
            }, false);

            xhr.open('POST', s.BASE_URL + 'videos');
            xhr.send(data);
        });
    },
    [a.FETCH_TAGS] ({ commit }, { id, number }) {
        console.log(number);
        number = number === undefined ? 100 : number;
        return backend.videos.getUniqueTags(id, 'pageSize=' + number)
            .then(response => {
                commit(m.TAGS_SET, { id: id, data: response.data });
            })
            .catch(err => {
                throw err;
            });
    },
    [a.FETCH_GEOLOCATION] ({ commit }, id) {
        return backend.videos.getGeolocation(id)
            .then(response => {
                commit(m.GEOLOCATION_SET, response);
                console.log(response);
            })
            .catch(err => {
                throw err;
            });
    },
    [a.FETCH_PROCESSING_VIDEOS] ({ commit }) {
        commit(m.PROCESSING_VIDEOS_REQUEST);
        return backend.videos.getProcessing('pageSize=200')
            .then(videos => {
                commit(m.PROCESSING_VIDEOS_SUCCESS, videos);
            })
            .catch(err => {
                console.log('[ERR] getProcessing videos');
                console.log(err);
                commit(m.PROCESSING_VIDEOS_FAILED);
                throw err;
            });
    },
    [a.FETCH_VIDEO_BY_ID] ({ commit }, id) {
        commit(m.VIDEO_BY_ID_REQUEST);
        return backend.videos.fetch(id)
            .then(response => {
                commit(m.VIDEO_BY_ID_SUCCESS, response);
                console.log(response);
                return response;
            })
            .catch(err => {
                commit(m.VIDEO_BY_ID_FAILED);
                throw err;
            });
    },
    [a.DELETE_VIDEO] ({ commit }, id) {
        commit(m.DELETE_VIDEO_REQUEST);
        return backend.videos.delete(id)
            .then(response => {
                commit(m.DELETE_VIDEO_SUCCESS);
                console.log(response);
                return response;
            })
            .catch(err => {
                commit(m.DELETE_VIDEO_FAILED);
                throw err;
            });
    },
    [a.ADD_NEW_TAG] ({ commit }, { tagName, videoId }) {
        commit(m.ADD_TAG_REQUEST);
        return backend.videos.addTag(videoId, { tagName: tagName, from: 0, to: 0 })
            .then(response => {
                commit(m.ADD_TAG_SUCCESS);
                console.log(response);
                return response;
            })
            .catch(err => {
                commit(m.ADD_TAG_FAILED);
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
