import * as s from '../../settings';

export const tags = {
    /**
     * get all tags
     * @returns array if all tags
     */
    fetchAll () {
        return fetch(s.BASE_URL + 'tags', {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * add new tag
     * @param tagName new tag to be added {Name: tagName(string)}
     * @returns newly created tag
     */
    add (tagName) {
        return fetch(s.BASE_URL + 'tags', {
            method: 'POST',
            body: JSON.stringify(tagName)
        }).then(response => {
            return response.json();
        }).then(response => {
            return response;
        });
    },
    /**
     * get tag by name
     * @param name tag name
     * @returns tag object
     */
    fetch (name) {
        return fetch(s.BASE_URL + 'tags/' + name, {
            method: 'GET'
        }).then(response => {
            return response.json();
        });
    },
    /**
     * @param oldName old tag name
     * @param newName new tag name
     * @returns updated tag
     */
    rename (oldName, newName) {
        return fetch(s.BASE_URL + 'tags/' + oldName, {
            method: 'PUT',
            body: JSON.stringify(newName)
        }).then(response => {
            return response.json();
        });
    },
    /**
     * delete tag
     * @param name tag name
     * @returns deleted tag
     */
    delete (name) {
        return fetch(s.BASE_URL + 'tags/' + name, {
            method: 'DELETE'
        }).then(response => {
            return response.json();
        });
    }
};
