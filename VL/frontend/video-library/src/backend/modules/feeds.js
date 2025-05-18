import * as s from '../../settings';

export const feeds = {
    /**
     * get all feeds
     * @return array od feeds
     */
    fetchAll () {
        return fetch(s.BASE_URL + 'feeds', {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * fetch feed with specified id
     * @param id of the feed
     * @returns feed
     */
    fetch (id) {
        return fetch(s.BASE_URL + 'feeds/' + id, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * deletes feed with specified id
     * @param id of the feed
     * @returns deleted feed
     */
    delete (id) {
        return fetch(s.BASE_URL + 'feeds/' + id, {
            method: 'DELETE'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * add new feed
     * @param new feed to be added { videoId,  URL, Loop}
     * @returns newly created tag
     */
    add (feed) {
        console.log(feed);
        return fetch(s.BASE_URL + 'feeds', {
            method: 'POST',
            body: JSON.stringify(feed),
            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            return response.json();
        });
    },
    /**
     * start again feed
     * @param feed id
     * @return feeds
     */
    startagain (id) {
        console.log(id);
        return fetch(s.BASE_URL + 'feeds/startagain', {
            method: 'POST',
            body: JSON.parse(id),

            headers: {
                'Content-Type': 'application/json',
                accept: '*/*'
            }
        }).then(response => {
            console.log(response);
            return response.json();
        });
    }
};
