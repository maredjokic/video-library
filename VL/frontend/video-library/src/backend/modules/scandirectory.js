import * as s from '../../settings';

export const scandirectory = {
    /**
    * get all scan directories
    * @return array of directory info recource
    */
    fetchAllScanDirectories () {
        return fetch(s.BASE_URL + 'ScanDirectory', {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
    * get all scan directory entries
    * @return array of entry
    */
    fetchEntries (hash) {
        return fetch(s.BASE_URL + 'ScanDirectory/' + hash + '/details', {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
    * deletes scan directory and all entries
    * @param hash of the feed
    * @returns deleted feed hash
    */
    delete (hash) {
        return fetch(s.BASE_URL + 'ScanDirectory/' + hash, {
            method: 'DELETE'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * process directory
     * @param { "path": "path" }
     * @returns hash
     */
    startProcessing (path) {
        console.log(path);
        return fetch(s.BASE_URL + 'ScanDirectory/', {
            method: 'POST',
            body: JSON.stringify(path),
            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            return response;
        });
    },
    /**
     * pause directory processing
     * @param { "path": "path" }
     * @returns hash
     */
    pause (path) {
        console.log(path);
        return fetch(s.BASE_URL + 'ScanDirectory/pause/', {
            method: 'POST',
            body: JSON.stringify(path),
            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            return response;
        });
    },
    /**
     * resume directory processing
     * @param { "path": "path" }
     * @returns hash
     */
    resume (path) {
        console.log(path);
        return fetch(s.BASE_URL + 'ScanDirectory/resume/', {
            method: 'POST',
            body: JSON.stringify(path),
            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            return response;
        });
    },
    /**
     * cleanup directory
     * @param { "path": "path" }
     * @returns hash
     */
    cleanup (path) {
        console.log(path);
        return fetch(s.BASE_URL + 'ScanDirectory/cleanup/', {
            method: 'POST',
            body: JSON.stringify(path),
            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            return response;
        });
    }
};
