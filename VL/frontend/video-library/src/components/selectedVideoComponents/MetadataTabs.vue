<template>
  <v-tabs
    fixed-tabs
    background-color="default"
    co
    dark
  >
    <v-tab>
      Option
    </v-tab>
    <v-tab>
      Another Selection
    </v-tab>
    <v-tab>
      Items
    </v-tab>
    <v-tab>
      Another Screen
    </v-tab>
  </v-tabs>
</template>

<script>
export default {
    name: 'metadataTabs',
    props: ['video'],
    data () {
        return {
        };
    }
};
</script>
