import * as s from '../../settings';

export const videos = {
    /**
     * fetch all videos (from first page?)
     * @param query string (can be empty)
     * @returns object with paging info and array of videos
     */
    fetchAll (query) {
        let url = s.BASE_URL + 'videos';
        if (typeof query === 'string' && query !== '') {
            url += '?' + query;
        }

        return fetch(url, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * fetch video with specified id
     * @param id of the video
     * @returns video
     */
    fetch (id) {
        return fetch(s.BASE_URL + 'videos/' + id, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * upload video
     * @param formData video file
     * @returns video data
     */
    uploadFile (file) {
        const formData = new FormData();
        formData.append('file', file);
        console.log(formData);
        var request = new XMLHttpRequest();
        request.open('POST', s.BASE_URL + 'videos');
        request.send(formData);
    },
    /**
     * rename video with specified id
     * @param id of the video
     * @param fileName new video name
     * @returns renamed video
     */
    rename (id, fileName) {
        return fetch(s.BASE_URL + 'videos/' + id, {
            method: 'PUT',
            body: JSON.stringify(fileName)
        }).then(response => {
            return response.json();
        });
    },
    /**
     * deletes video with specified id
     * @param id of the video
     * @returns deleted video
     */
    delete (id) {
        return fetch(s.BASE_URL + 'videos/' + id, {
            method: 'DELETE'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * get related videos by fpMatch score
     * @param id of the video
     * @param query string to sort and filter results (can be empty) (works?? page and sort)
     * @returns video array
     */
    getRelatedFp (id, query) {
        let url = s.BASE_URL + 'videos' + id + '/relatedfp';
        if (typeof query === 'string' && query !== '') {
            url += '?' + query;
        }

        return fetch(url, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * get related videos with the same tags
     * @param id of the video
     * @param query string to sort and filter results (can be empty) (works??)
     * @returns video array
     */
    getRelatedTags (id, query) {
        return fetch(s.BASE_URL + 'videos/' + id + '/relatedtags', {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * get videos which are currently being processed
     * @param query string to sort and filter results (can be empty) (works??)
     * @returns video array
     */
    getProcessing (query) {
        let url = s.BASE_URL + 'videos/processing';
        if (typeof query === 'string' && query !== '') {
            url += '?' + query;
        }

        return fetch(url, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * get videos which are currently being processed
     * @param id of the video
     * @returns area and crosstrail for the video
     */
    getGeolocation (id) {
        return fetch(s.BASE_URL + 'videos/' + id + '/geolocation/lat-long', {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * add new tag to the video
     * @param id of the video
     * @param tag - { TagName: tagName(string), From: from(int), To: to(int) }
     * @returns updated video
     */
    addTag (id, tag) {
        return fetch(s.BASE_URL + 'videos/' + id + '/tags', {
            method: 'POST',
            body: JSON.stringify(tag),
            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            return response.json();
        }).then(response => {
            return response;
        });
    },
    /**
     * get all tags associated with the video
     * @param id of the video
     * @param query string to sort and filter results (can be empty) (works??)
     * @returns paginated list of tags
     */
    getTags (id, query) {
        let url = s.BASE_URL + 'videos/' + id + '/tags';
        if (typeof query === 'string' && query !== '') {
            url += '?' + query;
        }

        return fetch(url, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * deletes specified tag from the video
     * @param id of the video
     * @param tag - { TagName: tagName(string), From: from(int), To: to(int) }
     * @returns deleted tag
     */
    deleteTag (id, tag) {
        return fetch(s.BASE_URL + 'videos/' + id + 'tags', {
            method: 'DELETE',
            body: JSON.stringify(tag)
        }).then(response => {
            if (response.ok) {
                return {};
            }
            return response.json();
        });
    },
    /**
     * get all unique tags associated with video
     * @param id of the video
     * @param query string to sort and filter results (can be empty) (works??)
     * @returns paginated list of unique tags associated with video
     */
    getUniqueTags (id, query) {
        let url = s.BASE_URL + 'videos/' + id + '/tags/unique';
        if (typeof query === 'string' && query !== '') {
            url += '?' + query;
        }

        return fetch(url, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * delete all occurrences of the tag from video
     * @param id of the video
     * @param tagName name of the tag to be deleted {TagName: tagName(string)}
     * @returns deleted tag
     */
    deleteUniqueTag (id, tagName) {
        return fetch(s.BASE_URL + 'videos/' + id + '/tags/unique', {
            method: 'DELETE',
            body: JSON.stringify(tagName)
        }).then(response => {
            if (response.ok) {
                return {};
            }
            return response.json();
        });
    }
};
