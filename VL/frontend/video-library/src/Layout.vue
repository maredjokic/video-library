<template>
<div id="app">
<v-app>

  <v-app-bar app color="primary">
    <a><router-link v-bind:to="'/'" exact>
      <v-btn large text class="ma-2" color="white">
        Videos
        <v-icon>mdi-play-circle</v-icon>
      </v-btn>
    </router-link></a>
    <a><router-link v-bind:to="'/feeds'" exact>
      <v-btn large text class="ma-2" color="white">
        Streams
        <v-icon>mdi-play-speed</v-icon>
      </v-btn>
    </router-link></a>
    <a><router-link v-bind:to="'/processing'" exact>
      <v-btn large text class="ma-2" color="white">
        Processing
        <v-icon>mdi-progress-clock</v-icon>
      </v-btn>
    </router-link></a>
    <a><router-link v-bind:to="'/scandir'" exact>
      <v-btn large text class="ma-2" color="white">
        Scan Directories
        <v-icon>mdi-folder-open-outline</v-icon>
      </v-btn>
    </router-link></a>
    <video-upload></video-upload>
  </v-app-bar>

  <!-- Sizes your content based upon application components -->
  <v-content>

    <!-- Provides the application the proper gutter -->
    <v-container fluid>

      <!-- If using vue-router -->
      <router-view></router-view>
    </v-container>
  </v-content>

  <v-footer app>
    <!-- -->
  </v-footer>
</v-app>

</div>
</template>

<script>
import UploadVideo from '../src/components/UploadVideo.vue';

export default {
    name: 'Layot',
    data () {
        return {
            activeBtn: [0, 1, 2]
        };
    },
    components: {
        'video-upload': UploadVideo
    }
};
</script>

<style>
</style>
