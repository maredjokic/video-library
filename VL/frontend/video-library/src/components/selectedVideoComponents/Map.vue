<template>
    <div id="mymap"></div>
</template>

<script>
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
export default {
    name: 'vue-map',
    props: ['geolocation'],
    data () {
        return {
            cordinatesLine: [],
            cordinatesPolygon: [],
            mymap: null
        };
    },
    methods: {
        draw () {
            if (this.geolocation.cameraLine !== null) {
                var line = L.polyline(this.geolocation.cameraLine.coordinates, { color: '#82B1FF' }).addTo(this.mymap);
                this.mymap.fitBounds(line.getBounds());
            }
            if (this.geolocation.filmedArea !== null) {
                var poly = L.polygon(this.geolocation.filmedArea.coordinates, { color: '#1976D2' }).addTo(this.mymap);
                this.mymap.fitBounds(poly.getBounds());
            }
        }
    },
    mounted: function () {
        console.log(this.geolocation);
        console.log(this.geolocation.cameraLine);
        this.mymap = L.map('mymap').setView([44.505, 21], 6);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(this.mymap);
        this.draw();
    },
    computed: {
    }
};
</script>

<style>
#mymap {
    min-width: 300px;
    min-height: 300px;
    width: 30%;
    height: 20%;
}
</style>
