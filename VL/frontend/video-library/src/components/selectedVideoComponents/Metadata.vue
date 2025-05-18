<template>
  <v-carousel
    v-if=video
    height="250"
    show-arrows
    hide-delimiters
  >
    <v-carousel-item
      v-for="(slide, i) in slides"
      :key="i"
    >
      <v-sheet
        :color="colors[i]"
        height="100%"
      >
        <v-row justify="space-around" v-if="video !== undefined"
        >
            <div v-if="i === 0">
                <p  class="subtitle-2 text-center">BASIC INFORMATION</p>
                <p  class="subtitle-2 text-center">Format long name: {{video.formatLongName}}</p>
                <p  class="subtitle-2 text-center">Codec name: {{video.codecName}}</p>
                <p  class="subtitle-2 text-center">Width x height: {{video.height}} x {{video.width}}</p>
                <p  class="subtitle-2 text-center">Bit rate: {{video.bitRate}}</p>
                <p  class="subtitle-2 text-center">Size in bytes: {{video.size}}</p>
                <p  class="subtitle-2 text-center">Path: {{video.video}}</p>
            </div>
            <div v-if="i === 1">
              <h5>{{video.klV2JSON}}</h5>
            </div>
            <div v-if="i === 2">
              <h5>{{video.ffProbe}}</h5>
            </div>
        </v-row>
      </v-sheet>
    </v-carousel-item>
  </v-carousel>
</template>

<script>
export default {
    name: 'metadata',
    props: ['video'],
    data () {
        return {
            colors: [
                'secondary',
                'primary',
                'secondary',
                'primary',
                'secondary'
            ],
            slides: [
                'First',
                'Second',
                'Third',
                'Fourth',
                'Fifth'
            ]
        };
    }
};
</script>
