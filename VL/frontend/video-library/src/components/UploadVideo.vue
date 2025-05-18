<template>
    <div class="ml-auto">
        <v-container fluid>
        <v-row>
            <v-file-input show-size counter color="white" @change="uploadFile" label="Upload"></v-file-input>
        </v-row>
    </v-container>
    </div>
</template>

<script>
import * as a from '../store/action_types';
export default {
    name: 'UploadVideo',
    data () {
        return {
            file: '',
            respose: ''
        };
    },
    methods:
    {
        uploadFile (e) {
            const newFile = e;
            console.log(newFile);
            console.log(newFile.name);
            this.$store.dispatch(a.UPLOAD_VIDEO, { file: newFile, fileName: newFile.name });
        }
    }
};
</script>
